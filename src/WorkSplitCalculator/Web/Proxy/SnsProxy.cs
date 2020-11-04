using Amazon;
using Amazon.SimpleNotificationService;
using Infrastructure;
using System.Threading.Tasks;

namespace Web.Proxy
{
    public interface ISnsProxy
    {
        Task Publish(string message);
    }

    public class SnsProxy : ISnsProxy
    {
        private readonly IAmazonSimpleNotificationService snsClient;
        private readonly string snsTopicArn;

        public SnsProxy(string envVariableTopicArn, IEnviromentVariableProxy enviromentVariableProxy = null, IAmazonSimpleNotificationService snsClient = null)
        {
            enviromentVariableProxy = enviromentVariableProxy ?? new EnviromentVariableProxy();
            var region = RegionEndpoint.GetBySystemName(enviromentVariableProxy.get("Region"));
            this.snsClient = snsClient ?? new AmazonSimpleNotificationServiceClient(region);
            snsTopicArn = enviromentVariableProxy.get(envVariableTopicArn);
        }

        public Task Publish(string message)
        {
            return this.snsClient.PublishAsync(snsTopicArn, message);
        }
    }
}
