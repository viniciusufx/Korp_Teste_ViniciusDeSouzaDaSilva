namespace Korp.BillingService.Entities.Implementations
{
    /// <summary>
    /// Representa o status de uma nota fiscal.
    /// </summary>
    /// <remarks>
    /// Define os estados possíveis durante o ciclo de vida
    /// de uma nota fiscal.
    /// </remarks>
    public enum InvoiceStatusEntitie
    {
        /// <summary>
        /// Nota fiscal aberta para edição e inclusão de itens.
        /// </summary>
        Open,

        /// <summary>
        /// Nota fiscal fechada após impressão.
        /// </summary>
        Closed
    }
}