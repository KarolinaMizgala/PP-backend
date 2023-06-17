using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System.Text;
using WizardShopAPI.DTOs;
using WizardShopAPI.Infrastructure;
using WizardShopAPI.Models;
using WizardShopAPI.Paypal;
using WizardShopAPI.Services;
using Order = WizardShopAPI.Models.Order;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserContextService _userContextService;
        private readonly IConfiguration _configuration;
        private ShoppingCartManager _shoppingCartManager { get; set; }
        private WizardShopDbContext _dbContext { get; set; }
        public OrderController(IHttpContextAccessor httpContextAccessor, WizardShopDbContext dbContext, IMapper mapper, IUserContextService userContextService, IConfiguration configuration)
        {

            _shoppingCartManager = new ShoppingCartManager(httpContextAccessor, dbContext, mapper);
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _configuration = configuration;
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

            order.OrderState=OrderState.Cancelled;
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
            return Ok();

        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public ActionResult UpdateOrder(int id, OrderState orderSate)
        {
            var order = _dbContext
            .Orders
            .FirstOrDefault(r => r.OrderId == id);
            if (order == null) return NotFound("Orders not fount");

            int orderDetailsId = order.OrderDetailsId;
            var orderDetails = _dbContext.OrderDetails.FirstOrDefault(r => r.Id == orderDetailsId);
            order.OrderState= orderSate;
            switch(orderSate)
            {
                case OrderState.Paid:  order.DatePayment = DateTime.Now; break;
                case OrderState.Delivered: order.DateDelivered = DateTime.Now; break;
                case OrderState.Shipped: order.DateShipped = DateTime.Now; break;
            }
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges();
            return Ok();

        }

        private Order GetOrder(int id)
        {
            if (_dbContext.Orders == null)
            {
                return null;
            }

            var order =  _dbContext
                  .Orders
                  .Include(r => r.OrderDetails)
                  .Include(r => r.OrderItems)
                  .FirstOrDefault(r => r.OrderId == id);

            return order;
        }
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<ActionResult> GetOrderById(int id)
        {

            var order = GetOrder(id);

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
