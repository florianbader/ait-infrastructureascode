using Pulumi;
using Pulumi.Azure.AppInsights;
using AIT.InfrastructureAsCode.Resources;
using System.Collections.Generic;

namespace AIT.InfrastructureAsCode
{
    public class ApplicationInsightsResource : AbstractResource, IConfiguration
    {
        private Insights? _applicationInsights;

        public IEnumerable<(string Key, Output<string>? Value)> Configuration
            => new[] { ("APPINSIGHTS_INSTRUMENTATIONKEY", InstrumentationKey) };

        public Output<string>? InstrumentationKey => _applicationInsights?.InstrumentationKey;

        public ApplicationInsightsResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "ai")
        {
        }

        public void Build()
        {
            _applicationInsights = new Insights(Name, new InsightsArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                ApplicationType = "web"
            });
        }
    }
}