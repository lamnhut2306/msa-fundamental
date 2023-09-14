using MassTransit;

using MSA.Common.Contracts.Commands.Product;
using MSA.Common.Contracts.Domain;
using MSA.Common.Contracts.Events.Order;
using MSA.Common.Contracts.Events.Product;
using MSA.ProductService.Entities;

namespace MSA.ProductService.Consumers;

public class ValidateProductConsumer : IConsumer<OrderSubmitted>
{
    private readonly ILogger<ValidateProductConsumer> logger;
    private readonly IRepository<Product> repository;

    public ValidateProductConsumer(
        ILogger<ValidateProductConsumer> logger,
        IRepository<Product> repository)
    {
        this.logger = logger;
        this.repository = repository;
    }

    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        var message = context.Message;
        //TODO : Validate and submit Commands/Events for further flow
        logger.Log(LogLevel.Information,
            $"Receiving message of order {message.OrderId} validating product {message.ProductId}"
        );

        var product = await this.repository.GetAsync(x => x.Id == message.ProductId);
        bool isExisted = product != null;
        if (isExisted)
        {
            logger.Log(LogLevel.Information,
                $"Publishing succeeded validation message of order {message.OrderId} validating product {message.ProductId}"
            );
            await context.Publish<ProductValidatedSucceeded>(new ProductValidatedSucceeded()
            {
                OrderId = message.OrderId,
                ProductId = message.ProductId,
            });
        }
        else
        {
            logger.Log(LogLevel.Information,
                $"Publishing failed validation message of order {message.OrderId} validating product {message.ProductId}"
            );
            await context.Publish<ProductValidatedFailed>(new ProductValidatedFailed()
            {
                OrderId = message.OrderId,
                ProductId = message.ProductId,
            });
        }
    }
}