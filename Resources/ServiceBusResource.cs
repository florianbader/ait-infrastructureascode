using Pulumi;
using Pulumi.Azure.ServiceBus;
using AIT.InfrastructureAsCode.Resources;
using System;
using System.Collections.Generic;

namespace AIT.InfrastructureAsCode
{
    public class ServiceBusResource : AbstractResource, ISecrets
    {
        private Namespace? _serviceBusNamespace;

        public Output<string>? ConnectionString => _serviceBusNamespace?.DefaultPrimaryConnectionString;

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new[] { (Key: "ServiceBusConnectionString", Value: ConnectionString) };

        public ServiceBusResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "sb", nameHasHyphens: false)
        {
        }

        public void Build()
        {
            var config = new Config("servicebus");

            _serviceBusNamespace = new Namespace(Name, new NamespaceArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                Sku = config.Require("sku"),
                Capacity = 0
            });
        }

        private void CreateQueue(string name)
        {
            if (_serviceBusNamespace == null)
            {
                throw new InvalidOperationException("Service Bus needs to be build before creating queues.");
            }

            new Queue(name, new QueueArgs
            {
                Name = name,
                ResourceGroupName = ResourceGroupName,
                NamespaceName = _serviceBusNamespace.Name
            });
        }
    }
}