using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.UserDto
{
	public class UpdateMoneyRequest
	{
		[Required]
		public int UserId { get; set; }
		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Money must be greater than 0.")]
		public decimal Money {  get; set; }

        [Required]
        public int TypeUpdate {  get; set; }

	}
}
