using Amazon.CDK;
using System.Data;
using WorkSplitCdkStacks;

namespace WorkSplitCdkStacks
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            
            new InfrastructureStack(app, "InfrastructureStack", new StackProps() { Env = GetEnvironment() } );
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
