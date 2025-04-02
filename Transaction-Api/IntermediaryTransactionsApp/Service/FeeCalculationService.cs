using IntermediaryTransactionsApp.Strategies;

namespace IntermediaryTransactionsApp.Service
{
    public interface IFeeCalculationService
    {
        decimal CalculateFee(decimal amount);
        decimal CalculateTotalForBuyer(decimal amount, bool isSellerChargeFee);
        decimal CalculateSellerReceived(decimal amount, bool isSellerChargeFee);
    }

    public class FeeCalculationService : IFeeCalculationService
    {
        private readonly IFeeCalculationStrategy _strategy;

        public FeeCalculationService(IFeeCalculationStrategy strategy)
        {
            _strategy = strategy;
        }

        public decimal CalculateFee(decimal amount)
        {
            return _strategy.CalculateFee(amount);
        }

        public decimal CalculateTotalForBuyer(decimal amount, bool isSellerChargeFee)
        {
            return _strategy.CalculateTotalForBuyer(amount, isSellerChargeFee);
        }

        public decimal CalculateSellerReceived(decimal amount, bool isSellerChargeFee)
        {
            return _strategy.CalculateSellerReceived(amount, isSellerChargeFee);
        }
    }
} 