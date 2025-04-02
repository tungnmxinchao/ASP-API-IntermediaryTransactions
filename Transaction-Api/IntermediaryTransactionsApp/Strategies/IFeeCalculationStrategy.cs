namespace IntermediaryTransactionsApp.Strategies
{
    public interface IFeeCalculationStrategy
    {
        decimal CalculateFee(decimal amount);
        decimal CalculateTotalForBuyer(decimal amount, bool isSellerChargeFee);
        decimal CalculateSellerReceived(decimal amount, bool isSellerChargeFee);
    }
} 