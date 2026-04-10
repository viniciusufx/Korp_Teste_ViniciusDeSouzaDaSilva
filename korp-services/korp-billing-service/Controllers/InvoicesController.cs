using System.ComponentModel.DataAnnotations;
using Korp.BillingService.DTO;
using Korp.BillingService.Entities.Implementations;
using Korp.BillingService.Infrastructure.Services.Stock;
using Korp.BillingService.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Korp.BillingService.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de notas fiscais.
    /// </summary>
    /// <remarks>
    /// Este controller disponibiliza endpoints para criação, consulta, atualização,
    /// exclusão e impressão de notas fiscais.
    /// 
    /// Também realiza integração com o microserviço de estoque para efetuar a baixa
    /// de produtos durante o processo de impressão.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IMongoRepository<InvoiceEntitie> _invoicesRepository;
        private readonly IStockService _stockService;

        /// <summary>
        /// Inicializa uma nova instância do controlador de notas fiscais.
        /// </summary>
        /// <remarks>
        /// O construtor recebe as dependências necessárias para o funcionamento do controller,
        /// incluindo o repositório de notas fiscais e o serviço responsável pela comunicação
        /// com o microserviço de estoque.
        /// 
        /// Essas dependências são injetadas via Dependency Injection.
        /// </remarks>
        /// <param name="invoicesRepository">
        /// Repositório responsável pelas operações de acesso a dados das notas fiscais.
        /// </param>
        /// <param name="stockService">
        /// Serviço responsável pela comunicação com o microserviço de estoque.
        /// </param>
        public InvoicesController(
            IMongoRepository<InvoiceEntitie> invoicesRepository,
            IStockService stockService
        )
        {
            _stockService = stockService;
            _invoicesRepository = invoicesRepository;
        }

        /// <summary>
        /// Cria uma nova nota fiscal no sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint cria uma nova nota fiscal com status inicial "Open".
        /// 
        /// O número da nota fiscal é gerado automaticamente através de uma sequência
        /// incremental garantida por uma operação atômica, segura para acesso concorrente.
        /// 
        /// Os itens informados são mapeados para a entidade de domínio e persistidos
        /// juntamente com a nota fiscal.
        /// </remarks>
        /// <param name="invoice">
        /// Objeto contendo os dados necessários para criação da nota fiscal,
        /// incluindo a lista de produtos e suas respectivas quantidades.
        /// </param>
        /// <returns>
        /// Retorna HTTP 201 (Created) com a nota fiscal criada.
        /// </returns>
        /// <response code="201">Nota fiscal criada com sucesso.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewInvoiceDto invoice)
        {
            var newInvoice = new InvoiceEntitie
            {
                // A operação é atômica e segura para acesso concorrente.
                Number = await _invoicesRepository.GetNextSequenceValueAsync(),
                Items = invoice.Items.Select(i => new InvoiceItemEntitie{ ProductId = i.ProductId, Quantity = i.Quantity}).ToList(),
                Status = InvoiceStatusEntitie.Open
            };

            await _invoicesRepository.InsertOneAsync(newInvoice);
            var invoiceDto = new InvoicePagedResultDto
            {
                Id = newInvoice.Id,
                Number = newInvoice.Number,
                Items = newInvoice.Items.Select(
                    item => new InvoiceItemDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList(),
                Status = (InvoiceStatusDto)newInvoice.Status
            };

            return CreatedAtAction(nameof(GetById), new { id = invoiceDto.Id }, invoiceDto);
        }

        /// <summary>
        /// Lista notas fiscais de forma paginada.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna uma lista paginada de notas fiscais cadastradas no sistema.
        /// 
        /// Os parâmetros pageNumber e pageSize controlam a paginação dos resultados.
        /// Caso não existam registros para os parâmetros informados, uma resposta vazia é retornada.
        /// 
        /// Cada nota fiscal contém:
        /// - Identificador
        /// - Número sequencial
        /// - Lista de itens com produto e quantidade
        /// - Status da nota fiscal
        /// </remarks>
        /// <param name="pageNumber">Número da página a ser recuperada, deve ser ≥ 1.</param>
        /// <param name="pageSize">Quantidade de itens por página, valores permitidos: 15, 30 ou 45.</param>
        /// <returns>
        /// Retorna HTTP 200 (OK) com a lista paginada de notas fiscais.
        /// Caso não existam registros, retorna HTTP 200 com conteúdo vazio.
        /// </returns>
        /// <response code="200">Lista de notas fiscais retornada com sucesso.</response>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery]
            [Range(1, int.MaxValue, ErrorMessage = "PageNumber deve ser maior ou igual a 1.")]
            int pageNumber = 1, 
            [FromQuery]
            [RegularExpression("^(15|30|45)$", ErrorMessage = "O valor de PageSize deve ser 15, 30 ou 45.")]
            int pageSize = 15
        )
        {
            var invoices = await _invoicesRepository.FindAsync(pageNumber,pageSize);
            if (invoices == null)
                return Ok();
            var invoicesDto = invoices.Select(
                i => new InvoicePagedResultDto 
                {
                    Id = i.Id,
                    Number = i.Number, 
                    Items = i.Items.Select(item => new InvoiceItemDto
                        {
                            ProductId = item.ProductId, Quantity = item.Quantity
                        }).ToList(), 
                    Status = (InvoiceStatusDto)i.Status
                }).ToList();

            return Ok(invoicesDto);
        }

        /// <summary>
        /// Consulta uma nota fiscal pelo identificador.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna os dados de uma nota fiscal específica com base no seu identificador único.
        /// 
        /// Caso a nota fiscal seja encontrada, são retornados:
        /// - Identificador
        /// - Número sequencial
        /// - Lista de itens contendo produto e quantidade
        /// - Status da nota fiscal
        /// 
        /// Caso o identificador informado não corresponda a nenhum registro,
        /// uma resposta 404 será retornada.
        /// </remarks>
        /// <param name="id">
        /// Identificador único da nota fiscal (deve conter exatamente 24 caracteres).
        /// </param>
        /// <returns>
        /// Retorna HTTP 200 (OK) com os dados da nota fiscal.
        /// Retorna HTTP 404 (NotFound) caso a nota fiscal não seja encontrada.
        /// </returns>
        /// <response code="200">Nota fiscal encontrada com sucesso.</response>
        /// <response code="404">Nota fiscal não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id
        )
        {
            var invoice = await _invoicesRepository.FindOneAsync(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            var invoicesDto = new InvoicePagedResultDto 
                {
                    Id = invoice.Id,
                    Number = invoice.Number, 
                    Items = invoice.Items.Select(item => new InvoiceItemDto
                        {
                            ProductId = item.ProductId, Quantity = item.Quantity
                        }).ToList(), 
                    Status = (InvoiceStatusDto)invoice.Status
                };

            return Ok(invoicesDto);
        }

        /// <summary>
        /// Atualiza uma nota fiscal existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza a atualização dos dados de uma nota fiscal com base no seu identificador.
        /// 
        /// Os campos atualizados incluem:
        /// - Número da nota fiscal
        /// - Status
        /// - Lista de itens (produto e quantidade)
        /// 
        /// O sistema primeiro verifica se a nota fiscal existe. Caso não exista,
        /// uma resposta 404 será retornada.
        /// 
        /// Após a validação, os dados são atualizados no banco de dados.
        /// </remarks>
        /// <param name="id">
        /// Identificador único da nota fiscal (deve conter exatamente 24 caracteres).
        /// </param>
        /// <param name="invoice">
        /// Objeto contendo os novos dados da nota fiscal.
        /// </param>
        /// <returns>
        /// Retorna HTTP 204 (NoContent) em caso de sucesso.
        /// Retorna HTTP 404 (NotFound) caso a nota fiscal não seja encontrada.
        /// </returns>
        /// <response code="204">Nota fiscal atualizada com sucesso.</response>
        /// <response code="404">Nota fiscal não encontrada.</response>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id, 
            [FromBody] UpdateInvoiceDto invoice
        )
        {
            var exists = await _invoicesRepository.ExistAsync(x => x.Id == id);

            if (!exists)
                return NotFound();

            var update = Builders<InvoiceEntitie>.Update
                .Set(i => i.Number, invoice.Number)
                .Set(i => i.Status, (InvoiceStatusEntitie)invoice.Status)
                .Set(i => i.Items, invoice.Items.Select(
                    item => new InvoiceItemEntitie
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }).ToList());
            var result = await _invoicesRepository.UpdateOneAsync(i => i.Id == id, update);

            return NoContent();
        }

        /// <summary>
        /// Remove uma nota fiscal do sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza a exclusão de uma nota fiscal com base no seu identificador único.
        /// 
        /// A operação tenta remover o registro diretamente no banco de dados.
        /// Caso nenhuma nota fiscal seja encontrada com o identificador informado,
        /// uma resposta 404 será retornada.
        /// 
        /// Se a exclusão for realizada com sucesso, a API retorna 204 sem conteúdo.
        /// </remarks>
        /// <param name="id">
        /// Identificador único da nota fiscal (deve conter exatamente 24 caracteres).
        /// </param>
        /// <returns>
        /// Retorna HTTP 204 (NoContent) em caso de sucesso.
        /// Retorna HTTP 404 (NotFound) caso a nota fiscal não seja encontrada.
        /// </returns>
        /// <response code="204">Nota fiscal removida com sucesso.</response>
        /// <response code="404">Nota fiscal não encontrada.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            [FromRoute]
            [Required(ErrorMessage = "O campo 'Id' é obrigatório e não pode estar vazio.")]
            [Length(24,24, ErrorMessage = "O campo 'Id' deve conter exatamente 24 caracteres.")]
            string id
        )
        {
            var result = await _invoicesRepository.DeleteOneAsync(x => x.Id == id);

            if (result.DeletedCount == 0)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Realiza a impressão de uma nota fiscal e efetua a baixa no estoque.
        /// </summary>
        /// <remarks>
        /// Este endpoint simula o processo de impressão de uma nota fiscal.
        /// 
        /// O fluxo da operação é:
        /// 1. Busca a nota fiscal pelo identificador.
        /// 2. Verifica se a nota fiscal existe.
        /// 3. Verifica se a nota fiscal está com status "Open".
        /// 4. Para cada item da nota fiscal, realiza a baixa do saldo no microserviço de estoque.
        /// 5. Atualiza o status da nota fiscal para "Closed".
        /// 6. Retorna a nota fiscal atualizada.
        /// 
        /// Apenas notas fiscais com status "Open" podem ser impressas.
        /// Após a impressão, a nota é automaticamente fechada.
        /// </remarks>
        /// <param name="id">
        /// Identificador único da nota fiscal.
        /// </param>
        /// <returns>
        /// Retorna HTTP 200 (OK) com a nota fiscal atualizada.
        /// Retorna HTTP 404 (NotFound) caso a nota fiscal não seja encontrada.
        /// Retorna HTTP 400 (BadRequest) caso a nota fiscal não esteja aberta.
        /// </returns>
        /// <response code="200">Nota fiscal impressa e fechada com sucesso.</response>
        /// <response code="404">Nota fiscal não encontrada.</response>
        /// <response code="400">Apenas notas fiscais abertas podem ser impressas.</response>
        [HttpPost("{id}/print")]
        public async Task<IActionResult> Print(string id)
        {
            var invoice = await _invoicesRepository.FindOneAsync(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            if (invoice.Status != InvoiceStatusEntitie.Open)
                return BadRequest("Only open invoices can be printed.");

            foreach (var item in invoice.Items)
            {
                await _stockService.BaixarSaldoProdutoAsync(item.ProductId, item.Quantity);
            }

            var filter = Builders<InvoiceEntitie>.Filter.Eq(i => i.Id, id);

            var update = Builders<InvoiceEntitie>.Update.Set(p => p.Status, InvoiceStatusEntitie.Closed);

            var options = new FindOneAndUpdateOptions<InvoiceEntitie>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _invoicesRepository.FindOneAndUpdateAsync(filter, update, options);

            return Ok(result);
        }
    }
}