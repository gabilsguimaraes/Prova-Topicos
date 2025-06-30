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
    // Verificar se título e autor possuem mais de 3 caracteres
    if (livro.Titulo.Length < 3)
    {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres");
    }

    if (livro.Autor.Length < 3){
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres");
    }
    
    //Verifica se o categoriaId corresponde a uma categoria existente
    Categoria? categoria = context.Categorias.Find(livro.CategoriaId);

    if (categoria is null)
    {
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    livro.Categoria = categoria;

    context.Livros.Add(livro);
    context.SaveChanges();

    return Results.Created($"/api/livros/{livro.Id}", livro);

    
});

//2.Listar Livros
app.MapGet("/api/livros", ([FromServices] BibliotecaDbContext context) =>
{
    var livros = context.Livros.Include(x => x.Categoria).ToList();
    return Results.Ok(livros);
});

//3. Buscar livro por Id
app.MapGet("/api/livros/{id}", ([FromRoute] int id,
    [FromServices] BibliotecaDbContext context) =>
{
    var livro = context.Livros
        .Include(x => x.Categoria)
        .FirstOrDefault(x => x.Id == id);

    if (livro == null)
    {
        return Results.NotFound($"Livro com ID {id} não encontrado.");
    }

    return Results.Ok(livro);
});

//4. Atualizar Livro
app.MapPut("/api/livros/{id}", ([FromRoute] int id,
    [FromBody] Livro livroAlterado,
    [FromServices] BibliotecaDbContext context) =>
{
    //verificando se livro existe
    var livro = context.Livros.Find(id);
    if (livro == null)
        return Results.NotFound($"Livro com ID {id} não encontrado para atualização.");

    //Verificando caracteres
    if (livroAlterado.Titulo.Length < 3)
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");

    if (livroAlterado.Autor.Length < 3)
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");

    //Verificando categoria
    var categoria = context.Categorias.Find(livroAlterado.CategoriaId);
    if (categoria is null)
        return Results.BadRequest("Categoria inválida. O ID da categoria fornecido não existe.");

    livro.Titulo = livroAlterado.Titulo;
    livro.Autor = livroAlterado.Autor;
    livro.CategoriaId = livroAlterado.CategoriaId;
    livro.Categoria = categoria;

    context.SaveChanges();
    return Results.Ok(livro);
});


// 5. Remover Livro
app.MapDelete("/api/livros/{id}", ([FromRoute] int id, [FromServices] BibliotecaDbContext context) =>
{
    var livro = context.Livros.Find(id);
    if (livro == null)
    {
        return Results.NotFound($"Livro com ID {id} não encontrado para remoção.");
    }

    context.Livros.Remove(livro);
    context.SaveChanges();

    return Results.NoContent();
});


app.Run();


