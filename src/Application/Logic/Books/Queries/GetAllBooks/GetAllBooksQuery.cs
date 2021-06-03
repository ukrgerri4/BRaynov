﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logic.Books.Queries.GetAllBooks
{
    public class GetAllBooksQuery : IRequest<GetAllBooksViewModel>
    {
    }
}
