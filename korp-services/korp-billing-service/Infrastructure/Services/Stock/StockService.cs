using Korp.BillingService.DTO;

namespace Korp.BillingService.Infrastructure.Services.Stock
{
    /// <summary>
    /// Implementação do serviço responsável pela comunicação com o microserviço de estoque.
    /// </summary>
    /// <remarks>
    /// Esta classe realiza chamadas HTTP para o serviço de estoque,
    /// permitindo operações como a baixa de saldo de produtos durante o faturamento.
    /// </remarks>
    public class StockService : IStockService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpClient">Http client.</param>
        public StockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<int?> BaixarSaldoProdutoAsync(string productId, int quantidade)
        {
            var response = await _httpClient.PatchAsync(
                $"/api/products/{productId}/baixar?quantidade={quantidade}",
                null
            );

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<BaixarSaldoResponseDto>();

            return result?.Saldo;
        }
    }
}