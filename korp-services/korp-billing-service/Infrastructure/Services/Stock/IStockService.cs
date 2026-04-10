namespace Korp.BillingService.Infrastructure.Services.Stock
{
    /// <summary>
    /// Define operações de comunicação com o serviço de estoque.
    /// </summary>
    /// <remarks>
    /// Esta interface abstrai as chamadas ao microserviço de estoque,
    /// permitindo realizar operações relacionadas ao saldo de produtos
    /// a partir do serviço de faturamento.
    /// </remarks>
    public interface IStockService
    {
        /// <summary>
        /// Realiza a baixa de saldo de um produto no serviço de estoque.
        /// </summary>
        /// <param name="productId">Identificador único do produto.</param>
        /// <param name="quantidade">Quantidade a ser baixada do saldo.</param>
        /// <returns>
        /// Retorna o saldo restante do produto após a baixa,
        /// ou <c>null</c> caso a operação não seja concluída com sucesso.
        /// </returns>
        /// <remarks>
        /// Esta operação chama o endpoint do microserviço de estoque responsável
        /// por decrementar o saldo do produto de forma atômica.
        /// </remarks>
        public Task<int?> BaixarSaldoProdutoAsync(string productId, int quantidade);
    }
}