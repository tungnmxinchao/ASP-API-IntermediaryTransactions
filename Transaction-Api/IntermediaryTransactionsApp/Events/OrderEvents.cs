using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Interface.MessageInterface;

namespace IntermediaryTransactionsApp.Events
{
    public abstract class OrderEvent
    {
        public Guid OrderId { get; set; }
        public int UserId { get; set; }
    }

    public class OrderCreatedEvent : OrderEvent
    {
        public OrderCreatedEvent(Guid orderId, int userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }

    public class OrderBoughtEvent : OrderEvent
    {
        public OrderBoughtEvent(Guid orderId, int userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }

    public interface IOrderEventHandler<TEvent> where TEvent : OrderEvent
    {
        Task HandleAsync(TEvent @event);
    }

    public class OrderCreatedEventHandler : IOrderEventHandler<OrderCreatedEvent>
    {
        private readonly IMessageService _messageService;

        public OrderCreatedEventHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Hệ thống đã ghi nhận yêu trung gian mã số: {@event.OrderId}",
                Content = $"Hệ thống đã ghi nhận yêu cầu trung gian mã số: {@event.OrderId}!\r\nVui lòng nhấn \"CHI TIẾT\" để xem chi tiết yêu cầu trung gian",
                UserId = @event.UserId
            };

            await _messageService.CreateMessage(messageRequest);
        }
    }

    public class OrderBoughtEventHandler : IOrderEventHandler<OrderBoughtEvent>
    {
        private readonly IMessageService _messageService;

        public OrderBoughtEventHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task HandleAsync(OrderBoughtEvent @event)
        {
            var messageRequest = new CreateMessageRequest
            {
                Subject = $"Hoàn thành xử lý thanh toán giao dịch mã số: {@event.OrderId}",
                Content = $"Trạng thái giao dịch: Thành công\r\nVui lòng nhấn \"CHI TIẾT\" để đến trang xem giao dịch",
                UserId = @event.UserId
            };

            await _messageService.CreateMessage(messageRequest);
        }
    }

    public interface IOrderEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event) where TEvent : OrderEvent;
    }

    public class OrderEventDispatcher : IOrderEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : OrderEvent
        {
            var handlerType = typeof(IOrderEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                await (Task)method.Invoke(handler, new object[] { @event });
            }
        }
    }
} 