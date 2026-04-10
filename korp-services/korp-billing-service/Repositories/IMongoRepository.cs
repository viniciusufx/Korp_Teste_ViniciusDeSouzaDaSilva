using System.Linq.Expressions;
using Korp.BillingService.Entities.Base;
using MongoDB.Driver;

namespace Korp.BillingService.Repositories
{
    /// <summary>
    /// Define um repositório genérico para acesso a documentos no MongoDB.
    /// </summary>
    /// <typeparam name="TDocument">
    /// Tipo do documento que herda de <see cref="Document"/>.
    /// </typeparam>
    public interface IMongoRepository<TDocument> where TDocument : Document
    {
        /// <summary>
        /// Retorna a coleção como um <see cref="IQueryable{TDocument}"/> permitindo a construção de consultas LINQ.
        /// </summary>
        /// <remarks>
        /// Este método expõe o provedor LINQ do MongoDB, permitindo consultas fortemente tipadas
        /// utilizando operadores padrão como <c>Where</c>, <c>OrderBy</c>, <c>Skip</c> e <c>Take</c>.
        ///
        /// A consulta utiliza execução adiada (deferred execution) e só é enviada ao banco de dados
        /// quando é enumerada (por exemplo, através de métodos como <c>ToListAsync()</c>,
        /// <c>FirstOrDefaultAsync()</c>, etc.).
        ///
        /// ⚠️ Nem todas as expressões LINQ são suportadas pelo MongoDB. Expressões não suportadas
        /// podem gerar exceções em tempo de execução ou serem avaliadas no lado do cliente.
        ///
        /// ⚠️ Tenha cuidado ao encadear operações que possam impactar a performance, como
        /// valores altos de <c>Skip</c> ou projeções complexas.
        ///
        /// <para><b>Exemplo: Filtro</b></para>
        /// <code>
        /// var usuarios = await repository
        ///     .AsQueryable()
        ///     .Where(u => u.EmailVerified)
        ///     .ToListAsync();
        /// </code>
        ///
        /// <para><b>Exemplo: Ordenação e paginação</b></para>
        /// <code>
        /// var usuarios = await repository
        ///     .AsQueryable()
        ///     .OrderBy(u => u.Username)
        ///     .Skip(10)
        ///     .Take(10)
        ///     .ToListAsync();
        /// </code>
        ///
        /// <para><b>Exemplo: Projeção</b></para>
        /// <code>
        /// var usuarios = await repository
        ///     .AsQueryable()
        ///     .Where(u => u.IsActive)
        ///     .Select(u => new { u.Id, u.Username })
        ///     .ToListAsync();
        /// </code>
        /// </remarks>
        /// <returns>
        /// Um <see cref="IQueryable{TDocument}"/> que pode ser usado para compor consultas LINQ
        /// e executado contra a coleção MongoDB.
        /// </returns>
        IQueryable<TDocument> AsQueryable();

