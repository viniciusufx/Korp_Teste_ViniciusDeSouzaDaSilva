namespace Korp.StockService.Infrastructure.SecuritySettings
{
    /// <summary>
    /// Implementação das configurações de segurança da aplicação.
    /// </summary>
    public class SecuritySettings : ISecuritySettings
    {
        /// <inheritdoc/>
        public IMongoDbSettings MongoDbSettings { get; set; } = new MongoDbSettings();
    }

    /// <summary>
    /// Implementação das configurações de conexão com o MongoDB.
    /// </summary>
    public class MongoDbSettings : IMongoDbSettings
    {
        /// <inheritdoc/>
        public string ConnectionString { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string DatabaseName { get; set; } = string.Empty;
    }
}