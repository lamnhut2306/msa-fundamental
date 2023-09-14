namespace MSA.BankService.Dtos
{
    public class CreatePaymentDto
    {
        public Guid OrderId { get; init; }

        public string Status { get; set; }
    }
}
