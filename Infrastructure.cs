using AIT.InfrastructureAsCode.Resources;

namespace AIT.InfrastructureAsCode
{
    public class Infrastructure
    {
        public Infrastructure()
        {
            var resourceGroup = new ResourceGroupResource();

            var appServicePlan = new AppServicePlanResource(resourceGroup);
            appServicePlan.Build();

            var appService = new AppServiceResource(resourceGroup);

            var functionApp = new FunctionAppResource(resourceGroup);

            var keyVault = new KeyVaultResource(resourceGroup);
            keyVault.Build();

            var azureActiveDirectory = new ActiveDirectoryResource();
            keyVault.AddSecrets(azureActiveDirectory);

            var serviceBus = new ServiceBusResource(resourceGroup);
            serviceBus.Build();
            keyVault.AddSecrets(serviceBus);

            var iotHub = new IoTHubResource(resourceGroup);
            iotHub.Build();
            keyVault.AddSecrets(iotHub);

            var storageAccount = new StorageAccountResource(resourceGroup);
            storageAccount.Build();
            keyVault.AddSecrets(storageAccount);

            var applicationInsights = new ApplicationInsightsResource(resourceGroup);
            applicationInsights.Build();
            appService.AddConfiguration(applicationInsights);

            var sqlDatabase = new SqlServerResource(resourceGroup);
            sqlDatabase.Build();
            keyVault.AddSecrets(sqlDatabase);

            functionApp.AddConfiguration(keyVault);
            appService.AddConfiguration(keyVault);

            appService.AddConfiguration("AzureActiveDirectoryBearer__ClientSecret", keyVault["AzureActiveDirectoryClientSecret"]);
            functionApp.AddConfiguration("AzureActiveDirectoryBearer__ClientSecret", keyVault["AzureActiveDirectoryClientSecret"]);

            appService.Build(appServicePlan);
            functionApp.Build(appServicePlan, storageAccount);

            //keyVault.AddAccessPolicy("appservice", appService.TenantId, appService.PrincipalId);
            //keyVault.AddAccessPolicy("functionapp", functionApp.TenantId, functionApp.PrincipalId);
        }
    }
}