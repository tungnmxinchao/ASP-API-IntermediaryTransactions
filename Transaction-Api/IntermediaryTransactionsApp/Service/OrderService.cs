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
		private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
		private readonly JwtService _jwtService;

		public OrderService(ApplicationDbContext context, IUserService userService, 
			IMessageService messageService, IMapper mapper, IHistoryService historyService,
			IUnitOfWorkPersistDb unitOfWorkCreateOrder, JwtService jwtService)
		{
			_context = context;
			_userService = userService;
			_messageService = messageService;
			_mapper = mapper;
			_historyService = historyService;
			_unitOfWorkDb = unitOfWorkCreateOrder;
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

			await _unitOfWorkDb.BeginTransactionAsync();

			try
			{
				var order = CreateOrderEntity(request, (int) userId);

				await _context.Orders.AddAsync(order);

				await UpdateUserBalance((int)userId, Constants.Constants.FeeAddNewOrder);

				await NotifyUser((int)userId, order.Id);

				await RecordHistory((int)userId, order.Id);

				await _unitOfWorkDb.SaveChangesAsync();

				await _unitOfWorkDb.CommitAsync();

				return true;
			}
			catch (Exception)
			{
				await _unitOfWorkDb.RollbackAsync();
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

		private async Task UpdateUserBalance(int userId, decimal money)
		{
			var updateMoney = new UpdateMoneyRequest
			{
				UserId = userId,
				Money = money
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

        private async Task NotifyBuyer(int userId, Guid orderId)
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Hoàn thành xử lý thanh toán giao dịch mã số: {orderId}",
                Content = $"Trạng thái giao dịch: Thành công\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = userId
            };

            await _messageService.CreateMessage(messageRequest);
        }

        private async Task RecordHistoryBuyAction(int userId, Guid orderId)
        {
            var historyRequest = new CreateHistoryRequest
            {
                Amount = Constants.Constants.FeeAddNewOrder,
                TransactionType = 2,
                Note = $"Thu phí thực hiện cầu trung gian mã số: {orderId}",
                Payload = "Giao dịch thành công",
                UserId = userId,
                OnDoneLink = "Example@gmail.com"
            };

            await _historyService.CreateHistory(historyRequest);
        }

		public void UpdateOrderAfterBuy(Order order, int status, int buyerId)
		{
			order.Customer = buyerId;
            order.StatusId = status;
			order.Updateable = false;

        }


        public  async Task<bool> BuyOrder(BuyOrderRequest request)
		{
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            var order = _context.Orders.FirstOrDefault(x => x.Id == request.OrderId);

            if (order == null)
            {
                throw new ObjectNotFoundException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFound));

            }

			if (order.CreatedBy == userId)
			{
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.NotHavePermisson));
            }

			var user = _context.Users.Find(userId);

			if (user == null)
			{
                throw new ObjectNotFoundException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFound));

            }

			if(user.Money < order.TotalMoneyForBuyer)
			{
				throw new Exception(ErrorMessageExtensions.GetMessage(ErrorMessages.BalanceNotEnough));
			}

            await _unitOfWorkDb.BeginTransactionAsync();

			try
			{
                await UpdateUserBalance((int)userId, order.TotalMoneyForBuyer);

                await UpdateUserBalance(order.CreatedBy, order.SellerReceivedOnSuccess);

				UpdateOrderAfterBuy(order, 3, (int)userId);

                await NotifyBuyer((int)userId, order.Id);

                await RecordHistory((int)userId, order.Id);

                await _unitOfWorkDb.SaveChangesAsync();

                await _unitOfWorkDb.CommitAsync();

				return true;
				
            }
            catch (Exception)
            {
                await _unitOfWorkDb.RollbackAsync();
                throw;
            }

        }

	

		

	}
}
