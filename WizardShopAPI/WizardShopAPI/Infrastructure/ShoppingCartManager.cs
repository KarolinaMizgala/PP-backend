using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using WizardShopAPI.DTOs;
using WizardShopAPI.Models;

namespace WizardShopAPI.Infrastructure
{
    public class ShoppingCartManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WizardShopDbContext _dbContext;
        private readonly IMapper _mapper;
        public const string CartSessionKey = "CartData";

        public ShoppingCartManager(IHttpContextAccessor httpContextAccessor, WizardShopDbContext dbContext, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public CartItem AddToCart(int productId)
        {
            var cart = GetCart();

            var cartItem = cart.Find(c => c.Product.Id == productId);
            CartItem item = null;
            if (cartItem != null)
            {
                
                    cartItem.Quantity++;
                cartItem.Price += cartItem.Price;
                item= cartItem;
            }
            else
            {
                var productToAdd = _dbContext.Products.Where(p => p.Id == productId).SingleOrDefault();
                if (productToAdd != null)
                {
                    var newCartItem = new CartItem()
                    {
                        Product = productToAdd,
                        Quantity = 1,
                        Price = productToAdd.Price
                    };
                    cart.Add(newCartItem);
                    item = newCartItem;
                }
            }
            SetCart(cart);
            return item;
           
        }

        public List<CartItem> GetCart()
        {
            List<CartItem> cart;

            if (!_httpContextAccessor.HttpContext.Session.TryGetValue(CartSessionKey, out byte[] cartBytes))
            {
                cart = new List<CartItem>();
            }
            else
            {
                var json = Encoding.UTF8.GetString(cartBytes);
                cart = JsonConvert.DeserializeObject<List<CartItem>>(json);
            }
         
            return cart;
        }

        public List<OrderItem> GetItemsList()
        {
            var cart = GetCart();
            if (cart == null)
            {
                return null;
            }
            var dishDtos = _mapper.Map<List<OrderItem>>(cart);
            return dishDtos;
        }

  




        public int RemoveFromCart(int productId)
        {
            var cart = GetCart();

            var cartItem = cart.Find(c => c.Product.Id == productId);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    SetCart(cart);
                    return cartItem.Quantity;
                }
                else
                    cart.Remove(cartItem);
            }
            return 0;
        }

        public double GetCartTotalPrice()
        {
            var cart = GetCart();
            return cart.Sum(c => (c.Quantity * c.Product.Price));
        }

        public int GetCartItemsCount()
        {
            var cart = GetCart();
            int count = cart.Sum(c => c.Quantity);

            return count;
        }

     /*   public Order CreateOrder(OrderDto newOrder)
        {
           
        }
*/
        public void EmptyCart()
        {
            _httpContextAccessor.HttpContext.Session.Remove(CartSessionKey);
        }

        private void SetCart(List<CartItem> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Session.SetString(CartSessionKey, json);
        }
    }

}
