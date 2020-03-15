using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using AIT.InfrastructureAsCode.Resources;
using System;

namespace AIT.InfrastructureAsCode
{
    public class FunctionAppResource : AbstractResource
    {
        private readonly InputMap<string> _appSettings;
        private FunctionApp? _functionApp;

        public Output<string>? PrincipalId => _functionApp?.Identity.Apply(i => i.PrincipalId);

        public Output<string>? TenantId => _functionApp?.Identity.Apply(i => i.TenantId);

        public FunctionAppResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "func")
        {
            _appSettings = new InputMap<string>();
        }

        public void AddConfiguration(string key, Output<string>? value)
        {
            if (value == null)
            {
                return;
            }

            _appSettings.Add(key, value);
        }

        public void AddConfiguration(IConfiguration configurationProvider)
        {
            foreach (var configuration in configurationProvider.Configuration)
            {
                AddConfiguration(configuration.Key, configuration.Value);
            }
        }

        public void Build(AppServicePlanResource appServicePlanResource, StorageAccountResource storageAccountResource)
        {
            if (appServicePlanResource.Id == null)
            {
                throw new InvalidOperationException("App service plan was not build.");
            }

            if (storageAccountResource.ConnectionString == null)
            {
                throw new InvalidOperationException("Storage account was not build.");
            }

            _functionApp = new FunctionApp(Name, new FunctionAppArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                AppServicePlanId = appServicePlanResource.Id,
                HttpsOnly = true,
                Identity = new FunctionAppIdentityArgs
                {
                    Type = "SystemAssigned"
                },
                SiteConfig = new FunctionAppSiteConfigArgs
                {
                    LinuxFxVersion = "DOTNETCORE|3.1"
                },
                AppSettings = _appSettings,
                StorageConnectionString = storageAccountResource.ConnectionString
            });
        }
    }
}