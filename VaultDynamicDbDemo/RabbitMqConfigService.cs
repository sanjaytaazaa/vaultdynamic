using System.Text.Json;

namespace VaultDynamicDbDemo
{
    public class RabbitMqConfigService
    {
        private const string SecretFilePath = "/vault/secrets/db-secrets.json";
        public (string User, string Pass, string Host) GetCredentials()
        {
            var json = File.ReadAllText(SecretFilePath);
            var secret = JsonSerializer.Deserialize<Cred>(json);
            return (secret.rabbit_username, secret.rabbit_password, "rabbitmq-shared.rabbitmq-shared.svc.cluster.local");
        }
        private class Cred
        {
            public string rabbit_username { get; set; }
            public string rabbit_password { get; set; }
        }
    }
}
