public class Livro
{
    public int Id { get; set; }

    public string Titulo { get; set; }

    public string Autor { get; set; }

    public int CategoriaId { get; set; }

    public Categoria Categoria { get; set; }
}