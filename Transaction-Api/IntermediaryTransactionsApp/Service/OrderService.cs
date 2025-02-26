using AutoMapper;
using IntermediaryTransactionsApp.Constants;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Exceptions;
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
				throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
			}

			await _unitOfWorkCreateOrder.BeginTransactionAsync();

			try
			{
				var order = CreateOrderEntity(request, (int) userId);

				await _context.Orders.AddAsync(order);

				await UpdateUserBalance((int)userId);

				await NotifyUser((int)userId, order.Id);

				await RecordHistory((int)userId, order.Id);

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

		private Order CreateOrderEntity(CreateOrderRequest request, int userId)
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

			return order;
		}

		private async Task NotifyUser(int userId, Guid orderId)
		{
			var messageRequest = new CreateMessageRequest
			{
				Subject = $"Hệ thống đã ghi nhận yêu trung gian mã số: {orderId}",
				Content = $"Hệ thống đã ghi nhận yêu cầu trung gian mã số: {orderId}!\r\nVui lòng nhấn \"CHI TIẾT\" để xem chi tiết yêu cầu trung gian",
				UserId = userId
			};

			await _messageService.CreateMessage(messageRequest);
		}

		private async Task UpdateUserBalance(int userId)
		{
			var updateMoney = new UpdateMoneyRequest
			{
				UserId = userId,
				Money = Constants.Constants.FeeAddNewOrder
			};

			await _userService.UpdateMoney(updateMoney);
		}

		private async Task RecordHistory(int userId, Guid orderId)
		{
			var historyRequest = new CreateHistoryRequest
			{
				Amount = Constants.Constants.FeeAddNewOrder,
				TransactionType = 2,
				Note = $"Thu phí tạo yêu cầu trung gian mã số: {orderId}",
				Payload = "Giao dịch thành công",
				UserId = userId,
				OnDoneLink = "Example@gmail.com"
			};

			await _historyService.CreateHistory(historyRequest);
		}


		public async Task<UpdateOrderResponse> UpdateOrder(UpdateOrderRequest request)
		{
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            var order = _context.Orders.FirstOrDefault(x => x.Id == request.OrderId);

			if(order == null)
			{
				throw new ObjectNotFoundException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFound));

			}

			if(order.CreatedBy != userId || order.Updateable == false)
			{
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.NotHavePermisson));
            }

			UpdateOrderEntity(order, request);

			_context.Update(order);
			await _context.SaveChangesAsync();

			var orderResponse = _mapper.Map<UpdateOrderResponse>(order);

			return orderResponse;

		}

		private void UpdateOrderEntity(Order order, UpdateOrderRequest request)
		{
			decimal feeOnSuccess = request.MoneyValue * 0.05m;
			decimal totalMoneyForBuyer = request.IsSellerChargeFee
				? request.MoneyValue + feeOnSuccess
				: request.MoneyValue;
			decimal sellerReceivedOnSuccess = request.IsSellerChargeFee
				? request.MoneyValue
				: request.MoneyValue - feeOnSuccess;

			order.Title = request.Title;
			order.Description = request.Description;
			order.IsPublic = request.IsPublic;
			order.HiddenValue = request.HiddenValue;
			order.MoneyValue = request.MoneyValue;
			order.IsSellerChargeFee = request.IsSellerChargeFee;
			order.UpdatedAt = DateTime.Now;
			order.FeeOnSuccess = feeOnSuccess;
			order.TotalMoneyForBuyer = totalMoneyForBuyer;
			order.SellerReceivedOnSuccess = sellerReceivedOnSuccess;
		}
	}
}
