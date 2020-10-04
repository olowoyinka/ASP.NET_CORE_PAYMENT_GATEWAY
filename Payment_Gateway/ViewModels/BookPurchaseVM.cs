using Payment_Gateway.Models;

namespace Payment_Gateway.ViewModels
{
    public class BookPurchaseVM : Book
    {
        public string Nonce { get; set; }
    }
}