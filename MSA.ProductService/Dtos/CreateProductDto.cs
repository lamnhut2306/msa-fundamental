using System.ComponentModel.DataAnnotations;

namespace MSA.ProductService.Dtos
{
    public record CreateProductDto
    (
        [Required]
        string Name,

        [Required]
        string Description,

        [Range(0, 1000)]
        Decimal Price
    );
}