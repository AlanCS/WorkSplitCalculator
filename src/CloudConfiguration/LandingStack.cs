using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using System;
using CloudConfiguration.Helpers;
using System.Collections.Generic;

namespace CloudConfiguration
{
    public class LandingStack : Stack
    {
        static Dictionary<bool, DnsValidatedCertificate>  certsCache = new Dictionary<bool, DnsValidatedCertificate>();

        internal LandingStack(Construct scope, string id, string baseDomainName, IStackProps props = null) : base(scope, id, props)
        {
            var zone = HostedZone.FromLookup(this, "Zone", new HostedZoneProviderProps { DomainName = baseDomainName });

            var landingPage = new StaticSiteOnS3WithCloudFront(this, "LandingPage", new StaticSiteConstructProps
            {
                DomainName = baseDomainName,
                WebsiteFilesPath = "published/LandingWebsite",
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
