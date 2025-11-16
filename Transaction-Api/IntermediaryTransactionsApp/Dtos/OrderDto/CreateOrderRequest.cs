using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.OrderDto
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập thông tin liên hệ.")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề giao dịch.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả giao dịch.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chế độ hiển thị (công khai/ẩn).")]
        public bool IsPublic { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung ẩn.")]
        public string HiddenValue { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền giao dịch.")]
        public decimal MoneyValue { get; set; }

        [Required(ErrorMessage = "Vui lòng xác định bên chịu phí.")]
        public bool IsSellerChargeFee { get; set; }
    }
}
