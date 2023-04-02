using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Consulta Venda API",
        Description = "API com as principais consultas de venda na camada do Próton Retail e também na camada de loja.",
        TermsOfService = new Uri("https://lebiscuit.com.br/termos"),
        Contact = new OpenApiContact
        {
            Name = "Lojas Le biscuit SA",
            Url = new Uri("https://www.lebiscuit.com.br")
        },
        License = new OpenApiLicense
        {
            Name = "(C)2023 Lojas Le biscuit S/A",
            Url = new Uri("https://www.lebiscuit.com.br")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
