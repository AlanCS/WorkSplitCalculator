using Amazon.CDK;

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
    }
}
