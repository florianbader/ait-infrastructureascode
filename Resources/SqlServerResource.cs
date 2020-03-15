using Pulumi;
using Pulumi.Azure.Sql;
using AIT.InfrastructureAsCode.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AIT.InfrastructureAsCode
{
    public class SqlServerResource : AbstractResource, ISecrets
    {
        private Output<string>? _password;

        public string? ConnectionString { get; private set; }

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new (string Key, Output<string>? Value)[]
            {
                (Key: "DatabaseConnectionString", Value: Output<string>.Create(Task.FromResult(ConnectionString ?? string.Empty))),
                (Key: "DatabasePassword", Value: _password)
            };

        public SqlServerResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "sqls")
        {
        }

        public void Build()
        {
            var config = new Config("sqlserver");

            var databaseName = GetResourceName("sqld");
            var username = "ServerAdmin";
            _password = config.GetSecret("password");

            if (_password == null)
            {
                throw new InvalidOperationException("SQL Server password configuration is missing.");
            }

            ConnectionString = $"Server=tcp:${Name}.database.windows.net;Initial Catalog=${databaseName};User Id={username};Password={_password};Min Pool Size=0;Max Pool Size=30;Persist Security Info=true;";

            var sqlServer = new SqlServer(Name, new SqlServerArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                Version = "12.0",
                AdministratorLogin = username,
                AdministratorLoginPassword = _password
            });

            new Database(databaseName, new DatabaseArgs
            {
                Name = sqlServer.Name,
                ResourceGroupName = ResourceGroupName,
                Location = Location,
                ServerName = Name,
                Edition = config.Require("edition"),
                RequestedServiceObjectiveName = config.Require("serviceObjectiveName")
            });

            new FirewallRule("Allow all Azure services", new FirewallRuleArgs
            {
                ServerName = sqlServer.Name,
                ResourceGroupName = ResourceGroupName,
                Name = "Allow all Azure services",
                StartIpAddress = "0.0.0.0",
                EndIpAddress = "0.0.0.0"
            });
        }
    }
}