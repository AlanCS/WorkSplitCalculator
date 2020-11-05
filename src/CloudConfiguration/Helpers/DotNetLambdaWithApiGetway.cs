using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Amazon.CDK.AWS.SQS;

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

            var snsTopic = new Topic(this, "WorkSplitRequest", new TopicProps()
            {
                TopicName = "work-split-request",
                DisplayName = "Work Split Request"
            });

            snsTopic.AddSubscription(new EmailSubscription("alanzis@gmail.com", new EmailSubscriptionProps()));

            var queue = new Queue(this, "QueueProcessor", new QueueProps() { RetentionPeriod = Duration.Days(2) });

            snsTopic.AddSubscription(new SqsSubscription(queue));

            var dotnetWebApiLambda = new Function(this, "WebLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = props.Code,
                Handler = "Web::Web.LambdaEntryPoint::FunctionHandlerAsync"
            });
            dotnetWebApiLambda.AddEnvironment("Region", scope.Region);
            dotnetWebApiLambda.AddEnvironment("SnsTopic", snsTopic.TopicArn);
            snsTopic.GrantPublish(dotnetWebApiLambda);

            var fullDomain = $"{props.SiteSubDomain}.{props.DomainName}";

            var apiGetway = new LambdaRestApi(this, fullDomain, new LambdaRestApiProps()
            {
                Handler = dotnetWebApiLambda
            });            

            var apiDomain = apiGetway.AddDomainName("customDomain", new DomainNameOptions()
            {
                DomainName = fullDomain,
                Certificate = props.Certificate,
                SecurityPolicy = SecurityPolicy.TLS_1_2,
                EndpointType = EndpointType.EDGE
            });

            new ARecord(this, "ApiGateway-ARecord", new ARecordProps() {
                Zone = props.Zone,
                RecordName = fullDomain,
                Target = RecordTarget.FromAlias(new ApiGateway(apiGetway)),
            });


            scope.Log($"ApiGateway Url {id}", apiGetway.Url);
            scope.Log($"ApiGateway public domain {id}", apiDomain.DomainNameAliasDomainName);
        }
    }
}
