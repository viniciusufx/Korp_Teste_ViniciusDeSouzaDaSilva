namespace Korp.StockService.Infrastructure.SecuritySettings
{
    /// <summary>
    /// Define as configurações de segurança da aplicação.
    /// </summary>
    public interface ISecuritySettings
    {
        /// <summary>
        /// Configurações de acesso ao MongoDB.
        /// </summary>
        IMongoDbSettings MongoDbSettings { get; set; }
    }

    /// <summary>
    /// Define as configurações necessárias para conexão com o MongoDB.
    /// </summary>
    public interface IMongoDbSettings
    {
        /// <summary>
        /// String de conexão utilizada para acessar o MongoDB.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Nome do banco de dados MongoDB.
        /// </summary>
        string DatabaseName { get; set; }
    }
}