// Arquivo: servico-estoque/Program.cs
// Program de inicialização para o serviço de estoque (configura serviços, CORS e pipeline)

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO DE SERVIÇOS ---

// Adiciona o serviço de Controladores (nossos endpoints da API)
builder.Services.AddControllers();

// Adiciona o Swagger/OpenAPI para documentação e teste da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- IMPORTANTE: Configuração do CORS ---
// Isso permite que o seu frontend Angular (que rodará em localhost:4200)
// possa "conversar" com este backend (que rodará em localhost:xxxx).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
        policy.WithOrigins("http://localhost:4200", "http://localhost:3441") // Permite também a porta dev usada
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// --- FIM DA CONFIGURAÇÃO DE SERVIÇOS ---

var app = builder.Build();

// --- INÍCIO DA CONFIGURAÇÃO DO PIPELINE HTTP ---

// Configura o Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona de HTTP para HTTPS somente em Development quando a porta HTTPS está configurada
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// --- IMPORTANTE: Habilita o CORS ---
app.UseCors("AllowAngularApp"); // Aplica a política que definimos

// Habilita o uso de Controladores
app.MapControllers();

// --- FIM DA CONFIGURAÇÃO DO PIPELINE HTTP ---

// Inicia a aplicação
app.Run();