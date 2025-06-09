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

//1. Adicionar Livros 
app.MapPost("/api/livros", ([FromBody] Livro livro, [FromServices] BibliotecaDbContext ctx) => 
{
    // Verificar caracteres
    /*if
    {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }
    */

    //Verifica se o categoriaId corresponde a uma categoria existente
    Categoria? categoria = ctx.Categorias.Find(livro.CategoriaId);
    if (categoria is null)
    {
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    livro.Categoria = categoria;

    ctx.Livros.Add(livro);
    ctx.SaveChanges();

    return Results.Created("/api/categorias"+livro.Id, livro);
    
});

//2.Listar Livros
app.MapGet("/api/livros", ([FromServices] BibliotecaDbContext ctx) =>
{
    if (ctx.Livros.Any())
    {
        return Results.Ok(ctx.Livros.Include(x => x.Categoria).ToList());
    }
    return Results.NotFound();
});

//3. Buscar livro por Id
app.MapGet("/api/livros/{id}", ([FromRoute] string id,
    [FromServices] BibliotecaDbContext ctx) =>
{
    Livro? livro = ctx.Livros.Find(id);
    if (livro == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(livro);
});


app.Run();


