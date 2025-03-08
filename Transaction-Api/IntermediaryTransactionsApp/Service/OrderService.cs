using AutoMapper;
using IntermediaryTransactionsApp.Commands;
using IntermediaryTransactionsApp.Constants;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Events;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using IntermediaryTransactionsApp.Interface.IOrderService;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.Specifications;
using IntermediaryTransactionsApp.UnitOfWork;
using Microsoft.EntityFrameworkCore;

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
		private readonly IFeeCalculationService _feeCalculationService;
		private readonly IOrderEventDispatcher _eventDispatcher;

		public OrderService(ApplicationDbContext context, IUserService userService, 
			IMessageService messageService, IMapper mapper, IHistoryService historyService,
			IUnitOfWorkPersistDb unitOfWorkCreateOrder, JwtService jwtService,
			IFeeCalculationService feeCalculationService, IOrderEventDispatcher eventDispatcher)
		{
			_context = context;
			_userService = userService;
			_messageService = messageService;
			_mapper = mapper;
			_historyService = historyService;
			_unitOfWorkDb = unitOfWorkCreateOrder;
			_jwtService = jwtService;
			_feeCalculationService = feeCalculationService;
			_eventDispatcher = eventDispatcher;
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

			var command = new CreateOrderCommand(
				request,
				(int)userId,
				_context,
				_userService,
				_messageService,
				_historyService,
				_unitOfWorkDb,
				_feeCalculationService,
				_mapper);

			var order = await command.ExecuteAsync();

			if (order != null)
			{
				return true;
			}

			return false;
		}

		public async Task<UpdateOrderResponse> UpdateOrder(UpdateOrderRequest request)
		{
			var userId = _jwtService.GetUserIdFromToken();
			if (userId == null)
			{
				throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
			}

			var specification = new OrderUpdateableSpecification(request.OrderId, (int)userId);
			var order = await ApplySpecification(specification).FirstOrDefaultAsync();

			if(order == null)
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
			var feeOnSuccess = _feeCalculationService.CalculateFee(request.MoneyValue);
			var totalMoneyForBuyer = _feeCalculationService.CalculateTotalForBuyer(request.MoneyValue, request.IsSellerChargeFee);
			var sellerReceivedOnSuccess = _feeCalculationService.CalculateSellerReceived(request.MoneyValue, request.IsSellerChargeFee);

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

		public async Task<bool> BuyOrder(BuyOrderRequest request)
		{
			var userId = _jwtService.GetUserIdFromToken();
			if (userId == null)
			{
				throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
			}

			var specification = new OrderBuyableSpecification(request.OrderId, (int)userId);
			var order = await ApplySpecification(specification).FirstOrDefaultAsync();

			if (order == null)
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

			var command = new BuyOrderCommand(
				request,
				(int)userId,
				order,
				_context,
				_userService,
				_messageService,
				_historyService,
				_unitOfWorkDb);

			var result = await command.ExecuteAsync();

			if (result)
			{
				await _eventDispatcher.DispatchAsync(new OrderBoughtEvent(order.Id, (int)userId));
			}

			return result;
		}

        public async Task<bool> CompleteOrder(Guid orderId)
        {
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            var specification = new OrderCompletableSpecification(orderId, (int)userId);
            var order = await ApplySpecification(specification).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.NotHavePermisson));
            }

            var command = new CompleteOrderCommand(
                _historyService,
                _userService,
                order,
                _context,
                _messageService,
                _unitOfWorkDb);             

            var result = await command.ExecuteAsync();

            if(result)
            {
                return true;
            }

            return false;

        }       

        public async Task<bool> ComplainOrder(Guid orderId)
        {
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            var specification = new OrderComplaintSpecification(orderId, (int)userId);
            var order = await ApplySpecification(specification).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.NotHavePermisson));
            }


            var command = new ComplainOrderCommand(
                order,
                _context,
                _messageService,
                _unitOfWorkDb);

            var result = await command.ExecuteAsync();

            if (result)
            {
                return true;
            }

            return false;

        }

        public async Task<bool> RequestBuyerCheckOrder(Guid orderId)
        {
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            var specification = new RequestBuyerCheckOrder(orderId, (int)userId);
            var order = await ApplySpecification(specification).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.NotHavePermisson));
            }

            var command = new RequestBuyerCheckOrderCommand(
                order,
                _context,
                _messageService,
                _unitOfWorkDb);

            var result = await command.ExecuteAsync();

            if (result)
            {
                return true;
            }

            return false;

        }

        public async Task<bool> CallAdmin(Guid orderId)
        {
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null || _userService.CheckBalanceUserWithMoney(Constants.Constants.FeeCallAdmin, (int) userId))
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            var specification = new CallAdminHandleOrder(orderId);
            var order = await ApplySpecification(specification).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.NotHavePermisson));
            }


            await _unitOfWorkDb.BeginTransactionAsync();

            var command = new CallAdminCommand(
                order,
                _context,
                _messageService,
                _unitOfWorkDb);

            var result = await command.ExecuteAsync();

            if (result)
            {
                return true;
            }

            return false;

        }

        public async Task<bool> CancelOrder(Guid orderId)
        {
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            var specification = new CancelOrder(orderId, (int) userId);
            var order = await ApplySpecification(specification).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.NotHavePermisson));
            }


            var command = new SellerCancelOrderCommand(
				_historyService,
				_userService,
                order,
                _context,
                _messageService,
                _unitOfWorkDb);

            var result = await command.ExecuteAsync();

            if (result)
            {
                return true;
            }

            return false;

        }



        private IQueryable<Order> ApplySpecification(ISpecification<Order> spec)
		{
			return SpecificationEvaluator<Order>.GetQuery(_context.Orders.AsQueryable(), spec);
		}
	}
}
