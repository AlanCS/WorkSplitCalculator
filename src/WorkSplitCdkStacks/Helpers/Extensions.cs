using Amazon.CDK;

namespace WorkSplitCdkStacks.Helpers
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
    }
}
