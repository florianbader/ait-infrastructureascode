using Pulumi;
using AIT.InfrastructureAsCode;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Program
{
    public static Task<int> Main()
        => Deployment.RunAsync(() =>
        {
            _ = new Infrastructure();
            return new Dictionary<string, object?>();
        });
}