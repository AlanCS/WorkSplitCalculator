using Amazon.CDK;
using System;

namespace CloudConfiguration.Helpers
{
    public static class Extensions
    {
        public static void Log(this Stack scope, string key, string value)
        {
            new CfnOutput(scope, key, new CfnOutputProps
            {
                Value = value
            });
        }

        public static void LogCompletion(this Stack scope)
        {
            var stackIdentifier = $"[{scope.StackName}] for region [{scope.Region}] and account [{scope.Account}]";
            Console.WriteLine($"Completed stack {stackIdentifier}");
        }
    }
}
