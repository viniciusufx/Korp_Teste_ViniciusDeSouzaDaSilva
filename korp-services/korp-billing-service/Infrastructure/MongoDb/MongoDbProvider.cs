using Korp.BillingService.Infrastructure.SecuritySettings;
using MongoDB.Driver;

namespace Korp.BillingService.Infrastructure.MongoDb
{
    /// <summary>
    /// Implementação do provedor de acesso ao MongoDB.
    /// Cria o cliente e retorna a instância do banco configurado.
    /// </summary>
    public class MongoDbProvider : IMongoDbProvider
    {
        /// <inheritdoc/>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="MongoDbProvider"/>.
        /// </summary>
        /// <param name="settings">
        /// Configurações de segurança contendo os dados de conexão com o MongoDB.
        /// </param>
        public MongoDbProvider(ISecuritySettings settings)
        {
            var client = new MongoClient(settings.MongoDbSettings.ConnectionString);

            Database = client.GetDatabase(settings.MongoDbSettings.DatabaseName);
        }
    }
}