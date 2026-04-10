using Korp.BillingService.Entities.Attributes;
using Korp.BillingService.Entities.Base;
using Korp.BillingService.Entities.Interfaces;

namespace Korp.BillingService.Entities.Implementations
{
    /// <summary>
    /// Representa uma nota fiscal.
    /// </summary>
    /// <remarks>
    /// Esta entidade é persistida na collection "Invoices" do MongoDB
    /// e contém os dados necessários para controle de faturamento.
    /// </remarks>
    [BsonCollection("Invoices")]
    public class InvoiceEntitie : Document, IInvoiceEntitie
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="InvoiceEntitie"/>.
        /// </summary>
        public InvoiceEntitie()
        {
        }

        /// <inheritdoc/>
        public long Number { get; set; }

        /// <inheritdoc/>
        public InvoiceStatusEntitie Status { get; set; }

        /// <inheritdoc/>
        public List<InvoiceItemEntitie> Items { get; set; } = new();
    }
}