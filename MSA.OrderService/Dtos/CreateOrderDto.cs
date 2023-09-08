namespace MSA.OrderService.Dtos
{
    public record class CreateOrderDto
    (
        Guid UserId,
        Guid ProductId
    );
}
