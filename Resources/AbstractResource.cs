namespace AIT.InfrastructureAsCode.Resources
{
    public abstract class AbstractResource
    {
        private readonly ResourceGroupResource _resourceGroupResource;

        public string Location => _resourceGroupResource.Location;

        public string Name { get; }

        public string ResourceGroupName => _resourceGroupResource.Name;

        public AbstractResource(ResourceGroupResource resourceGroupResource, string name,
            bool nameHasHyphens = true, bool nameHasEnvironment = true)
        {
            _resourceGroupResource = resourceGroupResource;

            Name = resourceGroupResource.GetResourceName(name, nameHasHyphens, nameHasEnvironment);
        }

        public string GetResourceName(string name, bool nameHasHyphens = true, bool nameHasEnvironment = true)
            => _resourceGroupResource.GetResourceName(name, nameHasHyphens, nameHasEnvironment);
    }
}