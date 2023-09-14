namespace MSA.Common.Contracts.Events.Order;

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
};

public record OrderCompleted
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
};

public record OrderCancelled
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public string Reason { get; set; }
};