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
    public class CartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        private ShoppingCartManager _shoppingCartManager { get; set; }
       
        private WizardShopDbContext _dbContext { get; set; }

        public CartController(IHttpContextAccessor httpContextAccessor, WizardShopDbContext dbContext, IMapper mapper, IUserContextService userContextService)
        {
          
            _shoppingCartManager = new ShoppingCartManager(httpContextAccessor, dbContext, mapper);
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
        }
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddToCart(int id)
        {

            var item = _shoppingCartManager.AddToCart(id);
            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpGet]
        public ActionResult GetCartItems()
        {
            var cart = _shoppingCartManager.GetCart();
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }



        [HttpDelete]
        [Route("{id}")]
        public ActionResult RemoveFromCart(int id)
        {
            int itemCount = _shoppingCartManager.RemoveFromCart(id);
            int cartItemsCount = _shoppingCartManager.GetCartItemsCount();
            double cartTotal = _shoppingCartManager.GetCartTotalPrice();

            return Ok(new
            {
                itemCount = itemCount,
                cartItemsCount = cartItemsCount,
                cartTotal = cartTotal
            });
        }


        

       
    }
}
