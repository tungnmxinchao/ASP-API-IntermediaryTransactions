using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using IntermediaryTransactionsApp.Interface.IOrderService;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.UnitOfWork;


namespace IntermediaryTransactionsApp.Service
{
	public class OrderService : IOrderService
	{
		private readonly ApplicationDbContext _context;
		private readonly IUserService _userService;
		private readonly IMessageService _messageService;
		private readonly IMapper _mapper;
		private readonly IHistoryService _historyService;
		private readonly IUnitOfWorkCreateOrder _unitOfWorkCreateOrder;
		private readonly JwtService _jwtService;

		public OrderService(ApplicationDbContext context, IUserService userService, 
			IMessageService messageService, IMapper mapper, IHistoryService historyService,
			IUnitOfWorkCreateOrder unitOfWorkCreateOrder, JwtService jwtService)
		{
			_context = context;
			_userService = userService;
			_messageService = messageService;
			_mapper = mapper;
			_historyService = historyService;
			_unitOfWorkCreateOrder = unitOfWorkCreateOrder;
			_jwtService = jwtService;
		}

		public async Task<bool> CreateOrder(CreateOrderRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var userId = _jwtService.GetUserIdFromToken();
			if (userId == null)
			{
				throw new UnauthorizedAccessException("User ID not found in token.");
			}

			await _unitOfWorkCreateOrder.BeginTransactionAsync();

			try
			{
				decimal feeOnSuccess = request.MoneyValue * 0.05m;
				decimal totalMoneyForBuyer = request.IsSellerChargeFee ? request.MoneyValue + feeOnSuccess : request.MoneyValue;
				decimal sellerReceivedOnSuccess = request.IsSellerChargeFee ? request.MoneyValue : request.MoneyValue - feeOnSuccess;

				var order = _mapper.Map<Order>(request);
				order.CreatedBy = (int)userId;
				order.StatusId = 1;
				order.IsPaidToSeller = true;
				order.ShareLink = Guid.NewGuid().ToString();
				order.FeeOnSuccess = feeOnSuccess;
				order.TotalMoneyForBuyer = totalMoneyForBuyer;
				order.SellerReceivedOnSuccess = sellerReceivedOnSuccess;
				order.Updateable = true;
				order.CustomerCanComplain = true;

				await _context.Orders.AddAsync(order);

				UpdateMoneyRequest updateMoney = new UpdateMoneyRequest
				{
					UserId = (int)userId,
					Money = Constants.Constants.FeeAddNewOrder
				};

				await _userService.UpdateMoney(updateMoney);

				CreateMessageRequest messageRequest = new CreateMessageRequest
				{
					Subject = $"Hệ thống đã ghi nhận yêu trung gian mã số: {order.Id}",
					Content = $"Hệ thống đã ghi nhận yêu cầu trung gian mã số: {order.Id}!\r\nVui lòng nhấn \"CHI TIẾT\" để xem chi tiết yêu cầu trung gian",
					UserId = (int)userId,

				};

				var createdMessage = await _messageService.CreateMessage(messageRequest);

				CreateHistoryRequest historyRequest = new CreateHistoryRequest
				{
					Amount = Constants.Constants.FeeAddNewOrder,
					TransactionType = 2,
					Note = $"Thu phí tạo yêu cầu trung gian mã số: {order.Id}",
					Payload = "Giao dịch thành công",
					UserId = (int)userId,
					OnDoneLink = "Example@gmail.com"
				};

				var createHistoryTransactions = await _historyService.CreateHistory(historyRequest);

				await _unitOfWorkCreateOrder.SaveChangesAsync();

				await _unitOfWorkCreateOrder.CommitAsync();
				return true;
			}
			catch (Exception)
			{
				await _unitOfWorkCreateOrder.RollbackAsync();
				throw;
			}
		}
	}
}
