using System.ComponentModel.DataAnnotations;
using Korp.StockService.DTOs;
using Korp.StockService.Entities.Implementations;
using Korp.StockService.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Korp.StockService.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar produtos no sistema.
    /// </summary>
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMongoRepository<ProductEntitie> _productsRepository;

        /// <summary>
        /// Construtor do ProductsController.
        /// </summary>
        /// <param name="productsRepository">Repositório genérico para operações com produtos.</param>
        public ProductsController(IMongoRepository<ProductEntitie> productsRepository)
        {
            _productsRepository = productsRepository;
        }

        /// <summary>
        /// Recupera uma lista paginada de produtos.
        /// </summary>
        /// <description>
        /// Este endpoint retorna produtos em páginas, permitindo que o cliente especifique o número da página e a quantidade de itens por página.
        /// Cada produto é mapeado para um ProductDto para retornar apenas as informações relevantes.
        /// </description>
        /// <processing>
        /// 1. Valida pageNumber e pageSize usando DataAnnotations.
        /// 2. Consulta o repositório de produtos com paginação.
        /// 3. Mapeia cada entidade Product para ProductDto.
        /// 4. Retorna a lista de ProductDto como resposta.
        /// </processing>
        /// <businessRules>
        /// - pageNumber deve ser maior ou igual a 1.
        /// - pageSize deve ser um dos valores permitidos: 15, 30 ou 45.
        /// </businessRules>
        /// <possibleErrors>
        /// - 400 Bad Request: se pageNumber for menor que 1 ou pageSize não for 15, 30 ou 45.
        /// </possibleErrors>
        /// <param name="pageNumber">Número da página a ser recuperada, deve ser ≥ 1.</param>
        /// <param name="pageSize">Quantidade de itens por página, valores permitidos: 15, 30 ou 45.</param>
        /// <returns>Uma lista de ProductDto representando a página solicitada de produtos.</returns>
        /// <response code="200">Retorna a lista paginada de produtos.</response>
        /// <response code="400">Retornado se pageNumber ou pageSize forem inválidos.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery]
            [Range(1, int.MaxValue, ErrorMessage = "PageNumber deve ser maior ou igual a 1.")]
            int pageNumber = 1,
            [FromQuery]
            [RegularExpression("^(15|30|45)$", ErrorMessage = "O valor de PageSize deve ser 15, 30 ou 45.")]
            int pageSize = 15
        )
        {
            var products = await _productsRepository.FindAsync(pageNumber, pageSize);
            List<ProductDto> listProducts = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Descricao = p.Descricao,
                Saldo = p.Saldo
            }).ToList();
            return Ok(listProducts);
        }

        /// <summary>
        /// Obtém um produto pelo seu identificador único.
        /// </summary>
        /// <remarks>
        /// Este endpoint busca um produto no banco de dados utilizando o seu ID.
        /// Caso o produto não seja encontrado, retorna HTTP 404 (NotFound).
        /// Caso seja encontrado, retorna os dados do produto mapeados para um DTO.
        /// </remarks>
        /// <param name="id">
        /// Identificador único do produto (24 caracteres, formato ObjectId do MongoDB).
        /// </param>
        /// <returns>
        /// Retorna HTTP 200 (Ok) com os dados do produto quando encontrado,
        /// ou HTTP 404 (NotFound) quando o produto não existir.
        /// </returns>
        /// <response code="200">Produto encontrado com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id
        )
        {
            var product = await _productsRepository.FindOneAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            var productDto = new ProductDto
            {
                Id = product.Id,
                Codigo = product.Codigo,
                Descricao = product.Descricao,
                Saldo = product.Saldo
            };

            return Ok(productDto);
        }
        
        /// <summary>
        /// Cria um novo produto no sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint recebe os dados de um produto, realiza o mapeamento para a entidade de domínio
        /// e persiste o registro no banco de dados.
        /// 
        /// Após a inserção, o identificador gerado é validado. Caso o ID não seja retornado,
        /// é considerado erro de persistência e uma resposta de erro é retornada.
        /// </remarks>
        /// <param name="product">
        /// Objeto contendo os dados necessários para criação do produto.
        /// </param>
        /// <returns>
        /// Retorna HTTP 201 (Created) com o produto criado em formato DTO.
        /// Em caso de falha na persistência, retorna HTTP 500 (Problem).
        /// </returns>
        /// <response code="201">Produto criado com sucesso.</response>
        /// <response code="500">Falha ao inserir o produto no banco de dados.</response>
        [HttpPost]
        public async Task<IActionResult> Create(NewProductDto product)
        {
            var newProduct = new ProductEntitie
            {
                Codigo = product.Codigo,
                Descricao = product.Descricao,
                Saldo = product.Saldo
            };

            await _productsRepository.InsertOneAsync(newProduct);
            if (string.IsNullOrEmpty(newProduct.Id))
                return Problem("Falha ao inserir o produto.");

            var productDto = new ProductDto
            {
                Id = newProduct.Id,
                Codigo = newProduct.Codigo,
                Descricao = newProduct.Descricao,
                Saldo = newProduct.Saldo
            };
            
            return CreatedAtAction(nameof(GetById), new { id = productDto.Id }, productDto);
        }

        /// <summary>
        /// Atualiza um produto existente no sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza a atualização parcial (PATCH) de um produto existente no banco de dados.
        /// 
        /// O sistema valida se o produto existe antes de tentar aplicar as alterações.
        /// Em seguida, os campos informados (Codigo, Descricao e Saldo) são atualizados na base.
        /// 
        /// Após a operação, o resultado do MongoDB é verificado:
        /// - MatchedCount: garante que o produto foi encontrado.
        /// - ModifiedCount: garante que a atualização foi efetivamente aplicada.
        /// 
        /// Caso alguma dessas validações falhe, uma resposta de erro é retornada.
        /// </remarks>
        /// <param name="id">
        /// Identificador único do produto (deve conter exatamente 24 caracteres).
        /// </param>
        /// <param name="updatedProduct">
        /// Objeto contendo os dados atualizados do produto.
        /// </param>
        /// <returns>
        /// Retorna HTTP 204 (NoContent) em caso de sucesso.
        /// Retorna HTTP 404 (NotFound) caso o produto não seja encontrado.
        /// Retorna HTTP 500 (Problem) caso a atualização falhe.
        /// </returns>
        /// <response code="204">Produto atualizado com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        /// <response code="500">Falha ao atualizar o produto no banco de dados.</response>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id,
            UpdateProductDto updatedProduct
        )
        {
            if (!await _productsRepository.ExistAsync(p => p.Id == id))
                return NotFound();

            var filter = Builders<ProductEntitie>.Filter.Eq(p => p.Id, id);
            var update = Builders<ProductEntitie>.Update
                .Set(p => p.Codigo, updatedProduct.Codigo)
                .Set(p => p.Descricao, updatedProduct.Descricao)
                .Set(p => p.Saldo, updatedProduct.Saldo);

            var result = await _productsRepository.UpdateOneAsync(filter, update);
            if (result.MatchedCount != 1)
                return NotFound("Falha ao encontrar o produto para atualizar.");
            if (result.ModifiedCount != 1)
                return Problem("Falha ao atualizar o produto.");

            return NoContent();
        }

        /// <summary>
        /// Remove um produto existente do sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza a exclusão de um produto no banco de dados com base no seu identificador único.
        /// 
        /// Antes da exclusão, o sistema valida se o produto existe.
        /// Caso o produto não seja encontrado, é retornada uma resposta 404.
        /// 
        /// Se o produto existir, ele é removido permanentemente do banco de dados.
        /// </remarks>
        /// <param name="id">
        /// Identificador único do produto (deve conter exatamente 24 caracteres).
        /// </param>
        /// <returns>
        /// Retorna HTTP 204 (NoContent) quando o produto é removido com sucesso.
        /// Retorna HTTP 404 (NotFound) caso o produto não exista.
        /// </returns>
        /// <response code="204">Produto removido com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id
        )
        {
            
            if (!await _productsRepository.ExistAsync(p => p.Id == id))
                return NotFound();

            await _productsRepository.DeleteOneAsync(p => p.Id == id);
            return NoContent();
        }

        /// <summary>
        /// Consulta o saldo de um produto específico.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna apenas o saldo atual de um produto identificado pelo seu Id.
        /// 
        /// Para otimização de performance, é feita uma projeção no banco de dados,
        /// retornando somente os campos necessários (Id e Saldo), sem carregar o documento completo.
        /// 
        /// Caso o produto não seja encontrado, é retornada resposta 404.
        /// </remarks>
        /// <param name="id">
        /// Identificador único do produto (deve conter exatamente 24 caracteres).
        /// </param>
        /// <returns>
        /// Retorna HTTP 200 (OK) com o Id e o saldo do produto.
        /// Retorna HTTP 404 (NotFound) caso o produto não seja encontrado.
        /// </returns>
        /// <response code="200">Saldo do produto retornado com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpGet("{id}/saldo")]
        public async Task<IActionResult> GetBalance(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id
        )
        {
            var product = await _productsRepository.FindOneAsync(
                p => p.Id == id,
                p => new ProductEntitie { Id = p.Id, Saldo = p.Saldo }
            );
            if (product == null)
                return NotFound();

            return Ok(new { product.Id, product.Saldo });
        }

        /// <summary>
        /// Realiza a baixa de saldo de um produto.
        /// </summary>
        /// <remarks>
        /// Este endpoint reduz a quantidade em estoque de um produto específico.
        /// 
        /// A operação só é realizada se o produto existir e possuir saldo suficiente.
        /// O sistema utiliza uma operação atômica no MongoDB (FindOneAndUpdate) para garantir
        /// consistência, evitando condições de corrida.
        /// 
        /// A regra de negócio impede que o saldo fique negativo.
        /// Caso o saldo informado seja maior que o disponível, a operação é negada.
        /// </remarks>
        /// <param name="id">
        /// Identificador único do produto (deve conter exatamente 24 caracteres).
        /// </param>
        /// <param name="quantidade">
        /// Quantidade a ser debitada do saldo do produto.
        /// </param>
        /// <returns>
        /// Retorna HTTP 200 (OK) com o saldo atualizado.
        /// Retorna HTTP 404 (NotFound) caso o produto não exista.
        /// Retorna HTTP 409 (Conflict) caso não haja saldo suficiente.
        /// </returns>
        /// <response code="200">Saldo atualizado com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        /// <response code="409">Saldo insuficiente para realizar a operação.</response>
        [HttpPatch("{id}/baixar")]
        public async Task<IActionResult> BaixarBalance(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id, 
            [FromQuery]
            int quantidade
        )
        {
            if (! await _productsRepository.ExistAsync(p => p.Id == id))
                return NotFound();
            var filter = Builders<ProductEntitie>.Filter.And(
                Builders<ProductEntitie>.Filter.Eq(p => p.Id, id),
                Builders<ProductEntitie>.Filter.Gte(p => p.Saldo, quantidade)
            );
            var update = Builders<ProductEntitie>.Update.Inc(p => p.Saldo, -quantidade);
            var options = new FindOneAndUpdateOptions<ProductEntitie>
            {
                ReturnDocument = ReturnDocument.After
            };
            // A operação é atômica e segura para acesso concorrente.
            var result = await _productsRepository.FindOneAndUpdateAsync(filter, update, options);
            if (result == null)
                return Conflict("Saldo insuficiente para realizar a operação");

            return Ok(new { result.Id, result.Saldo });
        }
    }
}