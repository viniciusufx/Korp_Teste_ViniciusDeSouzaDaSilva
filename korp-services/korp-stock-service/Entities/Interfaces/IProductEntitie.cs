namespace Korp.StockService.Entities.Interfaces
{
    /// <summary>
    /// Representa o contrato de um produto utilizado no controle de estoque.
    /// </summary>
    /// <remarks>
    /// Define as propriedades básicas necessárias para uma entidade de produto
    /// no gerenciamento de inventário.
    /// </remarks>
    public interface IProductEntitie
    {
        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o saldo disponível do produto em estoque.
        /// </summary>
        int Saldo { get; set; }
    }
}