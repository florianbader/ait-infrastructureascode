using Pulumi;
using Pulumi.Azure.Storage;
using AIT.InfrastructureAsCode.Resources;
using System.Collections.Generic;

namespace AIT.InfrastructureAsCode
{
    public class StorageAccountResource : AbstractResource, ISecrets
    {
        private Account? _storageAccount;

        public Output<string>? ConnectionString
            => _storageAccount?.PrimaryAccessKey.Apply(s => $"DefaultEndpointsProtocol=https;AccountName=${Name};AccountKey={s};EndpointSuffix=core.windows.net");

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new (string Key, Output<string>? Value)[] { (Key: "StorageConnectionString", Value: ConnectionString) };

        public StorageAccountResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "stg", nameHasHyphens: false)
        {
        }

        public void Build()
        {
            _storageAccount = new Account(Name, new AccountArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                AccountKind = "StorageV2",
                AccessTier = "Hot",
                AccountReplicationType = "LRS",
                AccountTier = "Standard"
            });
        }
    }
}