using Korp.BillingService.Entities.Implementations;

namespace Korp.BillingService.Entities.Interfaces
{
    /// <summary>
    /// Representa o contrato de uma nota fiscal.
    /// </summary>
    /// <remarks>
    /// Define as propriedades necessárias para controle de emissão
    /// e gerenciamento de itens de uma nota fiscal.
    /// </remarks>
    public interface IInvoiceEntitie
    {
        /// <summary>
        /// Obtém ou define o número sequencial da nota fiscal.
        /// </summary>
        long Number { get; set; }

        /// <summary>
        /// Obtém ou define o status atual da nota fiscal.
        /// </summary>
        InvoiceStatusEntitie Status { get; set; }

        /// <summary>
        /// Obtém ou define a lista de itens da nota fiscal.
        /// </summary>
        /// <remarks>
        /// A lista é inicializada vazia para permitir a criação de notas fiscais sem itens inicialmente, evitando referências nulas.
        /// </remarks>
        List<InvoiceItemEntitie> Items { get; set; }
    }
}