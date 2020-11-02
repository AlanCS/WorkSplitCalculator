using Amazon.CDK;
using System.Data;
using CloudConfiguration;

namespace CloudConfiguration
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var props = new StackProps() { Env = GetEnvironment() };

            var baseDomainName = "alancardozosarli.com";

            new WorkSplitCalculatorStack(app, "WorkSplitCalculatorStack", baseDomainName,  props);
            new LandingStack(app, "LandingStack", baseDomainName, props);

            app.Synth();
        }

        static Environment GetEnvironment()
        {
            // recommended by https://docs.aws.amazon.com/cdk/latest/guide/environments.html

            return new Environment
            {
                Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT") ?? throw new NoNullAllowedException(),
                Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION") ?? throw new NoNullAllowedException()
            };
        }
    }
}
