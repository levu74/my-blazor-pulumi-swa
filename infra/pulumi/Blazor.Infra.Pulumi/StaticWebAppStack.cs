using Pulumi;
using Az = Pulumi.AzureNative;
using System.Collections.Generic;

namespace Blazor.Infra.Pulumi
{
    public class StaticWebAppStack : Stack
    {
        [Output("primaryStorageKey")]
        public Output<string> PrimaryStorageKey { get; private set; }


        public StaticWebAppStack()
        {
            // Get current stack name
            var stackName = Deployment.Instance.StackName;
            var projectName = Deployment.Instance.ProjectName;
            var fullStackName = $"{projectName}-{stackName}";

            // Create an Azure Resource Group with stack-specific name
            var resourceGroupName = $"rg-{fullStackName}";

            // Create an Azure Resource Group
            var resourceGroup = new Az.Resources.ResourceGroup(resourceGroupName);

            // Create an Azure resource (Storage Account)
            var storageAccountName = CreateStorageAccountName("sa", projectName, stackName);

            var storageAccount = new Az.Storage.StorageAccount(storageAccountName, new()
            {
                ResourceGroupName = resourceGroup.Name,
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
