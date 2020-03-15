using Pulumi;
using Pulumi.Azure.Iot;
using Pulumi.Azure.Iot.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIT.InfrastructureAsCode.Resources
{
    public class IoTHubResource : AbstractResource, ISecrets
    {
        private IoTHub? _iotHub;

        public Output<string>? ConnectionString
            => _iotHub?.SharedAccessPolicies
                .First()
                .Apply(s => $"HostName={Name}.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=${s.PrimaryKey}");

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new (string Key, Output<string>? Value)[]
            {
                (Key: "IoTHubConnectionString", Value: ConnectionString),
                (Key: "EventHubConnectionString", Value: _iotHub.EventHubEventsEndpoint)
            };

        public IoTHubResource(ResourceGroupResource resourceGroupResource)
            : base(resourceGroupResource, "ioth")
        {
        }

        public void Build()
        {
            var config = new Config("iothub");

            _iotHub = new IoTHub(Name, new IoTHubArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                Sku = new IoTHubSkuArgs { Name = config.Require("name"), Capacity = 1 }
            });
        }

        public void ConfigureRouting(ServiceBusResource serviceBus)
        {
        }

        private void CreateServiceBusRoute(ServiceBusResource serviceBus, string queueName, string condition, string source)
        {
            if (serviceBus == null || serviceBus.ConnectionString == null)
            {
                throw new ArgumentNullException(nameof(serviceBus));
            }

            if (_iotHub == null)
            {
                throw new InvalidOperationException("IoT Hub needs to be build before creating routes.");
            }

            var endpoint = new EndpointServicebusQueue(queueName + "endpoint", new EndpointServicebusQueueArgs
            {
                Name = queueName + "endpoint",
                ResourceGroupName = ResourceGroupName,
                IothubName = _iotHub.Name,
                ConnectionString = serviceBus.ConnectionString
            });

            new Route(queueName, new RouteArgs
            {
                Name = queueName,
                ResourceGroupName = ResourceGroupName,
                IothubName = _iotHub.Name,
                Source = source,
                Enabled = true,
                Condition = condition,
                EndpointNames = endpoint.Name
            });
        }
    }
}