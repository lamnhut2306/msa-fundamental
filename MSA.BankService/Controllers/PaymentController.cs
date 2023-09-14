using MassTransit;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MSA.BankService.Data;
using MSA.BankService.Domain;
using MSA.BankService.Dtos;
using MSA.Common.Contracts.Domain;
using MSA.Common.Contracts.Events.Payment;
using MSA.Common.PostgresMassTransit.PostgresDB;

namespace BankService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("v1/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly PostgresUnitOfWork<BankDbContext> _uow;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentController(ILogger<PaymentController> logger,
            IRepository<Payment> paymentRepository,
            PostgresUnitOfWork<BankDbContext> uow,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
            _uow = uow;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Payment>> GetAsync(Guid? id)
        {
            var payment = await _paymentRepository.GetAsync(x => x.Id == id);
            return payment;
        }

        [HttpPost]
        [Authorize("read_access")]
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

            if (createPayment.Status == "Succeeded")
            {
                await _publishEndpoint.Publish(new PaymentProcessedSucceeded()
                {
                    OrderId = createPayment.OrderId,
                    PaymentId = payment.Id
                });
            }
            else
            {
                await _publishEndpoint.Publish(new PaymentProcessedFailed()
                {
                    OrderId = createPayment.OrderId,
                    PaymentId = payment.Id,
                    Reason = nameof(PaymentProcessedFailed)
                });
            }

            return CreatedAtAction(nameof(PostAsync), payment);
        }
    }
}