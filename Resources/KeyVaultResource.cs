using Pulumi;
using Pulumi.Azure.KeyVault;
using AIT.InfrastructureAsCode.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIT.InfrastructureAsCode
{
    public class KeyVaultResource : AbstractResource, IConfiguration
    {
        private readonly IDictionary<string, Secret> _secrets = new Dictionary<string, Secret>();
        private KeyVault? _keyVault;

        public IEnumerable<(string Key, Output<string>? Value)> Configuration
            => _secrets.Keys.Select(s => (Key: s, Value: _secrets[s].Id.Apply(id => $"@Microsoft.KeyVault(SecretUri={id})")))
                .Cast<(string Key, Output<string>? Value)>()
                .Union(new (string Key, Output<string>? Value)[]
                {
                    (Key: "KeyVaultName", Value: _keyVault.Name)
                });

        public Output<string>? this[string key]
        {
            get => _secrets[key].Id.Apply(id => $"@Microsoft.KeyVault(SecretUri={id})");
        }

        public KeyVaultResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "kv")
        {
        }

        public void AddAccessPolicy(string name, Output<string>? tenantId, Output<string>? principalId)
        {
            if (tenantId == null)
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (principalId == null)
            {
                throw new ArgumentNullException(nameof(principalId));
            }

            if (_keyVault == null)
            {
                throw new InvalidOperationException("Key vault needs to be build before adding secrets.");
            }

            new AccessPolicy(name + "accesspolicy", new AccessPolicyArgs
            {
                KeyVaultId = _keyVault.Id,
                TenantId = tenantId,
                ObjectId = principalId,
                SecretPermissions = new InputList<string> { "get", "list" }
            });
        }

        public void AddSecrets(ISecrets secrets)
        {
            foreach (var secret in secrets.Secrets)
            {
                if (secret.Value != null)
                {
                    SetSecret(secret.Key, secret.Value);
                }
            }
        }

        public void Build()
        {
            var projectConfig = new Config("project");
            var config = new Config("keyvault");

            _keyVault = new KeyVault(Name, new KeyVaultArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                SkuName = config.Require("sku"),
                TenantId = projectConfig.Require("tenantId")
            });
        }

        public Secret SetSecret(string secretName, string secretValue)
        {
            if (_keyVault == null)
            {
                throw new InvalidOperationException("Key vault needs to be build before adding secrets.");
            }

            var secret = new Secret(secretName, new SecretArgs
            {
                Name = secretName,
                KeyVaultId = _keyVault.Id,
                Value = secretValue
            });

            _secrets.Add(secretName, secret);

            return secret;
        }

        public Secret SetSecret(string secretName, Output<string> secretValue)
        {
            if (_keyVault == null)
            {
                throw new InvalidOperationException("Key vault needs to be build before adding secrets.");
            }

            var secret = new Secret(secretName, new SecretArgs
            {
                Name = secretName,
                KeyVaultId = _keyVault.Id,
                Value = secretValue
            });

            _secrets.Add(secretName, secret);

            return secret;
        }
    }
}