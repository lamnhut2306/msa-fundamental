namespace MSA.ProductService.Dtos
{
    public record ProductDto
    (
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        DateTimeOffset CreatedDate
    );
}