        /// <summary>
        /// Remove um único documento da coleção que corresponda ao filtro especificado.
        /// </summary>
        /// <remarks>
        /// Este método aplica a exclusão ao primeiro documento que corresponder à expressão fornecida.
        /// 
        /// ⚠️ A operação é assíncrona e deve ser aguardada com <c>await</c> para garantir que a exclusão seja concluída.
        ///
        /// O retorno é do tipo <see cref="DeleteResult"/>, que contém informações detalhadas
        /// sobre a operação, como o número de documentos removidos.
        ///
        /// <para><b>Exemplo:</b></para>
        /// <code>
        /// var result = await repository.DeleteOneAsync(p =&gt; p.Id == "64f3b2c2f3d4b12a1c9e1234");
        /// if (result.DeletedCount &gt; 0)
        /// {
        ///     Console.WriteLine("Documento excluído com sucesso.");
        /// }
        /// else
        /// {
        ///     Console.WriteLine("Nenhum documento encontrado para exclusão.");
        /// }
        /// </code>
        /// </remarks>
        /// <param name="filter">Expressão LINQ que define qual documento será removido.</param>
        /// <returns>
        /// Um <see cref="Task{DeleteResult}"/> representando a operação assíncrona,
        /// com informações sobre quantos documentos foram deletados.
        /// </returns>
        Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Verifica se existe algum documento que satisfaça o filtro especificado.
        /// </summary>
        /// <remarks>
        /// Este método retorna <c>true</c> se pelo menos um documento corresponder à expressão fornecida,
        /// caso contrário retorna <c>false</c>.
        ///
        /// ⚠️ A operação é assíncrona e deve ser aguardada com <c>await</c> para garantir a execução correta.
        ///
        /// <para><b>Exemplo:</b></para>
        /// <code>
        /// bool existe = await repository.ExistAsync(u =&gt; u.Email == "teste@exemplo.com");
        /// if (existe)
        /// {
        ///     Console.WriteLine("Usuário já existe.");
        /// }
        /// else
        /// {
        ///     Console.WriteLine("Usuário não encontrado.");
        /// }
        /// </code>
        /// </remarks>
        /// <param name="filter">Expressão LINQ que define o critério de busca.</param>
        /// <returns>
        /// Um <see cref="Task{Boolean}"/> indicando se algum documento satisfaz o filtro.
        /// </returns>
        Task<bool> ExistAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Busca um único documento que satisfaça o filtro especificado.
        /// </summary>
        /// <remarks>
        /// Este método utiliza o provedor do MongoDB para retornar apenas o primeiro documento
        /// que corresponda à expressão fornecida.
        ///
        /// ⚠️ Se nenhum documento corresponder, o método retornará <c>null</c>.
        ///
        /// <para><b>Exemplo:</b></para>
        /// <code>
        /// var user = await repository.FindOneAsync(u => u.Email == "teste@exemplo.com");
        /// if (user != null)
        /// {
        ///     Console.WriteLine(user.Id);
        /// }
        /// </code>
        /// </remarks>
        /// <param name="filter">
        /// Expressão LINQ que define o critério de busca para o documento.
        /// </param>
        /// <returns>
        /// Uma <see cref="Task{TDocument}"/> contendo o documento encontrado,
        /// ou <c>null</c> caso nenhum documento satisfaça o filtro.
        /// </returns>
        Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Busca um único documento que satisfaça o filtro especificado, aplicando uma projeção.
        /// </summary>
        /// <remarks>
        /// Este método retorna o primeiro documento que corresponda à expressão fornecida,
        /// aplicando a projeção definida em <paramref name="projection"/> para selecionar apenas os campos desejados.
        ///
        /// ⚠️ A operação é assíncrona e deve ser aguardada com <c>await</c> para garantir que a busca seja concluída.
        ///
        /// Se nenhum documento corresponder ao filtro, o método retornará <c>null</c>.
        ///
        /// <para><b>Exemplo: Selecionando campos específicos</b></para>
        /// <code>
        /// var userProjection = await repository.FindOneAsync(
        ///     u =&gt; u.Id == "64f3b2c2f3d4b12a1c9e1234",
        ///     u =&gt; new User { Id = u.Id, Email = u.Email }
        /// );
        /// if (userProjection != null)
        /// {
        ///     Console.WriteLine(userProjection.Email);
        /// }
        /// </code>
        /// </remarks>
        /// <param name="filter">Expressão LINQ que define qual documento será buscado.</param>
        /// <param name="projection">Expressão LINQ que define quais campos do documento serão retornados.</param>
        /// <returns>
        /// Um <see cref="Task{TDocument}"/> representando a operação assíncrona,
        /// retornando o documento projetado ou <c>null</c> se nenhum documento for encontrado.
        /// </returns>
        Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TDocument>> projection);

