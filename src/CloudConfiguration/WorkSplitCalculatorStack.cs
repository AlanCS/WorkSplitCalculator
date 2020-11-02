using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using System;
using CloudConfiguration.Helpers;
using System.Collections.Generic;

namespace CloudConfiguration
{
    public class WorkSplitCalculatorStack : Stack
    {
        static Dictionary<bool, DnsValidatedCertificate>  certsCache = new Dictionary<bool, DnsValidatedCertificate>();

        internal WorkSplitCalculatorStack(Construct scope, string id, string baseDomainName, IStackProps props = null) : base(scope, id, props)
        {
            // this script suposes the domain been registered manually in AWS

            var zone = HostedZone.FromLookup(this, "Zone", new HostedZoneProviderProps { DomainName = baseDomainName });

            var WorkSplitCalculatorDomain = $"worksplitcalculator.{baseDomainName}";

            //var WorkSplitCalculatorBackend = new DotNetLambdaWithApiGetway(this, "WorkSplitCalculatorBackend", new DotNetLambdaWithApiGetwayProps
            //{
            //    Domain = $"{WorkSplitCalculatorDomain}",
            //    Code = Code.FromAsset("published/WorkSplitCalculator/Web"),
            //    Certificate = CreateCertificate(true),
            //    Zone = zone
            //});

            var WorkSplitCalculatorFront = new StaticSiteOnS3WithCloudFront(this, "WorkSplitCalculatorFront", new StaticSiteConstructProps
            {
                DomainName = WorkSplitCalculatorDomain,
                WebsiteFilesPath = "published/WorkSplitCalculator/Client/wwwroot",
                CertificateArn = CreateCertificate(false).CertificateArn,
                Zone = zone
            });            

            DnsValidatedCertificate CreateCertificate(bool isSubDomain)
            {
                if (certsCache.ContainsKey(isSubDomain))
                    return certsCache[isSubDomain];

                var certificate = new DnsValidatedCertificate(this, "SiteCertificate_" + (isSubDomain ? "subDomain" : "mainDomain"), new DnsValidatedCertificateProps
                {
                    
                    DomainName = isSubDomain ? $"*.{baseDomainName}" : $"www.{baseDomainName}",
                    HostedZone = zone,
                    Region = "us-east-1" // Cloudfront only checks this region for certificates.
                });

                certsCache[isSubDomain] = certificate;

                return certificate;
            }

            var stackIdentifier = $"stack [{StackName}] for region [{Region}] and account [{Account}]";
            Console.WriteLine($"Completed stack {stackIdentifier}");
        }
    }
}
