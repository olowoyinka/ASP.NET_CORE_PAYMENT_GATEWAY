using Braintree;

namespace Payment_Gateway.Services
{
    public interface IBraintreeService
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();
    }
}