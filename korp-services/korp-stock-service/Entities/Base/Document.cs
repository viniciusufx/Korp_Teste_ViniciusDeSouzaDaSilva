using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Korp.StockService.Entities.Base
{
    /// <summary>
    /// Implementação base para documentos do MongoDB.
    /// Fornece Id e data de criação derivada do ObjectId.
    /// </summary>
    public abstract class Document : IDocument
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Document"/>.
        /// </summary>
        protected Document(){}

        /// <inheritdoc/>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        /// <inheritdoc/>
        [BsonIgnore]
        public DateTime CreatedAt => ObjectId.Parse(Id).CreationTime;
    }
}