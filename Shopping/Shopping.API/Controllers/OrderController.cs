using Microsoft.AspNetCore.Mvc;
using Shopping.Application.QueryService;
using Shopping.Application.Services;
using Shopping.Common.Messaging.Order.Commands;

namespace Shopping.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderCommandService orderCommandService;
        private readonly IOrderQueryService orderQueryService;


        public OrderController(IOrderCommandService orderCommandService, IOrderQueryService orderQueryService)
        {
            this.orderCommandService = orderCommandService;
            this.orderQueryService = orderQueryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderWithProductsById([FromRoute] Guid id)
        {
            var model = await orderQueryService.GetOrderWithProductsById(id);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> Post(CreateOrderCommand command)
        {
            var events = await orderCommandService.Handle(command);

            return Ok(events);
        }
        [HttpPost("remove-product")]
        public async Task<IActionResult> RemoveProductItem(RemoveOrderItemCommand command)
        {

            var result = await orderCommandService.Handle(command);

            return Ok(result);
        }
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(AddProductCommand command)
        {
            var result = await orderCommandService.Handle(command);

            return Ok(result);
        }

    }
}