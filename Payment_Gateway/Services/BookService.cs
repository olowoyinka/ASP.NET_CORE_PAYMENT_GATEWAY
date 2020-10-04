using Payment_Gateway.Data;
using Payment_Gateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payment_Gateway.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Book> GetAll()
        {
            var items = _context.Books.ToList();

            return items;
        }

        public Book GetById(Guid courseId)
        {
            return _context.Books.FirstOrDefault(n => n.Id == courseId);
        }
    }
}