using Amazon.CDK;
using CloudConfiguration.Helpers;

namespace CloudConfiguration
{
    public class LandingStack : Stack
    {
        internal LandingStack(Construct scope, string id, string baseDomainName, DTO.NetworkConfig config, IStackProps props = null) : base(scope, id, props)
        {
            var landingPage = new StaticSiteOnS3WithCloudFront(this, "LandingPage", new StaticSiteConstructProps
            {
                DomainName = baseDomainName,
                WebsiteFilesPath = "published/LandingWebsite",
                CertificateArn = config.Certificate.CertificateArn,
                Zone = config.Zone
            });

            this.LogCompletion();
        }
    }
}
