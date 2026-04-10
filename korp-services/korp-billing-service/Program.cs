using System.Reflection;
using Korp.BillingService.Infrastructure.MongoDb;
using Korp.BillingService.Infrastructure.SecuritySettings;
using Korp.BillingService.Infrastructure.Services.Stock;
using Korp.BillingService.Repositories;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Associa a seção "SecuritySettings" do appsettings.json à classe SecuritySettings.
// Permite acessar as configurações tipadas através do padrão Options.
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetRequiredSection("SecuritySettings"));

// Registra as configurações de segurança como Singleton.
// Isso permite injetar ISecuritySettings diretamente nos serviços, evitando a necessidade de usar IOptions<SecuritySettings> em toda aplicação.
builder.Services.AddSingleton<ISecuritySettings>(serviceProvider => serviceProvider.GetRequiredService<IOptions<SecuritySettings>>().Value);

// Registra o provedor de acesso ao MongoDB como Singleton.
// Uma única instância será utilizada durante todo o ciclo de vida da aplicação, evitando múltiplas conexões desnecessárias com o banco.
builder.Services.AddSingleton<IMongoDbProvider, MongoDbProvider>();

// Registra o repositório genérico do MongoDB com ciclo de vida Scoped.
// Uma nova instância será criada por requisição HTTP, garantindo isolamento entre operações e permitindo o uso de diferentes entidades genéricas.
builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

// Registra o cliente HTTP para comunicação com o Stock Service (porta 5003).
// Este client será utilizado para validar saldo, reservar e debitar estoque.
builder.Services.AddHttpClient<IStockService, StockService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5003");
});

// Adiciona suporte a Controllers (API REST).
builder.Services.AddControllers();

// Obtém o nome do arquivo de documentação XML gerado a partir do assembly atual.
// Esse arquivo contém os comentários XML escritos com /// <summary>, /// <remarks>, etc.
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
// Monta o caminho completo para o arquivo XML localizado no diretório base da aplicação.
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

// Adiciona o Swagger/OpenAPI para documentação automática da API.
// Permite visualizar e testar os endpoints diretamente pelo navegador.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Habilita o uso de anotações do Swagger ([SwaggerOperation], [SwaggerResponse], etc.)
    options.EnableAnnotations();
    // Inclui os comentários XML na documentação do Swagger.
    // Isso permite que o Swagger leia automaticamente summaries, descrições de parâmetros e detalhes de respostas definidos nos comentários de documentação XML.
    options.IncludeXmlComments(xmlPath);
});

// Configuração da política de CORS (Cross-Origin Resource Sharing).
// Permite que o frontend Angular (rodando em http://localhost:4200) consiga acessar esta API durante o desenvolvimento.
// Sem essa configuração, o navegador bloquearia as requisições devido à política de mesma origem (Same-Origin Policy).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularAndServices",
        policy =>
        {
            policy
                // Permite requisições vindas do Angular
                .WithOrigins("http://localhost:4200", "http://localhost:5002", "https://localhost:5003")
                // Permite qualquer header (Authorization, Content-Type, etc.)
                .AllowAnyHeader()
                // Permite qualquer método HTTP (GET, POST, PUT, DELETE, etc.)
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    // Habilita geração do JSON do Swagger
    app.UseSwagger();
    // Habilita interface gráfica do Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Service API V1");
        // Swagger será carregado na raiz da aplicação: "/"
        options.RoutePrefix = string.Empty;
    });
}

// Redireciona automaticamente requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Habilita a política de CORS configurada.
app.UseCors("AllowAngularAndServices");

// Habilita autorização (necessário quando usar [Authorize])
app.UseAuthorization();

// Mapeia os controllers da API
app.MapControllers();

// Inicia a aplicação
app.Run();
