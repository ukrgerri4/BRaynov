using Application.Logic.Books.Queries.GetAllBooks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Controllers.Common;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
