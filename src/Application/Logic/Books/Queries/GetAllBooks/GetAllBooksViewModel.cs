namespace Application.Logic.Books.Queries.GetAllBooks
{
    public class GetAllBooksViewModel
    {
        public BookViewModel[] Books { get; set; }
    }

    public class BookViewModel
    {
        public string Name { get; set; }
    }
}
