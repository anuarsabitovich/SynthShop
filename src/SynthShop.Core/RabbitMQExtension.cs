using RabbitMQ.Client;

namespace SynthShop.Core
{
    public static class RabbitMQExtension
    {
        public static ConnectionFactory GetFactory(string host, string userName, string password)
        {
#if DEBUG
            var factory = new ConnectionFactory
            {
                HostName = host,
                UserName = userName,
                Password = password
            };
#else
            var factory = new ConnectionFactory
            {
                Uri = new Uri(host),
                Port = 5671,
                UserName = userName,
                Password = password
            };
#endif
            return factory;
        }
    }
}
