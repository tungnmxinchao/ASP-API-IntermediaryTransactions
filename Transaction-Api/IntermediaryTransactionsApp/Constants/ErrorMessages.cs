namespace IntermediaryTransactionsApp.Constants
{
    public enum ErrorMessages
    {
        InvalidCredentials,
        ObjectNotFound,
        ObjectNotFoundInToken,
        NotHavePermisson,
        BalanceNotEnough,
    }

    public static class ErrorMessageExtensions
    {
        public static string GetMessage(this ErrorMessages error)
        {
            return error switch
            {
                ErrorMessages.InvalidCredentials => "Sai toàn khoản hoặc mật khẩu.",
                ErrorMessages.ObjectNotFound => "Không tìm thấy bản ghi.",
                ErrorMessages.ObjectNotFoundInToken => "Không tìn thấy thông tin người dùng.",
                ErrorMessages.NotHavePermisson => "Bạn không có quyền để thực hiện hành động.",
                ErrorMessages.BalanceNotEnough => "Số dư của bản không đủ để thực hiện.",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
