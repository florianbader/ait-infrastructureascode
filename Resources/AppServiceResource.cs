using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using AIT.InfrastructureAsCode.Resources;
using System;

namespace AIT.InfrastructureAsCode
{
    public class AppServiceResource : AbstractResource
    {
        private readonly InputMap<string> _appSettings;
        private AppService? _appService;

        public Output<string>? PrincipalId => _appService?.Identity.Apply(i => i.PrincipalId);

        public Output<string>? TenantId => _appService?.Identity.Apply(i => i.TenantId);

        public AppServiceResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "app")
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

        public void Build(AppServicePlanResource appServicePlanResource)
        {
            if (appServicePlanResource.Id == null)
            {
                throw new InvalidOperationException("App service plan was not build");
            }

            _appService = new AppService(Name, new AppServiceArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                AppServicePlanId = appServicePlanResource.Id,
                HttpsOnly = true,
                Identity = new AppServiceIdentityArgs
                {
                    Type = "SystemAssigned"
                },

                SiteConfig = new AppServiceSiteConfigArgs
                {
                    LinuxFxVersion = "DOTNETCORE|3.1"
                },

                AppSettings = _appSettings
            });
        }
    }
}