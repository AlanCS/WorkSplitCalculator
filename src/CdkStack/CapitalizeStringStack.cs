using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Lambda;

namespace CapitalizeString
{
    public class CapitalizeStringStack : Stack
    {
        internal CapitalizeStringStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            Function fn = new Function(this, "capitalizestring", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("../../../../CapitalizeStringHandler/bin/Release/netcoreapp3.1/publish"),
                Handler = "CapitalizeStringHandler::CapitalizeStringHandler.Function::FunctionHandler"
            });

            new Amazon.CDK.AWS.APIGateway.LambdaRestApi(this, "lambdaRest", new LambdaRestApiProps()
            {
                Handler = fn
            });
        }
    }
}
