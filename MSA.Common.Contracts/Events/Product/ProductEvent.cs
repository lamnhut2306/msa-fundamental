namespace MSA.Common.Contracts.Events.Product
{
    public record ProductCreated
    {
        public Guid ProductId { get; init; }
    }
}
