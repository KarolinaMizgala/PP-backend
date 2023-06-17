using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PayPal.Api;
using WizardShopAPI.Models;
using WizardShopAPI.Paypal;
using WizardShopAPI.DTOs;
using AutoMapper;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private PayPal.Api.Payment payment;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WizardShopDbContext _dbContext;
        private readonly IMapper _mapper;

        public PaymentController(IHttpContextAccessor httpContextAccessor, WizardShopDbContext dbContext, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        
        [HttpPost]
        [Authorize]
        [Route("PaypalPayment")]
        public ActionResult PaymentWithPaypal(string Cancel, int id)
        {
            // Pobranie apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            var order = GetOrder(id);
            if (order == null)
            {
                return NotFound("Zamówienie nie istnieje.");
            }

            try
            {
                string payerId = Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    // Tworzenie płatności i uzyskanie URL przekierowania do PayPal
                    var redirectUrl = CreatePayment(apiContext, Request.Scheme + "://" + Request.Host.Value, id);
                    return Redirect(redirectUrl);
                }
                else
                {
                    // Wykonanie płatności po otrzymaniu PayerID od PayPal
                    var success = ExecutePayment(apiContext, payerId, out var executedPayment);
                    if (success)
                    {
                        // Płatność zatwierdzona, zmiana stanu zamówienia na "Paid"
                        order.OrderState = OrderState.Paid;
                        order.DatePayment = DateTime.Now;
                        var pay = new Models.Payment();
                        pay.Type = PaymentType.PayPal;
                        order.PaymentId = pay.PaymentId;
                        _dbContext.Add(pay);
                        _dbContext.SaveChanges();
                        return Ok("Płatność zakończona sukcesem.");
                    }
                    else
                    {
                        // Płatność nieudana
                        return BadRequest("Płatność nie została zatwierdzona przez PayPal.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Wystąpił błąd podczas przetwarzania płatności.");
            }
        }

        private string CreatePayment(APIContext apiContext, string redirectUrl, int orderId)
        {
            var order = GetOrder(orderId);
            if (order == null)
            {
                return null;
            }

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            var orderItems = order.OrderItems;
            foreach (var item in orderItems)
            {
                itemList.items.Add(new Item()
                {
                    name = item.OrderId.ToString(),
                    currency = "PLN",
                    price = item.Price.ToString(),
                    quantity = item.Quantity.ToString(),
                    sku = "sku"
                });
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "/api/PaymentControllercs/PaypalPayment?Cancel=true",
                return_url = redirectUrl + "/api/PaymentControllercs/PaypalPayment?id=" + orderId
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = order.TotalPrice.ToString()
            };

            var amount = new Amount()
            {
                currency = "PLN",
                total = order.TotalPrice.ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });

            var payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            var createdPayment = payment.Create(apiContext);

            var approvalUrl = createdPayment.links.FirstOrDefault(lnk => lnk.rel.ToLower().Trim().Equals("approval_url"))?.href;

            return approvalUrl;
        }

        private bool ExecutePayment(APIContext apiContext, string payerId, out PayPal.Api.Payment executedPayment)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };

            executedPayment = this.payment.Execute(apiContext, paymentExecution);
            return executedPayment.state.ToLower() == "approved";
        }

        private Models.Order GetOrder(int id)
        {
            return _dbContext.Orders
                .Include(r => r.OrderItems)
                .FirstOrDefault(r => r.OrderId == id);
        }

        [HttpPost]
        [Authorize]
        [Route("CardPayment")]
        public ActionResult PaymentWithCard(PaymentDto paymentDto, int orderId)
        {
            var order = GetOrder(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderState = OrderState.Paid;
            order.DatePayment = DateTime.Now;

            var payment = _mapper.Map<Models.Payment>(paymentDto);
            payment.Type = PaymentType.Card;

            _dbContext.Payments.Add(payment);
            _dbContext.SaveChanges(); // Zapisz nową płatność, aby uzyskać PaymentId

            order.PaymentId = payment.PaymentId;
            _dbContext.Orders.Update(order);
            _dbContext.SaveChanges(); // Zaktualizuj zamówienie z przypisanym PaymentId

            return Ok(payment);
        }

        [HttpGet]
         [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Models.Payment>>> GetPayments()
        {
            if (_dbContext.Payments == null)
            {
                return NotFound();
            }
            var payments = await _dbContext
                  .Payments
                  .ToListAsync();


            if (payments == null) return NotFound();
            var result = _mapper.Map<List<Models.Payment>>(payments);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<ActionResult> GetPaymentById(int id)
        {
            if (_dbContext.Payments == null)
            {
                return null;
            }

            var payments = _dbContext
                  .Payments
                  .FirstOrDefault(r => r.PaymentId == id);

            if (payments == null) return NotFound();

            return Ok(payments);

        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] PaymentDto dto)
        {
            if (_dbContext.Payments == null)
            {
                return NotFound();
            }

            var payments = _dbContext
                            .Payments
                            .FirstOrDefault(r => r.PaymentId == id);

            if (payments     == null) return NotFound();

  
            var result = _mapper.Map<PaymentDto>(payments);
         result.ZIP = dto.ZIP;
            result.Country = dto.Country;
            result.CardNumber = dto.ExpirationDate;
            result.ExpirationDate = dto.ExpirationDate;
            result.CVC = dto.CVC;
            result.Country = dto.Country;
            result.NameOnCard = dto.NameOnCard;
         
            var paymentResult = _mapper.Map<Models.Payment>(result);
            _dbContext.SaveChanges();

            return Ok(result);

        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        public ActionResult DeletePayment(int id)
        {

            var payments = _dbContext
                            .Payments
                            .FirstOrDefault(r => r.PaymentId == id);

            if (payments == null) return NotFound("Orders not fount");

      
            _dbContext.Payments.Remove(payments);
         
            _dbContext.SaveChanges();
            return Ok();

        }
    }
}
