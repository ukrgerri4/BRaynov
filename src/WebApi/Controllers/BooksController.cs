using Application.Logic.Books.Queries.GetAllBooks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Controllers.Common;

namespace WebApi.Controllers
{
    public class BooksController : BaseController
    {
        public BooksController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<ActionResult<GetAllBooksViewModel>> Select()
        {
            return await mediator.Send(new GetAllBooksQuery());
        }
    }
}
