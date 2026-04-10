using System.ComponentModel.DataAnnotations;

namespace Korp.StockService.DTOs
{
    /// <summary>
    /// Data Transfer Object usado para retornar informações de um produto ao cliente.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Identificador único do produto.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Código do produto.
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade disponível em estoque.
        /// </summary>
        public int Saldo { get; set; }
    }

    /// <summary>
    /// Data Transfer Object usado para criar um novo produto.
    /// </summary>
    public class NewProductDto
    {
        /// <summary>
        /// Código do novo produto.
        /// </summary>
        [Required(ErrorMessage = "O Código é necessário.")]
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do novo produto.
        /// </summary>
        [Required(ErrorMessage = "A Descrição é necessária.")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Saldo inicial do produto em estoque.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "O saldo do produto não pode ser negativo.")]
        public int Saldo { get; set; }
    }

    /// <summary>
    /// Data Transfer Object usado para atualizar um produto existente.
    /// </summary>
    public class UpdateProductDto
    {
        /// <summary>
        /// Código atualizado do produto.
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição atualizada do produto.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Saldo atualizado do produto em estoque.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "O saldo do produto não pode ser negativo.")]
        public int Saldo { get; set; }
    }
}