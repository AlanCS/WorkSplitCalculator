using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;

namespace CloudConfiguration.Helpers
{

    public class DotNetLambdaWithApiGetwayProps
    {
        public Code Code;

        public string DomainName;

        public string SiteSubDomain;

        public ICertificate Certificate;

        public IHostedZone Zone;
    }

    public class DotNetLambdaWithApiGetway : Construct
    {
        public DotNetLambdaWithApiGetway(Stack scope, string id, DotNetLambdaWithApiGetwayProps props) : base(scope, id)
        {
            // domain and certificate have been created manually on AWS Console for security purposes

            var dotnetWebApiLambda = new Function(this, "WebLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = props.Code,
                Handler = "Web::Web.LambdaEntryPoint::FunctionHandlerAsync"
            });

            var apiGetway = new LambdaRestApi(this, "ApiGetwayLambda", new LambdaRestApiProps()
            {
                Handler = dotnetWebApiLambda
            });

            var fullDomain = $"{props.SiteSubDomain}.{props.DomainName}";

            var apiDomain = apiGetway.AddDomainName(fullDomain, new DomainNameOptions()
            {
                DomainName = fullDomain,
                Certificate = props.Certificate,
                EndpointType = EndpointType.EDGE
            });

            new CnameRecord(this, "ApiGatewayRecordSet", new CnameRecordProps()
            {
                Zone = props.Zone,
                RecordName = "api",
                DomainName = apiDomain.DomainNameAliasDomainName
            });

            scope.Log($"ApiGateway Url {id}", apiGetway.Url);
            scope.Log($"ApiGateway public domain {id}", apiDomain.DomainNameAliasDomainName);
        }
    }
}
