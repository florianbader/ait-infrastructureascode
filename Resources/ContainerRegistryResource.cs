using Pulumi;
using Pulumi.Azure.ContainerService;

namespace AIT.InfrastructureAsCode.Resources
{
    public class ContainerRegistryResource : AbstractResource
    {
        public ContainerRegistryResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "acr", nameHasHyphens: false, nameHasEnvironment: false)
        {
        }

        public void Build()
        {
            var config = new Config("containerregistry");

            new Registry(Name, new RegistryArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                AdminEnabled = true,
                Sku = config.Require("sku")
            }); ;
        }
    }
}