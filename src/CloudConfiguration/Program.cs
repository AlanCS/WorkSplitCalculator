using Amazon.CDK;
using System.Data;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Route53;
using CloudConfiguration.DTO;
using System.Runtime.CompilerServices;

namespace CloudConfiguration
{
    sealed class Program
    {
        static string baseDomainName = "alancardozosarli.com";

        public static void Main(string[] args)
        {
            var app = new App();

            var props = new StackProps() { Env = GetEnvironment() };

            // weird syntax, but a good way to grab Zone / Certificate from one place, otherwise
            // you get errors like
            // "Failed to create resource. Error: you have reached your limit of 20 certificates in the last year"

            var networkStack = new Stack(app, "NetworkStack", props);

            var config = GetNetworkConfig(networkStack);

            new LandingStack(app, "LandingStack", baseDomainName, config, props);

            new WorkSplitCalculatorStack(app, "WorkSplitCalculatorStack", baseDomainName, config, props);            

            app.Synth();
        }

        private static NetworkConfig GetNetworkConfig(Stack stack)
        {
            var result = new NetworkConfig();

            result.Zone = HostedZone.FromLookup(stack, "Zone", new HostedZoneProviderProps { DomainName = baseDomainName });

            result.Certificate = Certificate.FromCertificateArn(stack, "Certificate", "arn:aws:acm:us-east-1:249221827206:certificate/72c985a5-1576-4348-954b-34e4161d05f5");

            //result.Certificate = new DnsValidatedCertificate(stack, "SiteCertificate", new DnsValidatedCertificateProps
            //{
            //    DomainName = $"*.{baseDomainName}",
            //    HostedZone = result.Zone,
            //    Region = "us-east-1" // Cloudfront only checks this region for certificates.
            //});

            return result;
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
