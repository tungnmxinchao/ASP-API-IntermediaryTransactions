using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntermediaryTransactionsApp.Events
{
    public abstract class OrderEvent
    {
        public Guid OrderId { get; set; }
        public int UserId { get; set; }
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

    public class OrderBoughtEventHandler : IOrderEventHandler<OrderBoughtEvent>
    {
        private readonly IMessageService _messageService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderBoughtEventHandler> _logger;

        public OrderBoughtEventHandler(
            IMessageService messageService,
            IServiceScopeFactory scopeFactory,
            ILogger<OrderBoughtEventHandler> logger)
        {
            _messageService = messageService;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task HandleAsync(OrderBoughtEvent @event)
        {
            _logger.LogInformation($"Starting to process OrderBoughtEvent for OrderId: {@event.OrderId}, UserId: {@event.UserId}");

            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation($"Starting 2-minute delay for OrderId: {@event.OrderId}");
                    await Task.Delay(TimeSpan.FromMinutes(1000));
                    _logger.LogInformation($"Delay completed for OrderId: {@event.OrderId}, checking order status");
                    
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        
                        var order = await context.Orders.FindAsync(@event.OrderId);
                        if (order == null)
                        {
                            _logger.LogWarning($"Order not found for OrderId: {@event.OrderId}");
                            return;
                        }

                        _logger.LogInformation($"Current order status for OrderId: {@event.OrderId} is: {order.StatusId}");
                        
                        if (order.StatusId == 3)
                        {
                            _logger.LogInformation($"Updating order status from 3 to 4 for OrderId: {@event.OrderId}");
                            order.StatusId = 4;
                            await context.SaveChangesAsync();
                            _logger.LogInformation($"Successfully updated order status to 4 for OrderId: {@event.OrderId}");
                        }
                        else
                        {
                            _logger.LogWarning($"Order status is not 3 (current: {order.StatusId}) for OrderId: {@event.OrderId}, skipping update");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while updating order status for OrderId: {@event.OrderId}");
                }
            });
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