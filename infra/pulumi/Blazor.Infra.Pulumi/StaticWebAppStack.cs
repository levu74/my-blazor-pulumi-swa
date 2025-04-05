using Pulumi;
using Az = Pulumi.AzureNative;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Infra.Pulumi
{
    public class StaticWebAppStack : Stack
    {

        [Output("staticWebAppUrl")]
        public Output<string> StaticWebAppUrl { get; private set; }

        [Output("keyVaultName")]
        public Output<string> KeyVaultName { get; private set; }

        [Output("swaDeploymentTokenSecretName")]
        public Output<string> SwaDeploymentTokenSecretName { get; private set; }

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
            var resourceGroup = new Az.Resources.ResourceGroup(resourceGroupName, new()
            {
                ResourceGroupName = resourceGroupName
            });


            // Create an Azure Static Web App
            var staticWebAppName = $"swa-{fullStackName}";
            var staticWebApp = new Az.Web.StaticSite(staticWebAppName, new()
            {
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

            var deploymentToken = staticWebAppSecrets.Apply(secrets =>
            {
                return secrets.Properties.Where(x => x.Key == "apiKey").FirstOrDefault().Value;
            });


            // Create key vault and store deployment tokens in this keyvault
            var keyVaultName = $"kv-{fullStackName}";
            var keyVault = new Az.KeyVault.Vault(keyVaultName, new()
            {
                ResourceGroupName = resourceGroup.Name,
                VaultName = keyVaultName,

                Properties = new Az.KeyVault.Inputs.VaultPropertiesArgs
                {
                    EnablePurgeProtection = false,
                    EnableSoftDelete = false,
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
                SecretName = "swaDeploymentToken",

                Properties = new Az.KeyVault.Inputs.SecretPropertiesArgs
                {
                    Value = deploymentToken,
                    ContentType = "text/plain",
                }
            });

            // Output
            StaticWebAppUrl = staticWebApp.DefaultHostname.Apply(hostname => Output.Create(hostname));
            KeyVaultName = keyVault.Name.Apply(name => Output.Create(name));
            SwaDeploymentTokenSecretName = staticWebAppTokenSecret.Name.Apply(secretName =>Output.Create(secretName));
        }

    }
}
