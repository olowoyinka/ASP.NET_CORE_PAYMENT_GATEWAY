using Braintree;
using Microsoft.AspNetCore.Mvc;
using Payment_Gateway.Services;
using Payment_Gateway.ViewModels;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Payment_Gateway.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBookService _courseService;

        private readonly IBraintreeService _braintreeService;

        public CheckoutController(IBookService courseService, IBraintreeService braintreeService)
        {
            _courseService = courseService;
            _braintreeService = braintreeService;
        }

        public IActionResult PurchaseStripe(Guid id)
        {
            var book = _courseService.GetById(id);

            ViewBag.PurchaseAmount = book.Price;

            if (book == null) return NotFound();

            return View(book);
        }

        [HttpPost]
        public IActionResult CreateStripe(string stripeToken, Guid id)
        {
            var book = _courseService.GetById(id);

            var chargeOptions = new ChargeCreateOptions()
            {
                Amount = (long)(Convert.ToDouble(book.Price) * 100),
                Currency = "usd",
                Source = stripeToken,
                Metadata = new Dictionary<string, string>()
                {
                    {"BookId", book.Id.ToString() },
                    {"BookTitle", book.Title },
                    {"BookAuthor", book.Author }
                }
            };

            var service = new ChargeService();

            Charge charge = service.Create(chargeOptions);

            if (charge.Status == "succeeded")
            {
                return View("Success");
            }

            return View("Failure");
        }

        public IActionResult LoadAllStripePlans()
        {
            var service = new PlanService();

            var allPlans = service.List().ToList();

            return View(allPlans);
        }

        public IActionResult SubscribeToStripePlan(string id)
        {
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = "cus_I7v9x4UO9f8CqQ",
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = id
                    }
                }
            };

            var service = new SubscriptionService();

            Stripe.Subscription subscription = service.Create(subscriptionOptions);

            if (subscription.Created != null)
            {
                return View("Subscribed");
            }

            return View("NotSubscribed");
        }

        // Payment with Braintree
        public IActionResult Create(BookPurchaseVM model)
        {
            var gateway = _braintreeService.GetGateway();

            var book = _courseService.GetById(model.Id);

            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(book.Price),
                PaymentMethodNonce = model.Nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true,
                    PayeeEmail = "olowofelayinka@gmail.com",
                    PayeeId = "45673"
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                return View("Success");
            }
            else
            {
                return View("Failure");
            }
        }

        public IActionResult BraintreePlans()
        {
            var gateway = _braintreeService.GetGateway();

            var plans = gateway.Plan.All();

            return View(plans);
        }

        public IActionResult SubscribeToPlan(string id)
        {
            var gateway = _braintreeService.GetGateway();

            var subscriptionRequest = new SubscriptionRequest()
            {
                PaymentMethodToken = "my-payment-token-value",
                PlanId = id
            };

            Result<Braintree.Subscription> result = gateway.Subscription.Create(subscriptionRequest);

            if (result.IsSuccess())
            {
                return View("Subscribed");
            }
            return View("NotSubscribed");
        }

        public IActionResult Purchase(Guid id)
        {

            var book = _courseService.GetById(id);

            if (book == null) return NotFound();

            var gateway = _braintreeService.GetGateway();

            var clientToken = gateway.ClientToken.Generate();

            ViewBag.ClientToken = clientToken;

            var data = new BookPurchaseVM
            {
                Id = book.Id,
                Description = book.Description,
                Author = book.Author,
                Thumbnail = book.Thumbnail,
                Title = book.Title,
                Price = book.Price,
                Nonce = ""
            };

            return View(data);
        }
    }
}