namespace Korp.BillingService.Entities.Interfaces
{
    /// <summary>
    /// Representa o contrato de um item de nota fiscal.
    /// </summary>
    /// <remarks>
    /// Define as propriedades necessárias para relacionar
    /// um produto à quantidade utilizada na nota fiscal.
    /// </remarks>
    public interface IInvoiceItemEntitie
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        string ProductId { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do produto na nota fiscal.
        /// </summary>
        int Quantity { get; set; }
    }
}