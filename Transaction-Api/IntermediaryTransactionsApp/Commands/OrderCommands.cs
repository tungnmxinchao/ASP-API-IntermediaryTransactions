using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.Service;
using IntermediaryTransactionsApp.Strategies;
using IntermediaryTransactionsApp.UnitOfWork;

namespace IntermediaryTransactionsApp.Commands
{
    public class CreateOrderCommand : ICommand<Order>
    {
        private readonly CreateOrderRequest _request;
        private readonly int _userId;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IHistoryService _historyService;
        private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
        private readonly IFeeCalculationService _feeCalculationService;
        private readonly IMapper _mapper;

        public CreateOrderCommand(
            CreateOrderRequest request,
            int userId,
            ApplicationDbContext context,
            IUserService userService,
            IMessageService messageService,
            IHistoryService historyService,
            IUnitOfWorkPersistDb unitOfWorkDb,
            IFeeCalculationService feeCalculationService,
            IMapper mapper)
        {
            _request = request;
            _userId = userId;
            _context = context;
            _userService = userService;
            _messageService = messageService;
            _historyService = historyService;
            _unitOfWorkDb = unitOfWorkDb;
            _feeCalculationService = feeCalculationService;
            _mapper = mapper;
        }

        public async Task<Order> ExecuteAsync()
        {
            await _unitOfWorkDb.BeginTransactionAsync();

            try
            {
                var order = CreateOrderEntity();

                await _context.Orders.AddAsync(order);

                await UpdateUserBalance();

                await NotifyUser(order.Id);

                await RecordHistory(order.Id);

                await _unitOfWorkDb.SaveChangesAsync();

                await _unitOfWorkDb.CommitAsync();

                return order;
            }
            catch (Exception)
            {
                await _unitOfWorkDb.RollbackAsync();
                throw;
            }
        }

        private Order CreateOrderEntity()
        {
            var feeOnSuccess = _feeCalculationService.CalculateFee(_request.MoneyValue);
            var totalMoneyForBuyer = _feeCalculationService.CalculateTotalForBuyer(_request.MoneyValue, _request.IsSellerChargeFee);
            var sellerReceivedOnSuccess = _feeCalculationService.CalculateSellerReceived(_request.MoneyValue, _request.IsSellerChargeFee);

            var order = _mapper.Map<Order>(_request);
            order.CreatedBy = _userId;
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

        private async Task UpdateUserBalance()
        {
            var updateMoney = new UpdateMoneyRequest
            {
                UserId = _userId,
                Money = Constants.Constants.FeeAddNewOrder
            };

            await _userService.UpdateMoney(updateMoney);
        }

        private async Task NotifyUser(Guid orderId)
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Hệ thống đã ghi nhận yêu trung gian mã số: {orderId}",
                Content = $"Hệ thống đã ghi nhận yêu cầu trung gian mã số: {orderId}!\r\nVui lòng nhấn \"CHI TIẾT\" để xem chi tiết yêu cầu trung gian",
                UserId = _userId
            };

            await _messageService.CreateMessage(messageRequest);
        }

        private async Task RecordHistory(Guid orderId)
        {
            var historyRequest = new CreateHistoryRequest
            {
                Amount = Constants.Constants.FeeAddNewOrder,
                TransactionType = 2,
                Note = $"Thu phí tạo yêu cầu trung gian mã số: {orderId}",
                Payload = "Giao dịch thành công",
                UserId = _userId,
                OnDoneLink = "Example@gmail.com"
            };

            await _historyService.CreateHistory(historyRequest);
        }
    }

    public class BuyOrderCommand : ICommand<bool>
    {
        private readonly BuyOrderRequest _request;
        private readonly int _userId;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IHistoryService _historyService;
        private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
        private readonly Order _order;

        public BuyOrderCommand(
            BuyOrderRequest request,
            int userId,
            Order order,
            ApplicationDbContext context,
            IUserService userService,
            IMessageService messageService,
            IHistoryService historyService,
            IUnitOfWorkPersistDb unitOfWorkDb)
        {
            _request = request;
            _userId = userId;
            _order = order;
            _context = context;
            _userService = userService;
            _messageService = messageService;
            _historyService = historyService;
            _unitOfWorkDb = unitOfWorkDb;
        }

        public async Task<bool> ExecuteAsync()
        {
            await _unitOfWorkDb.BeginTransactionAsync();

            try
            {
                await UpdateUserBalance();

                await UpdateSellerBalance();

                UpdateOrderStatus();

                await NotifyBuyer();

                await RecordHistory();

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

        private async Task UpdateUserBalance()
        {
            var updateMoney = new UpdateMoneyRequest
            {
                UserId = _userId,
                Money = _order.TotalMoneyForBuyer
            };

            await _userService.UpdateMoney(updateMoney);
        }

        private async Task UpdateSellerBalance()
        {
            var updateMoney = new UpdateMoneyRequest
            {
                UserId = _order.CreatedBy,
                Money = _order.SellerReceivedOnSuccess
            };

            await _userService.UpdateMoney(updateMoney);
        }

        private void UpdateOrderStatus()
        {
            _order.Customer = _userId;
            _order.StatusId = 3;
            _order.Updateable = false;
        }

        private async Task NotifyBuyer()
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Hoàn thành xử lý thanh toán giao dịch mã số: {_order.Id}",
                Content = $"Trạng thái giao dịch: Thành công\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = _userId
            };

            await _messageService.CreateMessage(messageRequest);
        }

        private async Task RecordHistory()
        {
            var historyRequest = new CreateHistoryRequest
            {
                Amount = Constants.Constants.FeeAddNewOrder,
                TransactionType = 2,
                Note = $"Thu phí thực hiện cầu trung gian mã số: {_order.Id}",
                Payload = "Giao dịch thành công",
                UserId = _userId,
                OnDoneLink = "Example@gmail.com"
            };

            await _historyService.CreateHistory(historyRequest);
        }
    }
} 