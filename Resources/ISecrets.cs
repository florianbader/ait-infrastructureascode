using Pulumi;
using System.Collections.Generic;

namespace AIT.InfrastructureAsCode
{
    public interface ISecrets
    {
        IEnumerable<(string Key, Output<string>? Value)> Secrets { get; }
    }
}