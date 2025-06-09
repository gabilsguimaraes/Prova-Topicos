using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BibliotecaDbContext>();

var app = builder.Build();

app.MapGet("/", (BibliotecaDbContext context) =>
{
    var categorias = context.Categorias.ToList();
    return Results.Ok(categorias);
});


app.Run();
