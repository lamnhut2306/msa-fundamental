using Microsoft.AspNetCore.Mvc;

using MSA.BankService.Data;
using MSA.BankService.Domain;
using MSA.BankService.Dtos;
using MSA.Common.Contracts.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;

namespace BankService.Controllers
{
    [ApiController]
    [Route("v1/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly PostgresUnitOfWork<BankDbContext> _uow;


        public PaymentController(ILogger<PaymentController> logger,
            IRepository<Payment> paymentRepository,
            PostgresUnitOfWork<BankDbContext> uow)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
            _uow = uow;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Payment>> GetAsync(Guid? id)
        {
            var payment = await _paymentRepository.GetAsync(x => x.Id == id);
            return payment;
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> PostAsync(CreatePaymentDto createPayment)
        {
            var payment = new Payment()
            {
                Id = Guid.NewGuid(),
                OrderId = createPayment.OrderId,
                Status = createPayment.Status
            };
            await _paymentRepository.CreateAsync(payment);
            await _uow.SaveChangeAsync();

            return CreatedAtAction(nameof(PostAsync), payment);
        }
    }
}