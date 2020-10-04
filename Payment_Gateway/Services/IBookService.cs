using Payment_Gateway.Models;
using System;
using System.Collections.Generic;

namespace Payment_Gateway.Services
{
    public interface IBookService
    {
        IEnumerable<Book> GetAll();

        Book GetById(Guid courseId);
    }
}