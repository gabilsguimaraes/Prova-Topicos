using Microsoft.EntityFrameworkCore;

namespace API.Modelos;

public class BibliotecaDbContext : DbContext
{
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=biblioteca.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria() { Id = 1, Nome = "Poesia" },
            new Categoria() { Id = 2, Nome = "Romance" },
            new Categoria() { Id = 3, Nome = "Fantasia" },
            new Categoria() { Id = 4, Nome = "Ficção Científica" },
            new Categoria() { Id = 5, Nome = "Literatura Infantil" }
        );
    }

}