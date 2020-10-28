using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Lambda;
using CdkStack.Helpers;
using System;

namespace CdkStack
{
    public class InfrastructureStack : Stack
    {
        internal InfrastructureStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Console.WriteLine($"Creating stack for region [{this.Region}] and account [{this.Account}]");

            var dotnetWebApiLambda = new Function(this, "WebLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("temp/Web"),
                Handler = "Web::Web.LambdaEntryPoint::FunctionHandlerAsync"
            });

            var apiGetway = new LambdaRestApi(this, "ApiGetwayLambda", new LambdaRestApiProps()
            {
                Handler = dotnetWebApiLambda
            });

            var frontEnd = new StaticSiteOnS3WithCloudFront(this, "MyStaticSite", new StaticSiteConstructProps
            {
                DomainName = "alancardozosarli.com",
                SiteSubDomain = "www",
                WebsiteFilesPath = "temp/Client/wwwroot"
            });

        }
    }
}
