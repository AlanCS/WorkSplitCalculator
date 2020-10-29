using Amazon.CDK;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using System.Linq;
using Amazon.CDK.AWS.Route53.Targets;

namespace WorkSplitCdkStacks.Helpers
{
    // copied and adapted from https://github.com/aws-samples/aws-cdk-examples/blob/master/csharp/static-site/src/StaticSite/StaticSiteConstruct.cs

    public class StaticSiteConstructProps
    {
        public string DomainName;

        public string SiteSubDomain;

        public string WebsiteIndexDocument = "index.html";

        public string WebsiteFilesPath = "";
    }

    public class StaticSiteOnS3WithCloudFront : Construct
    {
        public StaticSiteOnS3WithCloudFront(Construct scope, string id, StaticSiteConstructProps props) : base(scope, id)
        {
            var zone = HostedZone.FromLookup(this, "Zone", new HostedZoneProviderProps
            {
                DomainName = props.DomainName
            });

            var siteDomain = $"{props.SiteSubDomain}.{props.DomainName}";

            var siteBucket = new Bucket(this, "SiteBucket", new BucketProps
            {
                BucketName = siteDomain,
                WebsiteIndexDocument = props.WebsiteIndexDocument,
                WebsiteErrorDocument = "error.html",
                PublicReadAccess = true,
                RemovalPolicy = RemovalPolicy.RETAIN
            });

            var certificate = new DnsValidatedCertificate(this, "SiteCertificate", new DnsValidatedCertificateProps
            {
                DomainName = siteDomain,
                HostedZone = zone,
                Region = "us-east-1" // Cloudfront only checks this region for certificates.
            });

            var distribution = new CloudFrontWebDistribution(this, "SiteDistribution", new CloudFrontWebDistributionProps
            {                
                AliasConfiguration = new AliasConfiguration()
                {
                    Names = new string[] { siteDomain },
                    AcmCertRef = certificate.CertificateArn, 
                    SecurityPolicy = SecurityPolicyProtocol.TLS_V1_2_2019
                },
                //ViewerCertificate = ViewerCertificate.FromAcmCertificate(certificate), // this syntax doesn't seem to be quite ready for use yet
                OriginConfigs = new ISourceConfiguration[]
                {
                    new SourceConfiguration
                    {
                        S3OriginSource = new S3OriginConfig
                        {
                            S3BucketSource = siteBucket
                        },
                        Behaviors = new Behavior[] { new Behavior() { IsDefaultBehavior = true } }
                    }
                }
            });

            new ARecord(this, "SiteAliasRecord", new ARecordProps
            {
                RecordName = siteDomain,
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution)),
                Zone = zone
            });

            new BucketDeployment(this, "DeployWithInvalidation", new BucketDeploymentProps
            {
                Sources = new ISource[] { Source.Asset(props.WebsiteFilesPath) },
                DestinationBucket = siteBucket,
                Distribution = distribution,
                DistributionPaths = new string[] { "/*" }
            });

            Log("Site", $"https://{siteDomain}");

            Log("StaticWebsiteBucket", siteBucket.BucketName);

            void Log(string key, string value)
            {
                new CfnOutput(this, key, new CfnOutputProps
                {
                    Value = value
                });
            }
        }
    }
}
