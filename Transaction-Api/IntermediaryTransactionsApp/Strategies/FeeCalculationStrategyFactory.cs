namespace IntermediaryTransactionsApp.Strategies
{
    public enum FeeStrategyType
    {
        Percentage,
        Fixed,
        Tiered
    }

    public interface IFeeCalculationStrategyFactory
    {
        IFeeCalculationStrategy CreateStrategy(FeeStrategyType type);
    }

    public class FeeCalculationStrategyFactory : IFeeCalculationStrategyFactory
    {
        public IFeeCalculationStrategy CreateStrategy(FeeStrategyType type)
        {
            return type switch
            {
                FeeStrategyType.Percentage => new PercentageFeeStrategy(),
                FeeStrategyType.Fixed => new FixedFeeStrategy(),
                FeeStrategyType.Tiered => new TieredFeeStrategy(),
                _ => throw new ArgumentException($"Unsupported fee strategy type: {type}")
            };
        }
    }
} 