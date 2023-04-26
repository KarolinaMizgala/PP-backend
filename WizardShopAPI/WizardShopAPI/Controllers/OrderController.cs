using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WizardShopAPI.DTOs;
using WizardShopAPI.Infrastructure;
using WizardShopAPI.Models;
using WizardShopAPI.Services;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        private ShoppingCartManager _shoppingCartManager { get; set; }
        private WizardShopDbContext _dbContext { get; set; }
        public OrderController(IHttpContextAccessor httpContextAccessor, WizardShopDbContext dbContext, IMapper mapper, IUserContextService userContextService)
        {

            _shoppingCartManager = new ShoppingCartManager(httpContextAccessor, dbContext, mapper);
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        [HttpPost]
        [Authorize]
        public ActionResult CreateOrder(OrderDetailsDto orderDetails)
        {
            if (ModelState.IsValid)
            {
                var orderDet = _mapper.Map<OrderDetails>(orderDetails);
                if (orderDet is null)
                    return NotFound();

                
                orderDet.UserId = _userContextService.GetUserId;

                if (orderDet.UserId is null)
                    return NotFound();

              


                var orderDto = _mapper.Map<OrderDto>(orderDetails);

                orderDto.DateCreated = DateTime.Now;
                orderDto.TotalPrice = _shoppingCartManager.GetCartTotalPrice();
                orderDto.OrderState = OrderState.Open;

                var order = _mapper.Map<Order>(orderDto);
                order.OrderDetailsId = orderDet.Id;
                  _dbContext.OrderDetails.Add(orderDet);
                _dbContext.SaveChanges();
                order.OrderDetails = orderDet;
                foreach (var item in _shoppingCartManager.GetItemsList())
                {
                    item.OrderId = order.OrderId;
                    order.OrderItems.Add(item);
                }

                

                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();
                return Ok(order);
            }
            return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public ActionResult DeleteOrder(int id)
        {
            var order = _dbContext
            .Orders
            .FirstOrDefault(r => r.OrderId == id);
            if (order == null) return NotFound("Orders not fount");

            int orderDetailsId = order.OrderDetailsId;
            var orderDetails = _dbContext.OrderDetails.FirstOrDefault(r => r.Id == orderDetailsId);
            _dbContext.OrderDetails.Remove(orderDetails);
            _dbContext.Orders.Remove(order);
            _dbContext.SaveChanges();
            return Ok();

        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<ActionResult> GetOrderById(int id)
        {
            if (_dbContext.Orders == null)
            {
                return NotFound();
            }
           
            var order = await _dbContext
                  .Orders
                  .Include(r => r.OrderDetails)
                  .Include(r => r.OrderItems)
                  .FirstOrDefaultAsync(r=> r.OrderId ==id);

            if (order == null) return NotFound();

            if (_userContextService.GetUserId == order.OrderDetails.UserId || _userContextService.GetUserRole.Equals("Admin"));
                var result = _mapper.Map<OrderDto>(order);
            return Ok(result);

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            if (_dbContext.Orders == null)
            {
                return NotFound();
            }
            var order = await _dbContext
                  .Orders
                  .Include(r => r.OrderDetails)
                  .Include(r => r.OrderItems)
                  .ToListAsync();


            if (order == null) return NotFound();
            var result = _mapper.Map<List<OrderDto>>(order);

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody]OrderDetailsDto dto)
        {
            if (_dbContext.Orders == null)
            {
                return NotFound();
            }

            var order = await _dbContext
                  .Orders
                  .Include(r => r.OrderDetails)
                  .Include(r => r.OrderItems)
                  .FirstOrDefaultAsync(r => r.OrderId == id);

            if (order == null) return NotFound();

            if (_userContextService.GetUserId == order.OrderDetails.UserId || _userContextService.GetUserRole.Equals("Admin")) ;
            var result = _mapper.Map < OrderDto > (order);
            result.FirstName=dto.FirstName;
            result.LastName = dto.LastName;
            result.PhoneNumber = dto.PhoneNumber;
            result.Email = dto.Email;
            result.Comment = dto.Comment;
            result.HouseNumber = dto.HouseNumber;
            result.ApartmentNumber = dto.ApartmentNumber;
            result.City = dto.City;
            result.Street = dto.Street;
            result.ZipCode = dto.ZipCode;
            var orderResult = _mapper.Map<Order>(result);
            _dbContext.SaveChanges();
           
            return Ok(result);

        }

    }
}
