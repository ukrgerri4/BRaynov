using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Application.Logic.Books.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, GetAllBooksViewModel>
    {
        private readonly ILogger<GetAllBooksQueryHandler> logger;

        public GetAllBooksQueryHandler(ILogger<GetAllBooksQueryHandler> logger)
        {
			this.logger = logger;
        }

        public async Task<GetAllBooksViewModel> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
			throw new NotImplementedException();
        }
    }
}
