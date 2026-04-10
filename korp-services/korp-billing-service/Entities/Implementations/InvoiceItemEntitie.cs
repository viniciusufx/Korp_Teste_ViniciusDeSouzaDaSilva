using Korp.BillingService.Entities.Interfaces;

namespace Korp.BillingService.Entities.Implementations
{
    /// <summary>
    /// Representa um item de uma nota fiscal.
    /// </summary>
    /// <remarks>
    /// Contém a referência ao produto e a quantidade utilizada
    /// na nota fiscal.
    /// </remarks>
    public class InvoiceItemEntitie : IInvoiceItemEntitie
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="InvoiceItemEntitie"/>.
        /// </summary>
        public InvoiceItemEntitie()
        {
        }

        /// <inheritdoc/>
        public string ProductId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public int Quantity { get; set; }
    }
}