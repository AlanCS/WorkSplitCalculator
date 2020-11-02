using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Route53;

namespace CloudConfiguration.DTO
{
    public class NetworkConfig
    {
        public IHostedZone Zone;

        public ICertificate Certificate;
    }
}
