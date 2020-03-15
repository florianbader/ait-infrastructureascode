using Pulumi;
using System.Collections.Generic;

namespace AIT.InfrastructureAsCode.Resources
{
    public interface IConfiguration
    {
        IEnumerable<(string Key, Output<string>? Value)> Configuration { get; }
    }
}