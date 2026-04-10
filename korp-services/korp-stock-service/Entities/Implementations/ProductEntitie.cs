using Korp.StockService.Entities.Attributes;
using Korp.StockService.Entities.Base;
using Korp.StockService.Entities.Interfaces;

namespace Korp.StockService.Entities.Implementations
{
    /// <summary>
    /// Representa um produto armazenado no estoque.
    /// </summary>
    /// <remarks>
    /// Esta entidade é persistida na collection "Products" do MongoDB e
    /// contém as informações básicas para controle de estoque.
    /// </remarks>
    [BsonCollection("Products")]
    public class ProductEntitie : Document, IProductEntitie
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ProductEntitie"/>.
        /// </summary>
        public ProductEntitie()
        {
        }

        /// <inheritdoc/>
        public string Codigo { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Descricao { get; set; } = string.Empty;
        
        /// <inheritdoc/>
        public int Saldo { get; set; }
    }
}