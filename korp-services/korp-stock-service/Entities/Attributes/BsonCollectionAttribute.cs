namespace Korp.StockService.Entities.Attributes
{
    /// <summary>
    /// Atributo utilizado para definir explicitamente o nome da collection
    /// do MongoDB associada a uma entidade.
    /// </summary>
    /// <remarks>
    /// Este atributo é aplicado em classes de entidade para mapear
    /// o nome da collection no MongoDB de forma explícita, evitando
    /// a necessidade de hardcode em repositórios ou contextos.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAttribute : Attribute
    {
        /// <summary>
        /// Obtém o nome da collection no MongoDB associada à entidade.
        /// </summary>
        public string CollectionName { get; }

        /// <summary>
        /// Inicializa uma nova instância do atributo <see cref="BsonCollectionAttribute"/>.
        /// </summary>
        /// <param name="collectionName">
        /// Nome da collection no MongoDB que será associada à entidade.
        /// </param>
        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}