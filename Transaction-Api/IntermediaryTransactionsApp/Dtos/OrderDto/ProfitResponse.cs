namespace IntermediaryTransactionsApp.Dtos.OrderDto
{
    public class ProfitResponse
    {
        public int TotalOrder { get; set; }

        public decimal ProfitOfCreateOrder { get; set; }

        public decimal ProfitOfFeeOrder { get;set; }
    }
}
