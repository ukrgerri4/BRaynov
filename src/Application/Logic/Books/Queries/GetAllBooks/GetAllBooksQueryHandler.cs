using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Logic.Books.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, GetAllBooksViewModel>
    {
        private readonly ILogger<GetAllBooksQueryHandler> logger;
        private readonly IAppDbContext appDbContext;

        public GetAllBooksQueryHandler(ILogger<GetAllBooksQueryHandler> logger, IAppDbContext appDbContext)
        {
			this.logger = logger;
            this.appDbContext = appDbContext;
        }

        public async Task<GetAllBooksViewModel> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            return new GetAllBooksViewModel
            {
                Books = new BookViewModel[]
                {
                    new BookViewModel { Name = "Book1" },
                    new BookViewModel { Name = "Book2" },
                    new BookViewModel { Name = "Book3" },
                    new BookViewModel { Name = "Book4" },
                    new BookViewModel { Name = "Book5" },
                }
            };
        }
    }
}
