using Microsoft.AspNetCore.Mvc;
using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;
using MSA.OrderService.Dtos;
using MSA.Common.Contracts.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Services;
using MSA.Common.Contracts.Commands.Product;
using MassTransit;
using MassTransit.Transports;
using MSA.Common.Contracts.Events.Order;

namespace MSA.OrderService.Controllers;

[ApiController]
[Route("v1/order")]
public class OrderController : ControllerBase
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly ILogger<OrderController> _logger;
    private readonly PostgresUnitOfWork<MainDbContext> uow;
    private readonly IProductService _productService;
    private readonly IPublishEndpoint _publishEndpoint;


    public OrderController(
        IRepository<Order> orderRepository,
        IRepository<Product> productRepository,
        PostgresUnitOfWork<MainDbContext> uow,
        IProductService productService,
        ILogger<OrderController> logger,
        IPublishEndpoint publishEndpoint
        )
    {
        this._orderRepository = orderRepository;
        this._productRepository = productRepository;
        this.uow = uow;
        this._productService = productService;
        this._logger = logger;
        this._publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IEnumerable<Order>> GetAsync()
    {
        var orders = (await _orderRepository.GetAllAsync()).ToList();
        return orders;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> PostAsync(CreateOrderDto createOrderDto)
    {
        // var product = await _productRepository.GetAsync(item => item.ProductId == createOrderDto.ProductId);
        // var isProductExisted = product is not null;
        // if (!isProductExisted) return BadRequest();
        
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = createOrderDto.UserId,
            OrderStatus = "Order Submitted"
        };
        await _orderRepository.CreateAsync(order);

        /*await _publishEndpoint.Publish(new ValidateProduct {
            OrderId = order.Id,
            ProductId = createOrderDto.ProductId
        });*/

        await _publishEndpoint.Publish<OrderSubmitted>(new OrderSubmitted
        {
            OrderId = order.Id,
            ProductId = createOrderDto.ProductId
        });

        await uow.SaveChangeAsync();

        return CreatedAtAction(nameof(PostAsync), order);
    }
}