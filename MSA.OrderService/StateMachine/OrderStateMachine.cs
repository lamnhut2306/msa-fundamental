using MassTransit;
using MSA.Common.Contracts.Settings;
using MSA.Common.Contracts.Events.Order;
using MSA.Common.Contracts.Events.Product;
using MSA.Common.Contracts.Events.Payment;

namespace MSA.OrderService.StateMachine;

public class OrderStateMachine
    : MassTransitStateMachine<OrderState>
{
    private readonly IConfiguration configuration;

    public OrderStateMachine(
        IConfiguration configuration
    )
    {
        this.configuration = configuration;

        var serviceEndPoints = configuration
            .GetSection(nameof(ServiceEndpointSetting))
            .Get<ServiceEndpointSetting>();

        InstanceState(
            x => x.CurrentState
        );

        Event(() => OrderSubmitted,
            x => x.CorrelateById(context => context.Message.OrderId)
        );

        Event(() => ProductValidatedSucceeded,
            x => x.CorrelateById(context => context.Message.OrderId)
        );

        Event(() => ProductValidatedFailed,
            x => x.CorrelateById(context => context.Message.OrderId)
        );

        Event(() => PaymentProcessedSucceeded,
            x => x.CorrelateById(context => context.Message.OrderId)
        );

        Event(() => PaymentProcessedFailed,
            x => x.CorrelateById(context => context.Message.OrderId)
        );

        Event(() => OrderCompleted,
            x => x.CorrelateById(context => context.Message.OrderId)
        );

        Event(() => OrderCancelled,
            x => x.CorrelateById(context => context.Message.OrderId)
        );

        Initially(
            When(OrderSubmitted)
                .Then(x => Console.WriteLine($"Receiving Order {x.Message.OrderId}"))
                .Then(x => {
                    x.Saga.OrderId = x.Message.OrderId;
                    x.Saga.Reason += $"Receiving Order {x.Message.OrderId};";
                })
                .TransitionTo(Submitted)
        );

        During(Submitted,
            When(ProductValidatedSucceeded)
                .Then(x => Console.WriteLine($"Validate Product Succeeded for OrderId {x.Message.OrderId}"))
                .Then(x => {
                    x.Saga.ProductValidationId = x.Message.ProductId;
                    x.Saga.Reason += $"Validate Product Succeeded for OrderId {x.Message.OrderId};";
                })
                .TransitionTo(Processed),
            When(ProductValidatedFailed)
                .Then(x => Console.WriteLine($"Validate Product Failed for OrderId {x.Message.OrderId}"))
                .Then(x => {
                    x.Saga.ProductValidationId = x.Message.ProductId;
                    x.Saga.Reason += x.Message.Reason;
                })
                .TransitionTo(Cancelled)
                .Finalize()
        );
        
        During(Processed,
            When(PaymentProcessedSucceeded)
                .Then(x => Console.WriteLine($"Pay Succeeded for OrderId {x.Message.OrderId}"))
                .Then(x => {
                    x.Saga.PaymentId = x.Message.PaymentId;
                    x.Saga.Reason += $"Pay Succeeded for OrderId {x.Message.OrderId}";
                })
                .Publish(context => new OrderCompleted()
                {
                    OrderId = context.Message.OrderId,
                })
                .TransitionTo(Confirmed),
            When(PaymentProcessedFailed)
                .Then(x => Console.WriteLine($"Pay Failed for OrderId {x.Message.OrderId}"))
                .Then(x => {
                    x.Saga.PaymentId = x.Message.PaymentId;
                    x.Saga.Reason += x.Message.Reason;
                })
                .Publish(context => new OrderCancelled()
                {
                    OrderId = context.Message.OrderId,
                })
                .TransitionTo(Cancelled)
        );

        During(Confirmed,
            When(OrderCompleted)
                .Then(x => Console.WriteLine($"Order completed for OrderId {x.Message.OrderId}"))
                .Then(x =>
                {
                    x.Saga.Reason += $"Order completed for OrderId {x.Message.OrderId}";
                })
                .Finalize()
        );

        During(Cancelled,
            When(OrderCancelled)
                .Then(x => Console.WriteLine($"Order cancelled for OrderId {x.Message.OrderId}"))
                .Then(x =>
                {
                    x.Saga.Reason += x.Message.Reason;
                })
                .Finalize()
        );
    }

    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<ProductValidatedSucceeded> ProductValidatedSucceeded { get; private set; }
    public Event<ProductValidatedFailed> ProductValidatedFailed { get; private set; }
    public Event<PaymentProcessedSucceeded> PaymentProcessedSucceeded { get; private set; }
    public Event<PaymentProcessedFailed> PaymentProcessedFailed { get; private set; }
    public Event<OrderCompleted> OrderCompleted { get; private set; }
    public Event<OrderCancelled> OrderCancelled { get; private set; }

    public State Submitted { get; private set; }
    public State Processed { get; private set; }
    public State Cancelled { get; private set; }
    public State Confirmed { get; private set; }
}