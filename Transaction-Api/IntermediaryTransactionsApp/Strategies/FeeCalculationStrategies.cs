namespace IntermediaryTransactionsApp.Strategies
{
    public class PercentageFeeStrategy : IFeeCalculationStrategy
    {
        private readonly decimal _percentage;

        public PercentageFeeStrategy(decimal percentage = 0.05m)
        {
            _percentage = percentage;
        }

        public decimal CalculateFee(decimal amount)
        {
            return amount * _percentage;
        }

        public decimal CalculateTotalForBuyer(decimal amount, bool isSellerChargeFee)
        {
            var fee = CalculateFee(amount);
            return isSellerChargeFee ? amount  : amount + fee;
        }

        public decimal CalculateSellerReceived(decimal amount, bool isSellerChargeFee)
        {
            var fee = CalculateFee(amount);
            return isSellerChargeFee ? amount - fee : amount;
        }
    }

    public class FixedFeeStrategy : IFeeCalculationStrategy
    {
        private readonly decimal _fixedFee;

        public FixedFeeStrategy(decimal fixedFee = 1000m)
        {
            _fixedFee = fixedFee;
        }

        public decimal CalculateFee(decimal amount)
        {
            return _fixedFee;
        }

        public decimal CalculateTotalForBuyer(decimal amount, bool isSellerChargeFee)
        {
            return isSellerChargeFee ? amount + _fixedFee : amount;
        }

        public decimal CalculateSellerReceived(decimal amount, bool isSellerChargeFee)
        {
            return isSellerChargeFee ? amount : amount - _fixedFee;
        }
    }

    public class TieredFeeStrategy : IFeeCalculationStrategy
    {
        private readonly decimal _lowTierPercentage = 0.03m;
        private readonly decimal _highTierPercentage = 0.05m;
        private readonly decimal _tierThreshold = 1000000m;

        public decimal CalculateFee(decimal amount)
        {
            return amount <= _tierThreshold 
                ? amount * _lowTierPercentage 
                : amount * _highTierPercentage;
        }

        public decimal CalculateTotalForBuyer(decimal amount, bool isSellerChargeFee)
        {
            var fee = CalculateFee(amount);
            return isSellerChargeFee ? amount + fee : amount;
        }

        public decimal CalculateSellerReceived(decimal amount, bool isSellerChargeFee)
        {
            var fee = CalculateFee(amount);
            return isSellerChargeFee ? amount : amount - fee;
        }
    }
} 