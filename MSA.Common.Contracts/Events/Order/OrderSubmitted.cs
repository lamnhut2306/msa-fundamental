namespace MSA.Common.Contracts.Events.Order;

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
};