using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.Services;
using System;

namespace Payment_Gateway.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var items = _service.GetAll();

            return View(items);
        }

        public IActionResult Details(Guid id)
        {
            var book = _service.GetById(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
    }
}