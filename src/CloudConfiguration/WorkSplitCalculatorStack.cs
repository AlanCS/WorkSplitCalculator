using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using CloudConfiguration.Helpers;
using System.Collections.Generic;

namespace CloudConfiguration
{
    public class WorkSplitCalculatorStack : Stack
    {
        internal WorkSplitCalculatorStack(Construct scope, string id, string baseDomainName, DTO.NetworkConfig config, IStackProps props = null) : base(scope, id, props)
        {
            var WorkSplitCalculatorBackend = new DotNetLambdaWithApiGetway(this, "WorkSplitCalculatorBackend", new DotNetLambdaWithApiGetwayProps
            {
                SiteSubDomain = "worksplitcalculatorapi",
                DomainName = baseDomainName,
                Code = Code.FromAsset("published/WorkSplitCalculator/Web"),
                Certificate = config.Certificate,
                Zone = config.Zone
            });

            var WorkSplitCalculatorFront = new StaticSiteOnS3WithCloudFront(this, "WorkSplitCalculatorFront", new StaticSiteConstructProps
            {
                SiteSubDomain = "worksplitcalculator",
                DomainName = baseDomainName,
                WebsiteFilesPath = "published/WorkSplitCalculator/Client/wwwroot",
                CertificateArn = config.Certificate.CertificateArn,
                Zone = config.Zone
            });

            this.LogCompletion();
        }
    }
}
