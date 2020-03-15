using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using AIT.InfrastructureAsCode.Resources;

namespace AIT.InfrastructureAsCode
{
    public class AppServicePlanResource : AbstractResource
    {
        private Plan? _appServicePlan;

        public Output<string>? Id => _appServicePlan?.Id;

        public AppServicePlanResource(ResourceGroupResource resourceGroup) : base(resourceGroup, "asp")
        {
        }

        public void Build()
        {
            var config = new Config("appservice");

            _appServicePlan = new Plan(Name, new PlanArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                Kind = "Linux",
                Reserved = true,
                Sku = new PlanSkuArgs
                {
                    Tier = config.Require("tier"),
                    Size = config.Require("size")
                }
            });
        }
    }
}