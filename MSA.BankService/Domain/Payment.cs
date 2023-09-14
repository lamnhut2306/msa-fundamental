using MSA.Common.Contracts.Domain;

namespace MSA.BankService.Domain
{
    public class Payment : IEntity
    {
        public Guid Id { get; init; }

        public Guid OrderId { get; set; }

        public string Status { get; set; } = "Succeeded";
    }
}
