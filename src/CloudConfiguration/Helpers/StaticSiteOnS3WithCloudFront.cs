using Amazon.CDK;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Deployment;

namespace CloudConfiguration.Helpers
{
    // copied and adapted from https://github.com/aws-samples/aws-cdk-examples/blob/master/csharp/static-site/src/StaticSite/StaticSiteConstruct.cs

    public class StaticSiteConstructProps
    {
        public string DomainName;

        public string SiteSubDomain = "www";

        public string WebsiteIndexDocument = "index.html";

        public string WebsiteFilesPath;

        public string CertificateArn;

        public IHostedZone Zone;
    }

    public class StaticSiteOnS3WithCloudFront : Construct
    {
        public StaticSiteOnS3WithCloudFront(Stack scope, string id, StaticSiteConstructProps props) : base(scope, id)
        {
            var fullDomain = $"{props.SiteSubDomain}.{props.DomainName}";

            var siteBucket = new Bucket(this, $"SiteBucket_{id}", new BucketProps
            {
                BucketName = $"static-content-{fullDomain}",
                WebsiteIndexDocument = props.WebsiteIndexDocument,
                WebsiteErrorDocument = "error.html",
                PublicReadAccess = true,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            var distribution = new CloudFrontWebDistribution(this, $"SiteDistribution_{id}", new CloudFrontWebDistributionProps
            {                
                AliasConfiguration = new AliasConfiguration()
                {
                    Names = new string[] { fullDomain },
                    AcmCertRef = props.CertificateArn, 
                    SecurityPolicy = SecurityPolicyProtocol.TLS_V1_2_2019
                },
                //ViewerCertificate = ViewerCertificate.FromAcmCertificate(certificate), // this syntax doesn't seem to be quite ready for use yet
                OriginConfigs = new ISourceConfiguration[]
                {
                    new SourceConfiguration
                    {
                        S3OriginSource = new S3OriginConfig { S3BucketSource = siteBucket },
                        Behaviors = new Behavior[] { new Behavior() { IsDefaultBehavior = true } }
                    }
                }
            });

            new ARecord(this, $"SiteAliasRecord_{id}", new ARecordProps
            {
                RecordName = fullDomain,
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution)),
                Zone = props.Zone
            });

            new BucketDeployment(this, $"DeployWithInvalidation_{id}", new BucketDeploymentProps
            {
                Sources = new ISource[] { Source.Asset(props.WebsiteFilesPath) },
                DestinationBucket = siteBucket,
                Distribution = distribution,
                DistributionPaths = new string[] { "/*" }
            });

            scope.Log($"CloudFrontSite_{props.DomainName}", $"https://{fullDomain}");

            scope.Log($"StaticWebsiteBucket_{props.DomainName}", siteBucket.BucketName);
        }
    }
}
