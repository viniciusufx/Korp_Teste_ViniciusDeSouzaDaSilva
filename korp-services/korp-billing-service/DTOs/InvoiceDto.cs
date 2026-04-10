using System.ComponentModel.DataAnnotations;

namespace Korp.BillingService.DTO
{
    /// <summary>
    /// DTO utilizado para criação de uma nova nota fiscal.
    /// Contém a lista de itens que compõem a nota.
    /// </summary>
    public class NewInvoiceDto
    {
        /// <summary>
        /// Lista de itens da nota fiscal.
        /// Cada item representa um produto e sua respectiva quantidade.
        /// </summary>
        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// Representa o resultado paginado de uma nota fiscal.
    /// </summary>
    /// <remarks>
    /// Este DTO é utilizado para retornar informações resumidas de notas fiscais
    /// em consultas paginadas.
    /// 
    /// Contém os principais dados da nota fiscal, incluindo identificador,
    /// número sequencial, itens associados e status atual.
    /// </remarks>
    public class InvoicePagedResultDto
    {
        /// <summary>
        /// Identificador único da nota fiscal.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Número sequencial da nota fiscal.
        /// </summary>
        public long Number { get; set; }
        
        /// <summary>
        /// Lista de itens associados à nota fiscal.
        /// </summary>
        public List<InvoiceItemDto> Items { get; set; } = new();
        
        /// <summary>
        /// Status atual da nota fiscal.
        /// </summary>
        public InvoiceStatusDto Status { get; set; }
    }

    /// <summary>
    /// DTO utilizado para atualização de uma nota fiscal.
    /// </summary>
    /// <remarks>
    /// Este objeto contém os dados que podem ser modificados em uma nota fiscal existente,
    /// incluindo o número, a lista de itens e o status.
    /// 
    /// É utilizado nos endpoints de atualização (PATCH).
    /// </remarks>
    public class UpdateInvoiceDto
    {
        /// <summary>
        /// Número sequencial da nota fiscal.
        /// </summary>
        public long Number { get; set; }

        /// <summary>
        /// Lista de itens da nota fiscal com produto e quantidade.
        /// </summary>
        public List<InvoiceItemDto> Items { get; set; } = new();

        /// <summary>
        /// Status da nota fiscal.
        /// </summary>
        public InvoiceStatusDto Status { get; set; }
    }

    /// <summary>
    /// DTO que representa um item dentro da criação de uma nova nota fiscal.
    /// </summary>
    public class InvoiceItemDto
    {
        /// <summary>
        /// Identificador do produto.
        /// Deve conter exatamente 24 caracteres no formato ObjectId do MongoDB.
        /// </summary>
        [Required(ErrorMessage = "O campo 'ProductId' é obrigatório e não pode estar vazio.")]
        [Length(24, 24, ErrorMessage = "O campo 'ProductId' deve conter exatamente 24 caracteres.")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade do produto incluído na nota fiscal.
        /// Não pode ser negativa.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade do produto não pode ser negativa.")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Representa os possíveis status de uma nota fiscal.
    /// </summary>
    /// <remarks>
    /// Define o estado atual da nota fiscal no fluxo do sistema.
    /// Uma nota pode estar aberta para edição ou fechada após processamento.
    /// </remarks>
    public enum InvoiceStatusDto
    {
        /// <summary>
        /// Nota fiscal aberta. Permite alterações e inclusão de itens.
        /// </summary>
        Open,

        /// <summary>
        /// Nota fiscal fechada. Não permite mais alterações.
        /// </summary>
        Closed
    }

    /// <summary>
    /// DTO de resposta da operação de baixa de saldo de um produto.
    /// </summary>
    /// <remarks>
    /// Este objeto é retornado após a execução da operação de baixa de estoque,
    /// contendo o identificador do produto e o saldo restante após a atualização.
    /// </remarks>
    public class BaixarSaldoResponseDto
    {
        /// <summary>
        /// Identificador único do produto.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Saldo restante do produto após a baixa.
        /// </summary>
        public int Saldo { get; set; }
    }
}