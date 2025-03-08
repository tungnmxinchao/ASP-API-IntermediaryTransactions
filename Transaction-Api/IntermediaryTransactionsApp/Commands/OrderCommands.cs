using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Enum;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.Service;
using IntermediaryTransactionsApp.State;
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
                Money = Constants.Constants.FeeAddNewOrder,
                TypeUpdate = (int) UpdateMoneyMode.SubMoney
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
                TransactionType = (int) UpdateMoneyMode.SubMoney,
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

                UpdateOrderStatus();

                await NotifyBuyer();

                await RecordHistory(_order);

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
                Money = _order.TotalMoneyForBuyer,
                TypeUpdate = (int) UpdateMoneyMode.SubMoney 
            };

            await _userService.UpdateMoney(updateMoney);
        }

        private void UpdateOrderStatus()
        {
            _order.Customer = _userId;
            _order.StatusId = (int) OrderState.BuyerInspecting;
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

        private async Task RecordHistory(Order order)
        {
            var historyRequest = new CreateHistoryRequest
            {
                Amount = order.TotalMoneyForBuyer,
                TransactionType = (int) UpdateMoneyMode.SubMoney,
                Note = $"Thu phí thực hiện mua đơn hàng mã số: {_order.Id}",
                Payload = "Giao dịch thành công",
                UserId = _userId,
                OnDoneLink = "Example@gmail.com"
            };

            await _historyService.CreateHistory(historyRequest);
        }
    }

    public class CompleteOrderCommand : ICommand<bool>
    {

        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;
        private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
        private readonly Order _order;
        private readonly IUserService _userService;
        private readonly IHistoryService _historyService;

        public CompleteOrderCommand(
            IHistoryService historyService,
            IUserService userService,
            Order order,
            ApplicationDbContext context,
            IMessageService messageService,
            IUnitOfWorkPersistDb unitOfWorkDb)
        {
            _historyService = historyService;
            _userService = userService;
            _order = order;
            _context = context;
            _messageService = messageService;
            _unitOfWorkDb = unitOfWorkDb;
        }

        public async Task<bool> ExecuteAsync()
        {
            await _unitOfWorkDb.BeginTransactionAsync();

            try
            {
                var orderContext = new OrderContext(_order);

                await orderContext.CompleteOrder();
                _context.Update(_order);

                await UpdateUserBalance();

                await RecordHistory(_order.Id);

                await NotifySeller();

                await _unitOfWorkDb.SaveChangesAsync();

                await _unitOfWorkDb.CommitAsync();

                return true ;
            }
            catch (Exception)
            {
                await _unitOfWorkDb.RollbackAsync();
                throw;
            }
        }

        private async Task RecordHistory(Guid orderId)
        {
            var historyRequest = new CreateHistoryRequest
            {
                Amount = _order.TotalMoneyForBuyer,
                TransactionType = (int)UpdateMoneyMode.AddMoney,
                Note = $"Cộng tiền hoàn thành đơn trung gian mã số: {orderId}",
                Payload = "Giao dịch thành công",
                UserId = _order.CreatedBy,
                OnDoneLink = "Example@gmail.com"
            };

            await _historyService.CreateHistory(historyRequest);
        }

        private async Task UpdateUserBalance()
        {
            var updateMoney = new UpdateMoneyRequest
            {
                UserId = _order.CreatedBy,
                Money = _order.SellerReceivedOnSuccess,
                TypeUpdate = (int) UpdateMoneyMode.AddMoney
            };

            await _userService.UpdateMoney(updateMoney);
        }

        private async Task NotifySeller()
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Hoàn thành đơn hàng mã số: {_order.Id}",
                Content = $"Trạng thái đơng hàng: Hoàn thành\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = _order.CreatedBy
            };

            await _messageService.CreateMessage(messageRequest);
        }

    }

    public class ComplainOrderCommand : ICommand<bool>
    {

        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;
        private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
        private readonly Order _order;

        public ComplainOrderCommand(
            Order order,
            ApplicationDbContext context,
            IMessageService messageService,
            IUnitOfWorkPersistDb unitOfWorkDb)
        {
            _order = order;
            _context = context;
            _messageService = messageService;
            _unitOfWorkDb = unitOfWorkDb;
        }

        public async Task<bool> ExecuteAsync()
        {
            await _unitOfWorkDb.BeginTransactionAsync();

            try
            {
                var orderContext = new OrderContext(_order);

                await orderContext.Complain();
                _context.Update(_order);

                await NotifySeller();

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

        private async Task NotifySeller()
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Ghi nhận khiếu nại đơn hàng mã số: {_order.Id}",
                Content = $"Trạng thái đơng hàng: Người mua khiếu nại đơn hàng\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = _order.CreatedBy
            };

            await _messageService.CreateMessage(messageRequest);
        }

    }

    public class RequestBuyerCheckOrderCommand : ICommand<bool>
    {

        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;
        private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
        private readonly Order _order;

        public RequestBuyerCheckOrderCommand(
            Order order,
            ApplicationDbContext context,
            IMessageService messageService,
            IUnitOfWorkPersistDb unitOfWorkDb)
        {
            _order = order;
            _context = context;
            _messageService = messageService;
            _unitOfWorkDb = unitOfWorkDb;
        }

        public async Task<bool> ExecuteAsync()
        {
            await _unitOfWorkDb.BeginTransactionAsync();

            try
            {
                var orderContext = new OrderContext(_order);

                await orderContext.RequestBuyerCheckOrder();
                _context.Update(_order);

                await NotifyBuyer();

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

        private async Task NotifyBuyer()
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Ghi nhận yêu cầu kiểm tra hàng mã số: {_order.Id}",
                Content = $"Trạng thái đơng hàng: Người bán yều cầu khách hàng kiểm tra lại đơn hàng\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = (int)_order.Customer
            };

            await _messageService.CreateMessage(messageRequest);
        }

    }

    public class CallAdminCommand : ICommand<bool>
    {

        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;
        private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
        private readonly Order _order;

        public CallAdminCommand(     
            Order order,
            ApplicationDbContext context,
            IMessageService messageService,
            IUnitOfWorkPersistDb unitOfWorkDb)
        {
            _order = order;
            _context = context;
            _messageService = messageService;
            _unitOfWorkDb = unitOfWorkDb;
        }

        public async Task<bool> ExecuteAsync()
        {
            await _unitOfWorkDb.BeginTransactionAsync();

            try
            {
                var orderContext = new OrderContext(_order);

                await orderContext.CallAdmin();
                _context.Update(_order);

                await NotifyBuyer();

                await NotifySeller();

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

        private async Task NotifyBuyer()
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Yêu cầu admin xử lý hàng mã số: {_order.Id}",
                Content = $"Trạng thái đơng hàng: Yêu cầu admin xử lý đơn hàng\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = (int) _order.Customer
            };

            await _messageService.CreateMessage(messageRequest);
        }

        private async Task NotifySeller()
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Yêu cầu admin xử lý hàng mã số: {_order.Id}",
                Content = $"Trạng thái đơng hàng: Yêu cầu admin xử lý đơn hàng\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = _order.CreatedBy
            };

            await _messageService.CreateMessage(messageRequest);
        }

    }

    public class SellerCancelOrderCommand : ICommand<bool>
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;
        private readonly IUnitOfWorkPersistDb _unitOfWorkDb;
        private readonly Order _order;
        private readonly IHistoryService _historyService;

        public SellerCancelOrderCommand(
            IHistoryService historyService,
            IUserService userService,
            Order order,
            ApplicationDbContext context,
            IMessageService messageService,
            IUnitOfWorkPersistDb unitOfWorkDb)
        {
            _historyService = historyService;
            _userService = userService;
            _order = order;
            _context = context;
            _messageService = messageService;
            _unitOfWorkDb = unitOfWorkDb;
        }

        public async Task<bool> ExecuteAsync()
        {
            await _unitOfWorkDb.BeginTransactionAsync();

            try
            {
                var orderContext = new OrderContext(_order);

                await orderContext.CancelOrder();
                _context.Update(_order);

                await UpdateUserBalance();

                await RecordHistory(_order.Id);

                await NotifyBuyer();

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

        private async Task RecordHistory(Guid orderId)
        {
            var historyRequest = new CreateHistoryRequest
            {
                Amount = _order.TotalMoneyForBuyer,
                TransactionType = (int)UpdateMoneyMode.AddMoney,
                Note = $"Hoàn tiền cho người mua do đơn hàng lỗi mã số: {orderId}",
                Payload = "Giao dịch thành công",
                UserId = (int) _order.Customer,
                OnDoneLink = "Example@gmail.com"
            };

            await _historyService.CreateHistory(historyRequest);
        }

        private async Task UpdateUserBalance()
        {
            var updateMoney = new UpdateMoneyRequest
            {
                UserId = (int) _order.Customer,
                Money = _order.TotalMoneyForBuyer,
                TypeUpdate = (int)UpdateMoneyMode.AddMoney
            };

            await _userService.UpdateMoney(updateMoney);
        }

        private async Task NotifyBuyer()
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Hoàn tiền cho người mua do đơn hàng lỗi mã số: {_order.Id}",
                Content = $"Trạng thái đơng hàng: Hủy đơn hàng\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = (int)_order.Customer
            };

            await _messageService.CreateMessage(messageRequest);
        }

    }

} 