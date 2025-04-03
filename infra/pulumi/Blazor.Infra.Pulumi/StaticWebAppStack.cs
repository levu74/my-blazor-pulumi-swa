using Pulumi;
using Az = Pulumi.AzureNative;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Infra.Pulumi
{
    public class StaticWebAppStack : Stack
    {
        [Output("primaryStorageKey")]
        public Output<string> PrimaryStorageKey { get; private set; }

        [Output("staticWebAppUrl")]
        public Output<string> StaticWebAppUrl { get; private set; }


        public StaticWebAppStack()
        {
            // Current config
            var currentConfig = Az.Authorization.GetClientConfig.Invoke();

            // Get current stack name
            var stackName = Deployment.Instance.StackName;
            var projectName = Deployment.Instance.ProjectName;
            var fullStackName = $"{projectName}-{stackName}";

            // Create an Azure Resource Group with stack-specific name
            var resourceGroupName = $"rg-{fullStackName}";

            // Create an Azure Resource Group
            var resourceGroup = new Az.Resources.ResourceGroup(resourceGroupName, new(){
                ResourceGroupName = resourceGroupName
            });

            // Create an Azure resource (Storage Account)
            var storageAccountName = CreateStorageAccountName("sa", projectName, stackName);

            var storageAccount = new Az.Storage.StorageAccount(storageAccountName, new()
            {
                ResourceGroupName = resourceGroup.Name,
                AccountName = storageAccountName,
                Sku = new Az.Storage.Inputs.SkuArgs
                {
                    Name = Az.Storage.SkuName.Standard_LRS
                },
                Kind = Az.Storage.Kind.StorageV2
            });

            var storageAccountKeys = Az.Storage.ListStorageAccountKeys.Invoke(new()
            {
                ResourceGroupName = resourceGroup.Name,
                AccountName = storageAccount.Name
            });

            PrimaryStorageKey = storageAccountKeys.Apply(accountKeys =>
            {
                var firstKey = accountKeys.Keys[0].Value;
                return Output.CreateSecret(firstKey);
            });

            // Create an Azure Static Web App
            var staticWebAppName = $"swa-{fullStackName}";
            var staticWebApp = new Az.Web.StaticSite(staticWebAppName, new (){
                ResourceGroupName = resourceGroup.Name,
                Name = staticWebAppName,
                Location = resourceGroup.Location,
                Sku = new Az.Web.Inputs.SkuDescriptionArgs
                {
                    Tier = "Free",
                    Name = "Free"
                },
                AllowConfigFileUpdates = true,
            });

            var staticWebAppSecrets = staticWebApp.Id.Apply(staticWebAppId =>
            {
                return Az.Web.ListStaticSiteSecrets.Invoke(new()
                {
                    ResourceGroupName = resourceGroup.Name,
                    Name = staticWebApp.Name
                });
            });

            var deploymentToken = staticWebAppSecrets.Apply(secrets => {
                return secrets.Properties.Where(x => x.Key == "apiKey").FirstOrDefault().Value;
            });

            StaticWebAppUrl = staticWebApp.DefaultHostname.Apply(hostname =>
            {
                return Output.Create(hostname);
            });

            // Create key vault and store deployment tokens in this keyvault
            var keyVaultName = $"kv-{fullStackName}";
            var keyVault = new Az.KeyVault.Vault(keyVaultName, new()
            {
                ResourceGroupName = resourceGroup.Name,
                VaultName = keyVaultName,


                Properties = new Az.KeyVault.Inputs.VaultPropertiesArgs
                {
                    EnableSoftDelete = true,
                    EnableRbacAuthorization = true,
                    Sku = new Az.KeyVault.Inputs.SkuArgs
                    {
                        Family = Az.KeyVault.SkuFamily.A,
                        Name = Az.KeyVault.SkuName.Standard
                    },
                    TenantId = currentConfig.Apply(config => config.TenantId)
                }
            });

            // Create a secret in the key vault to store Static Web App Deployment token
            var staticWebAppTokenSecret = new Az.KeyVault.Secret("staticWebAppTokenSecret", new()
            {
                ResourceGroupName = resourceGroup.Name,
                VaultName = keyVault.Name,
                SecretName = "SWA-Deployment-Token",

                Properties = new Az.KeyVault.Inputs.SecretPropertiesArgs
                {
                    Value = deploymentToken,
                    ContentType = "text/plain",
                }
            });

            // Create a secret in the key vault to store Storage Account Primary Key
            var storageAccountKeySecret = new Az.KeyVault.Secret("storageAccountKeySecret", new()
            {
                ResourceGroupName = resourceGroup.Name,
                VaultName = keyVault.Name,
                SecretName = "Storage-Account-Primary-Key",

                Properties = new Az.KeyVault.Inputs.SecretPropertiesArgs
                {
                    Value = PrimaryStorageKey,
                    ContentType = "text/plain",
                }
            });

        }

        // Helper method to create a valid storage account name
        private static string CreateStorageAccountName(string prefix, string projectName, string stackName)
        {
            int maxStorageNameLength = 16;
            var availableLength = maxStorageNameLength - prefix.Length - stackName.Length;

            var middlePart = "";
            if (availableLength > 0)
            {
                middlePart = projectName.ToLowerInvariant().Replace("-", "");
                if (middlePart.Length > availableLength)
                {
                    middlePart = middlePart.Substring(0, availableLength);
                }
            }

            return $"{prefix}{middlePart}{stackName}".ToLowerInvariant();
        }
    }
}
