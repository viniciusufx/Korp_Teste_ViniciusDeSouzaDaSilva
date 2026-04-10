using MongoDB.Driver;

namespace Korp.BillingService.Infrastructure.MongoDb
{
    /// <summary>
    /// Define o provedor de acesso ao banco de dados MongoDB.
    /// Responsável por disponibilizar a instância da base configurada.
    /// </summary>
    public interface IMongoDbProvider
    {
        /// <summary>
        /// Instância do banco de dados MongoDB configurado.
        /// </summary>
        IMongoDatabase Database { get; }
    }
}