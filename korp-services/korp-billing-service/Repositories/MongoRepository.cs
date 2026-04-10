using System.Linq.Expressions;
using Korp.BillingService.Entities.Attributes;
using Korp.BillingService.Entities.Base;
using Korp.BillingService.Infrastructure.MongoDb;
using MongoDB.Driver;

namespace Korp.BillingService.Repositories
{
    /// <summary>
    /// Implementação do repositório genérico para MongoDB.
    /// </summary>
    /// <typeparam name="TDocument">
    /// Tipo do documento que herda de <see cref="Document"/>.
    /// </typeparam>
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : Document
    {
        private readonly IMongoCollection<TDocument> _collection;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="MongoRepository{TDocument}"/>.
        /// </summary>
        /// <param name="mongoDbProvider">
        /// Provedor de acesso ao banco MongoDB.
        /// </param>
        public MongoRepository(IMongoDbProvider mongoDbProvider)
        {
            var database = mongoDbProvider.Database;

            _collection = database.GetCollection<TDocument>(
                GetCollectionName(typeof(TDocument))
            );
        }

        /// <summary>
        /// Obtém o nome da coleção a partir do atributo <see cref="BsonCollectionAttribute"/>.
        /// </summary>
        /// <param name="documentType">Tipo do documento.</param>
        /// <returns>Nome da coleção MongoDB.</returns>
        /// <exception cref="InvalidOperationException">
        /// Lançada quando o documento não define o atributo BsonCollectionAttribute.
        /// </exception>
        private protected string GetCollectionName(Type documentType)
        {
            if (
                documentType
                    .GetCustomAttributes(typeof(BsonCollectionAttribute), true)
                        .FirstOrDefault()
                is not BsonCollectionAttribute attribute)
            {
                throw new InvalidOperationException(
                    $"The type {documentType.Name} does not define BsonCollectionAttribute.");
            }

            return attribute.CollectionName;
        }

        /// <inheritdoc/>
        public virtual IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        /// <inheritdoc/>
        public async Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter)
        {
            return await _collection.DeleteOneAsync(filter);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistAsync(Expression<Func<TDocument, bool>> filter)
        {
            return await _collection.Find(filter).AnyAsync();
        }

        /// <inheritdoc/>
        public async Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TDocument>> projection)
        {
            return await _collection.Find(filter).Project(projection).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(int pageNumber, int pageSize)
        {
            return await _collection
                .Find(Builders<TDocument>.Filter.Empty)
                .Sort(Builders<TDocument>.Sort.Descending(d => d.Id))
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy, int pageNumber, int pageSize)
        {
            var sortDefinitions = sortBy
                .Select(s => s.ascending
                    ? Builders<TDocument>.Sort.Ascending(s.keySelector)
                    : Builders<TDocument>.Sort.Descending(s.keySelector));
            
            var combinedSort = Builders<TDocument>.Sort.Combine(sortDefinitions);

            return await _collection
                .Find(Builders<TDocument>.Filter.Empty)
                .Sort(combinedSort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, TDocument>> projection, int pageNumber, int pageSize)
        {
            return await _collection
                .Find(Builders<TDocument>.Filter.Empty)
                .Sort(Builders<TDocument>.Sort.Descending(d => d.Id))
                .Project(projection)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy, Expression<Func<TDocument, TDocument>> projection, int pageNumber, int pageSize)
        {
            var sortDefinitions = sortBy
                .Select(s => s.ascending
                    ? Builders<TDocument>.Sort.Ascending(s.keySelector)
                    : Builders<TDocument>.Sort.Descending(s.keySelector));
            
            var combinedSort = Builders<TDocument>.Sort.Combine(sortDefinitions);

            return await _collection
                .Find(Builders<TDocument>.Filter.Empty)
                .Sort(combinedSort)
                .Project(projection)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter)
        {
            return await _collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(d => d.Id))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy)
        {
            var sortDefinitions = sortBy
                .Select(s => s.ascending
                    ? Builders<TDocument>.Sort.Ascending(s.keySelector)
                    : Builders<TDocument>.Sort.Descending(s.keySelector));
            
            var combinedSort = Builders<TDocument>.Sort.Combine(sortDefinitions);

            return await _collection
                .Find(filter)
                .Sort(combinedSort)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, int pageNumber, int pageSize)
        {
            return await _collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(d => d.Id))
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy, int pageNumber, int pageSize)
        {
            var sortDefinitions = sortBy
                .Select(s => s.ascending
                    ? Builders<TDocument>.Sort.Ascending(s.keySelector)
                    : Builders<TDocument>.Sort.Descending(s.keySelector));
            
            var combinedSort = Builders<TDocument>.Sort.Combine(sortDefinitions);

            return await _collection
                .Find(filter)
                .Sort(combinedSort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TDocument>> projection)
        {
            return await _collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(d => d.Id))
                .Project(projection)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy, Expression<Func<TDocument, TDocument>> projection)
        {
        
            var sortDefinitions = sortBy
                .Select(s => s.ascending
                    ? Builders<TDocument>.Sort.Ascending(s.keySelector)
                    : Builders<TDocument>.Sort.Descending(s.keySelector));
            
            var combinedSort = Builders<TDocument>.Sort.Combine(sortDefinitions);

            return await _collection
                .Find(filter)
                .Sort(combinedSort)
                .Project(projection)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, bool>> filter, 
            Expression<Func<TDocument, TDocument>> projection, 
            int pageNumber, int pageSize
        )
        {
            return await _collection
                .Find(filter)
                .Sort(Builders<TDocument>.Sort.Descending(d => d.Id))
                .Project(projection)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, bool>> filter, 
            IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy, 
            Expression<Func<TDocument, TDocument>> projection, 
            int pageNumber, int pageSize
        )
        {
            var sortDefinitions = sortBy
                .Select(s => s.ascending
                    ? Builders<TDocument>.Sort.Ascending(s.keySelector)
                    : Builders<TDocument>.Sort.Descending(s.keySelector));
            
            var combinedSort = Builders<TDocument>.Sort.Combine(sortDefinitions);

            return await _collection
                .Find(filter)
                .Sort(combinedSort)
                .Project(projection)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task InsertOneAsync(TDocument entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        /// <inheritdoc/>
        public async Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update)
        {
            return await _collection.UpdateOneAsync(filter, update);
        }

        /// <inheritdoc/>
        public async Task<TDocument?> FindOneAndUpdateAsync(
            FilterDefinition<TDocument> filter, 
            UpdateDefinition<TDocument> update, 
            FindOneAndUpdateOptions<TDocument>? options = null
        )
        {
            return await _collection.FindOneAndUpdateAsync(filter, update, options);
        }

        /// <inheritdoc/>
        public async Task<ReplaceOneResult> ReplaceOneAsync(
            Expression<Func<TDocument, bool>> filter,
            TDocument replacement,
            ReplaceOptions? options = null)
        {
            return await _collection.ReplaceOneAsync(filter, replacement, options);
        }

        /// <inheritdoc/>
        public async Task<long> GetNextSequenceValueAsync()
        {
            var countersCollection = _collection
                .Database
                .GetCollection<MongoCounter>("Counters");

            var CollectionName = GetCollectionName(typeof(TDocument));

            var filter = Builders<MongoCounter>.Filter.Eq(x => x.Id, CollectionName);

            var update = Builders<MongoCounter>.Update.Inc(x => x.Value, 1);

            var options = new FindOneAndUpdateOptions<MongoCounter>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var counter = await countersCollection.FindOneAndUpdateAsync(
                filter,
                update,
                options
            );

            return counter!.Value;
        }
    }
}