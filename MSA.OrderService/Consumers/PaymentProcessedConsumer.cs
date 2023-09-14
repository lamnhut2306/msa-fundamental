using MassTransit;

using MSA.Common.Contracts.Domain;
using MSA.Common.Contracts.Enums;
using MSA.Common.Contracts.Events.Product;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;

namespace MSA.OrderService.Consumers
{
    public class ProductValidatedSucceededConsumer : IConsumer<ProductValidatedSucceeded>
    {
        private readonly IRepository<Order> orderRepository;
        private readonly PostgresUnitOfWork<MainDbContext> uoW;

        public ProductValidatedSucceededConsumer(
            IRepository<Order> orderRepository,
            PostgresUnitOfWork<MainDbContext> uoW)
        {
            this.orderRepository = orderRepository;
            this.uoW = uoW;
        }
        public async Task Consume(ConsumeContext<ProductValidatedSucceeded> context)
        {
            var message = context.Message;
            var order = await this.orderRepository.GetAsync(order => order.Id == message.OrderId);
            order.OrderStatus = OrderStatusEnum.Processed.ToString();
            await this.orderRepository.UpdateAsync(order);
            await this.uoW.SaveChangeAsync();
        }
    }
}
