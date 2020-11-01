using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using System;
using WorkSplitCdkStacks.Helpers;

namespace WorkSplitCdkStacks
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var stackIdentifier = $"stack [{StackName}] for region [{Region}] and account [{Account}]";

            Console.WriteLine($"Starting stack {stackIdentifier}");

            // this script suposes the domain been registered manually in AWS

            var baseDomainName = "alancardozosarli.com";
            var apiDomainName = $"api.{baseDomainName}";

            var zone = HostedZone.FromLookup(this, "Zone", new HostedZoneProviderProps
            {
                DomainName = baseDomainName
            });

            var backend = new DotNetLambdaWithApiGetway(this, "Backend", new DotNetLambdaWithApiGetwayProps
            {
                Domain = apiDomainName,
                Code = Code.FromAsset("temp/Web"),
                Certificate = CreateCertificate(true),
                Zone = zone
            });


            var frontEnd = new StaticSiteOnS3WithCloudFront(this, "MyStaticSite", new StaticSiteConstructProps
            {
                DomainName = baseDomainName,
                SiteSubDomain = "www",
                WebsiteFilesPath = "temp/Client/wwwroot",
                CertificateArn = CreateCertificate(false).CertificateArn
            });            

            DnsValidatedCertificate CreateCertificate(bool isSubDomain)
            {
                var certificate = new DnsValidatedCertificate(this, "SiteCertificate_" + (isSubDomain ? "subDomain" : "mainDomain"), new DnsValidatedCertificateProps
                {
                    DomainName = isSubDomain ? $"*.{baseDomainName}" : $"www.{baseDomainName}",
                    HostedZone = zone,
                    Region = "us-east-1" // Cloudfront only checks this region for certificates.
                });

                return certificate;
            }

            Console.WriteLine($"Completed stack {stackIdentifier}");
        }
    }
}
