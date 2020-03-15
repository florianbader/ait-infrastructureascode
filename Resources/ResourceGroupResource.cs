using Pulumi;
using Pulumi.Azure.Core;
using System;

namespace AIT.InfrastructureAsCode.Resources
{
    public class ResourceGroupResource
    {
        private readonly ResourceGroup _resourceGroup;

        public string Environment { get; }

        public string Location { get; }

        public string LocationAbbreviation
            => Location switch
            {
                "WestEurope" => "euw",
                _ => throw new ArgumentException("Invalid location"),
            };

        public string Name { get; }

        public string ProductName { get; }

        public ResourceGroupResource(bool nameHasEnvironment = true)
        {
            var azureConfig = new Config("azure");
            var projectConfig = new Config("project");

            ProductName = projectConfig.Require("productName");
            Environment = azureConfig.Require("environment");
            Location = azureConfig.Require("location");
            Name = GetResourceName("rg", nameHasEnvironment: nameHasEnvironment);

            _resourceGroup = new ResourceGroup(Name, new ResourceGroupArgs
            {
                Name = Name,
                Location = Location
            }, new CustomResourceOptions
            {
                Protect = true
            });
        }

        public string GetResourceName(string name, bool nameHasHyphens = true, bool nameHasEnvironment = true)
            => nameHasEnvironment
                ? (nameHasHyphens
                    ? $"{ProductName}-{Environment}-{LocationAbbreviation}-{name}"
                    : $"{ProductName}{Environment}{LocationAbbreviation}{name}")
                : (nameHasHyphens
                    ? $"{ProductName}-{LocationAbbreviation}-{name}"
                    : $"{ProductName}{LocationAbbreviation}{name}");
    }
}