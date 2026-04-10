namespace Korp.BillingService.Entities.Base
{
    /// <summary>
    /// Define a estrutura base para documentos armazenados no banco de dados.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Identificador único do documento.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Data de criação do documento baseada no ObjectId do MongoDB. Este campo não é persistido no banco.
        /// </summary>
        DateTime CreatedAt { get; }
    }
}