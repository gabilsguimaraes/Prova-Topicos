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
app.MapPost("/api/livros", ([FromBody] Livro livro, [FromServices] BibliotecaDbContext context) => 
{
    // Verificar caracteres
    /*if
    {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }
    */

    //Verifica se o categoriaId corresponde a uma categoria existente
    Categoria? categoria = context.Categorias.Find(livro.CategoriaId);
    if (categoria is null)
    {
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    livro.Categoria = categoria;

    context.Livros.Add(livro);
    context.SaveChanges();

    return Results.Created("/api/categorias"+livro.Id, livro);
    
});

//2.Listar Livros
app.MapGet("/api/livros", ([FromServices] BibliotecaDbContext context) =>
{
    if (context.Livros.Any())
    {
        return Results.Ok(context.Livros.Include(x => x.Categoria).ToList());
    }
    return Results.NotFound();
});

//3. Buscar livro por Id
app.MapGet("/api/livros/{id}", ([FromRoute] int id,
    [FromServices] BibliotecaDbContext context) =>
{
    Livro? livro = context.Livros.Find(id);
    if (livro == null)
    {
        return Results.NotFound("Livro com ID {id} não encontrado.");
    }
    return Results.Ok(context.Livros.Include(x => x.Categoria).ToList());
});

//4. Atualizar Livro
app.MapPut("/api/livross/{id}", ([FromRoute] int id,
    [FromBody] Livro livroAlterado,
    [FromServices] BibliotecaDbContext context) =>
{
    Livro? livro = context.Livros.Find(id);
    if (livro == null)
    {
        return Results.NotFound("Livro com ID {id} não encontrado para atualização.");
    }
    Categoria? categoria = context.Categorias.Find(livro.CategoriaId);
    if (categoria is null)
    {
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }
    livro.Categoria = categoria;
    livro.Titulo = livroAlterado.Titulo;
    livro.Autor = livroAlterado.Autor;
    context.Livros.Update(livro);
    context.SaveChanges();
    return Results.Ok(livro);
});


app.Run();