        /// <summary>
        /// Busca todos os documentos com paginação.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista de documentos na página especificada, limitada pelo tamanho da página definido.
        /// 
        /// ⚠️ A operação é assíncrona e deve ser aguardada com <c>await</c>.
        /// 
        /// <para><b>Exemplo: Página 1 com 20 itens</b></para>
        /// <code>
        /// var results = await repository.FindAsync(pageNumber: 1, pageSize: 20);
        /// foreach(var item in results)
        /// {
        ///     Console.WriteLine(item.Code);
        /// }
        /// </code>
        /// </remarks>
        /// <param name="pageNumber">Número da página (1-based).</param>
        /// <param name="pageSize">Quantidade de itens por página.</param>
        /// <returns>Documentos da página especificada.</returns>
        Task<IEnumerable<TDocument>> FindAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Busca todos os documentos com ordenação e paginação.
        /// </summary>
        /// <remarks>
        /// Retorna documentos ordenados pelas chaves especificadas, limitados pelo tamanho da página.
        /// 
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Ordenação por Code ascendente, página 1 com 20 itens</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     sortBy: new[] { (p => p.Code, true) },
        ///     pageNumber: 1,
        ///     pageSize: 20
        /// );
        /// </code>
        /// </remarks>
        /// <param name="sortBy">Lista de chaves para ordenação e direção (true = ascendente).</param>
        /// <param name="pageNumber">Número da página (1-based).</param>
        /// <param name="pageSize">Quantidade de itens por página.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy,
            int pageNumber,
            int pageSize
        );

        /// <summary>
        /// Busca documentos projetados com paginação.
        /// </summary>
        /// <remarks>
        /// Retorna documentos aplicando a projeção e limitados pelo tamanho da página.
        /// 
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Selecionando apenas Id e Code, página 1 com 20 itens</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     projection: p => new Product { Id = p.Id, Code = p.Code },
        ///     pageNumber: 1,
        ///     pageSize: 20
        /// );
        /// </code>
        /// </remarks>
        /// <param name="projection">Define quais campos do documento serão retornados.</param>
        /// <param name="pageNumber">Número da página (1-based).</param>
        /// <param name="pageSize">Quantidade de itens por página.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, TDocument>> projection,
            int pageNumber,
            int pageSize
        );

        /// <summary>
        /// Busca documentos projetados com ordenação e paginação.
        /// </summary>
        /// <remarks>
        /// Retorna documentos projetados e ordenados pelas chaves especificadas.
        /// 
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Ordenando por Code ascendente e projetando Id e Code</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     sortBy: new[] { (p => p.Code, true) },
        ///     projection: p => new Product { Id = p.Id, Code = p.Code },
        ///     pageNumber: 1,
        ///     pageSize: 20
        /// );
        /// </code>
        /// </remarks>
        /// <param name="sortBy">Chaves de ordenação com direção (true = ascendente).</param>
        /// <param name="projection">Campos do documento a retornar.</param>
        /// <param name="pageNumber">Número da página.</param>
        /// <param name="pageSize">Tamanho da página.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy,
            Expression<Func<TDocument, TDocument>> projection,
            int pageNumber,
            int pageSize
        );

        /// <summary>
        /// Busca documentos que satisfaçam o filtro especificado.
        /// </summary>
        /// <remarks>
        /// Retorna todos os documentos que correspondam à expressão fornecida.
        /// 
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo</b></para>
        /// <code>
        /// var results = await repository.FindAsync(p => p.Balance > 0);
        /// foreach(var item in results)
        /// {
        ///     Console.WriteLine(item.Code);
        /// }
        /// </code>
        /// </remarks>
        /// <param name="filter">Expressão LINQ para filtrar os documentos.</param>
        Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter);

        /// <summary>
        /// Busca documentos filtrados e ordenados.
        /// </summary>
        /// <remarks>
        /// Retorna documentos que correspondam ao filtro e ordenados pelas chaves especificadas.
        /// 
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo e ordenar por Code</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     p => p.Balance > 0,
        ///     new[] { (p => p.Code, true) }
        /// );
        /// </code>
        /// </remarks>
        /// <param name="filter">Expressão LINQ para filtrar os documentos.</param>
        /// <param name="sortBy">Chaves de ordenação.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, bool>> filter,
            IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy
        );

        /// <summary>
        /// Busca documentos filtrados com paginação.
        /// </summary>
        /// <remarks>
        /// Retorna documentos que correspondam ao filtro e limitados pelo tamanho da página.
        /// 
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo, página 1, 20 itens</b></para>
        /// <code>
        /// var results = await repository.FindAsync(p => p.Balance > 0, 1, 20);
        /// </code>
        /// </remarks>
        /// <param name="filter">Filtro para os documentos.</param>
        /// <param name="pageNumber">Número da página.</param>
        /// <param name="pageSize">Tamanho da página.</param>
        Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, int pageNumber, int pageSize);

        /// <summary>
        /// Busca documentos filtrados, ordenados e paginados.
        /// </summary>
        /// <remarks>
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo, ordenar por Code asc, página 1, 20 itens</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     p => p.Balance > 0,
        ///     new[] { (p => p.Code, true) },
        ///     1,
        ///     20
        /// );
        /// </code>
        /// </remarks>
        /// <param name="filter">Filtro para os documentos.</param>
        /// <param name="sortBy">Chaves de ordenação.</param>
        /// <param name="pageNumber">Número da página.</param>
        /// <param name="pageSize">Tamanho da página.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, bool>> filter,
            IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy,
            int pageNumber,
            int pageSize
        );

        /// <summary>
        /// Busca documentos filtrados e aplicando projeção.
        /// </summary>
        /// <remarks>
        /// Retorna documentos que correspondam ao filtro, projetando apenas os campos especificados.
        /// 
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo, retornando apenas Id e Balance</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     p => p.Balance > 0,
        ///     p => new Product { Id = p.Id, Balance = p.Balance }
        /// );
        /// </code>
        /// </remarks>
        /// <param name="filter">Filtro LINQ.</param>
        /// <param name="projection">Projeção LINQ.</param>
        Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TDocument>> projection);

        /// <summary>
        /// Busca documentos filtrados, ordenados e projetados.
        /// </summary>
        /// <remarks>
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo, ordenar por Code, projetando Id e Balance</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     p => p.Balance > 0,
        ///     new[] { (p => p.Code, true) },
        ///     p => new Product { Id = p.Id, Balance = p.Balance }
        /// );
        /// </code>
        /// </remarks>
        /// <param name="filter">Filtro LINQ.</param>
        /// <param name="sortBy">Chaves de ordenação.</param>
        /// <param name="projection">Projeção LINQ.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, bool>> filter,
            IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy,
            Expression<Func<TDocument, TDocument>> projection
        );

        /// <summary>
        /// Busca documentos filtrados, projetados e paginados.
        /// </summary>
        /// <remarks>
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo, projetando Id e Balance, página 1, 20 itens</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     p => p.Balance > 0,
        ///     p => new Product { Id = p.Id, Balance = p.Balance },
        ///     1,
        ///     20
        /// );
        /// </code>
        /// </remarks>
        /// <param name="filter">Filtro LINQ.</param>
        /// <param name="projection">Projeção LINQ.</param>
        /// <param name="pageNumber">Número da página.</param>
        /// <param name="pageSize">Tamanho da página.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, bool>> filter,
            Expression<Func<TDocument, TDocument>> projection,
            int pageNumber,
            int pageSize
        );

        /// <summary>
        /// Busca documentos filtrados, ordenados, projetados e paginados.
        /// </summary>
        /// <remarks>
        /// ⚠️ A operação é assíncrona.
        /// 
        /// <para><b>Exemplo: Buscar produtos com saldo positivo, ordenar por Code, projetar Id e Balance, página 1, 20 itens</b></para>
        /// <code>
        /// var results = await repository.FindAsync(
        ///     p => p.Balance > 0,
        ///     new[] { (p => p.Code, true) },
        ///     p => new Product { Id = p.Id, Balance = p.Balance },
        ///     1,
        ///     20
        /// );
        /// </code>
        /// </remarks>
        /// <param name="filter">Filtro LINQ.</param>
        /// <param name="sortBy">Chaves de ordenação.</param>
        /// <param name="projection">Projeção LINQ.</param>
        /// <param name="pageNumber">Número da página.</param>
        /// <param name="pageSize">Tamanho da página.</param>
        Task<IEnumerable<TDocument>> FindAsync(
            Expression<Func<TDocument, bool>> filter,
            IEnumerable<(Expression<Func<TDocument, object>> keySelector, bool ascending)> sortBy,
            Expression<Func<TDocument, TDocument>> projection,
            int pageNumber,
            int pageSize
        );

        /// <summary>
        /// Insere um novo documento na coleção MongoDB.
        /// </summary>
        /// <remarks>
        /// Este método adiciona o documento fornecido à coleção correspondente.
        /// 
        /// ⚠️ Se a propriedade <see cref="Document.Id"/> estiver vazia (<c>null</c> ou <c>string.Empty</c>), 
        /// o MongoDB irá gerar automaticamente um <c>ObjectId</c> para o documento. 
        /// O driver C# atualizará a propriedade <c>Id</c> do objeto após a inserção.
        ///
        /// ⚠️ A operação é assíncrona e deve ser aguardada com <c>await</c> para garantir que a inserção seja concluída.
        ///
        /// <para><b>Exemplo:</b></para>
        /// <code>
        /// var invoice = new Invoice { Number = "F123", TotalAmount = 1000m };
        /// await repository.InsertOneAsync(invoice);
        /// Console.WriteLine(invoice.Id); // Id gerado pelo MongoDB
        /// </code>
        /// </remarks>
        /// <param name="entity">Documento que será inserido na coleção.</param>
        /// <returns>Uma <see cref="Task"/> representando a operação assíncrona.</returns>
        Task InsertOneAsync(TDocument entity);

        /// <summary>
        /// Atualiza um único documento na coleção que corresponda ao filtro especificado.
        /// </summary>
        /// <remarks>
        /// Este método aplica a atualização definida em <paramref name="update"/> 
        /// ao primeiro documento que corresponder ao filtro fornecido.
        ///
        /// ⚠️ A operação é assíncrona e deve ser aguardada com <c>await</c> para garantir que a atualização seja concluída.
        ///
        /// <para><b>Exemplo:</b></para>
        /// <code>
        /// var filter = Builders&lt;Product&gt;.Filter.Eq(p =&gt; p.Id, "64f3b2c2f3d4b12a1c9e1234");
        /// var update = Builders&lt;Product&gt;.Update.Set(p =&gt; p.Balance, 50);
        /// var result = await repository.UpdateOneAsync(filter, update);
        ///
        /// if (result.ModifiedCount &gt; 0)
        /// {
        ///     Console.WriteLine("Documento atualizado com sucesso.");
        /// }
        /// </code>
        /// </remarks>
        /// <param name="filter">Filtro que determina qual documento será atualizado.</param>
        /// <param name="update">Definição das alterações que serão aplicadas ao documento.</param>
        /// <returns>
        /// Um <see cref="Task{UpdateResult}"/> representando a operação assíncrona. 
        /// O <see cref="UpdateResult"/> contém informações sobre quantos documentos foram modificados.
        /// </returns>
        Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update);

        /// <summary>
        /// Localiza um único documento e o atualiza de forma atômica, retornando o documento
        /// antes ou depois da atualização conforme as opções informadas.
        /// </summary>
        /// <description>
        /// Este método executa uma operação atômica de busca e atualização no MongoDB.
        /// Ele localiza um documento utilizando o filtro especificado, aplica a definição
        /// de atualização e retorna o documento modificado de acordo com as opções configuradas.
        ///
        /// A operação é executada em uma única chamada ao banco de dados, garantindo
        /// consistência e evitando condições de corrida em cenários concorrentes.
        ///
        /// Este método é comumente utilizado para:
        /// - Contadores auto-incrementais
        /// - Atualizações condicionais
        /// - Operações de leitura-modificação-escrita
        /// - Transições de estado
        /// </description>
        /// <processing>
        /// 1. Aplica o filtro informado para localizar um documento.
        /// 2. Executa a definição de atualização no documento encontrado.
        /// 3. Aplica comportamentos opcionais definidos em FindOneAndUpdateOptions.
        /// 4. Retorna o documento antes ou depois da atualização.
        /// </processing>
        /// <business>
        /// - A operação é atômica para um único documento.
        /// - Se nenhum documento for encontrado e Upsert estiver habilitado, um novo documento será criado.
        /// - O documento retornado depende da opção ReturnDocument.
        /// </business>
        /// <errors>
        /// - MongoException quando a operação no banco falha.
        /// - ArgumentNullException se filter ou update forem nulos.
        /// </errors>
        /// <param name="filter">
        /// Definição de filtro utilizada para localizar o documento alvo.
        /// </param>
        /// <param name="update">
        /// Definição de atualização que descreve as modificações a serem aplicadas.
        /// </param>
        /// <param name="options">
        /// Configurações opcionais da operação, como comportamento de Upsert e modo de retorno do documento.
        /// </param>
        /// <returns>
        /// O documento encontrado e atualizado, ou null caso nenhum documento seja localizado
        /// e Upsert não esteja habilitado.
        /// </returns>
        Task<TDocument?> FindOneAndUpdateAsync(
            FilterDefinition<TDocument> filter,
            UpdateDefinition<TDocument> update,
            FindOneAndUpdateOptions<TDocument>? options = null);

        /// <summary>
        /// Substitui um único documento que corresponda ao filtro informado.
        /// </summary>
        /// <param name="filter">Filtro para localizar o documento.</param>
        /// <param name="replacement">Documento que substituirá o atual.</param>
        /// <param name="options">Opções da operação de substituição.</param>
        /// <returns>Resultado da operação de substituição.</returns>
        Task<ReplaceOneResult> ReplaceOneAsync(
            Expression<Func<TDocument, bool>> filter,
            TDocument replacement,
            ReplaceOptions? options = null);

        /// <summary>
        /// Obtém o próximo valor sequencial numérico para o tipo de documento atual.
        /// </summary>
        /// <remarks>
        /// Este método utiliza uma coleção de contadores no MongoDB para gerar
        /// valores incrementais atômicos por tipo de entidade. O identificador
        /// do contador é baseado no nome da coleção definido pelo atributo
        /// BsonCollection do <typeparamref name="TDocument"/>.
        ///
        /// A operação é atômica e segura para acesso concorrente.
        /// Caso não exista um contador para a entidade, um novo será criado automaticamente.
        /// </remarks>
        /// <returns>
        /// Um <see cref="long"/> representando o próximo valor sequencial da entidade.
        /// </returns>
        Task<long> GetNextSequenceValueAsync();
    }
}