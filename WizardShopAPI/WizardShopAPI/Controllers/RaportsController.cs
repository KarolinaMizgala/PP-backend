using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using System.Drawing;
using System.Linq.Expressions;
using System.Text;
using WizardShopAPI.Infrastructure;
using WizardShopAPI.Models;
using WizardShopAPI.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaportsController: ControllerBase
    {
        private readonly WizardShopDbContext _dbContext;

        public RaportsController( WizardShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private Order GetOrder(int id)
        {
            if (_dbContext.Orders == null)
            {
                return null;
            }

            var order = _dbContext
                  .Orders
                  .Include(r => r.OrderDetails)
                  .Include(r => r.OrderItems)
                  .FirstOrDefault(r => r.OrderId == id);

            return order;
        }

        [HttpGet]
        [Route("Invoice/{id}")]
        public IActionResult GeneratePDF(int id)
        {
            var order = GetOrder(id);
            // Register the encoding provider for 1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Set up invoice data
            string sellerName = "WizardShop, Inc.";
            string sellerAddress = "44-100 Gliwice, Akademicka Street 11";
            string sellerBankAccount = "Account: Magic Bank, Branch in Gliwice, No. 12 1234 1234 1234 1234 1234 1234";
            string sellerNIP = "VAT ID: 9492107026 EU VAT ID: PL9492107026";


            string documentNumber = $"WS{order.DateCreated.Year}/{order.DateCreated.Month}/{order.OrderId}";
            DateTime saleDate = order.DateCreated;


            var totalPrice = order.TotalPrice.ToString();

            // Set up page layout
            int pageWidth = (int)page.Width - 120;
            int marginLeft = 40;
            int marginTop = 40;
            int columnWidth = (pageWidth - 2 * marginLeft) / 3;
            int rowHeight = 20;

            // Draw seller information
            DrawText(gfx, "Seller:", marginLeft, marginTop, XBrushes.Black);
            DrawText(gfx, sellerName, marginLeft, marginTop + rowHeight, XBrushes.Black);
            DrawText(gfx, sellerAddress, marginLeft, marginTop + 2 * rowHeight, XBrushes.Black);
            DrawText(gfx, sellerBankAccount, marginLeft, marginTop + 3 * rowHeight, XBrushes.Black);
            DrawText(gfx, sellerNIP, marginLeft, marginTop + 4 * rowHeight, XBrushes.Black);

            // Draw document header
            DrawText(gfx, $"PRO FORMA INVOICE NO: {documentNumber}", marginLeft, marginTop + 6 * rowHeight, XBrushes.Black, XStringFormats.TopCenter);

            var addressDb = _dbContext.Addresses.Find(order.OrderDetails.AddressId);

            string address = $"{addressDb.ZipCode} {addressDb.City}, {addressDb.Street} {addressDb.HouseNumber}/{addressDb.ApartmentNumber}";

            // Draw buyer information
            DrawText(gfx, $"Buyer: {order.OrderDetails.FirstName} {order.OrderDetails.LastName}", marginLeft, marginTop + 9 * rowHeight, XBrushes.Black);

            DrawText(gfx, address, marginLeft, marginTop + 10 * rowHeight, XBrushes.Black);
            DrawText(gfx, $"Sale data: {saleDate}", marginLeft, marginTop + 11 * rowHeight, XBrushes.Black);
            // Draw invoice items table header
            DrawTableCell(gfx, marginLeft, marginTop + 13 * rowHeight, columnWidth / 2, rowHeight, "No.");
            DrawTableCell(gfx, marginLeft + columnWidth / 2, marginTop + 13 * rowHeight, 2 * columnWidth, rowHeight, "Item/Service Name");
            DrawTableCell(gfx, marginLeft + 2.5 * columnWidth, marginTop + 13 * rowHeight, columnWidth / 2, rowHeight, "Unit");
            DrawTableCell(gfx, marginLeft + 3 * columnWidth, marginTop + 13 * rowHeight, columnWidth, rowHeight, "Price");

            int i = 0;
            foreach (var item in order.OrderItems)
            {
                var product = _dbContext.Products.Find(item.ProductId);
                DrawTableCell(gfx, marginLeft, marginTop + (14 + i) * rowHeight, columnWidth / 2, rowHeight, (i + 1).ToString());
                DrawTableCell(gfx, marginLeft + columnWidth / 2, marginTop + (14 + i) * rowHeight, 2 * columnWidth, rowHeight, product.Name.ToString());
                DrawTableCell(gfx, marginLeft + 2.5 * columnWidth, marginTop + (14 + i) * rowHeight, columnWidth / 2, rowHeight, item.Quantity.ToString());
                DrawTableCell(gfx, marginLeft + 3 * columnWidth, marginTop + (14 + i) * rowHeight, columnWidth, rowHeight, item.Price.ToString());
                i++;
            }
            // Adjust the position of the right-aligned cells
            int rightAlignedX = marginLeft + columnWidth;
            DrawText(gfx, $"Total: {totalPrice} PLN", 430, marginTop + 17 * rowHeight, XBrushes.Black, XStringFormats.TopCenter);



            byte[] response;
            using (var ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }

            string filename = "Invoice_pro_forma_" + documentNumber + ".pdf";
            return File(response, "application/pdf", filename);
        }

        private void DrawText(XGraphics gfx, string text, int x, int y, XBrush brush, XStringFormat format = null)
        {
            XSize textSize = gfx.MeasureString(text, new XFont("Arial", 10));
            XRect rect = new XRect(x, y, textSize.Width, textSize.Height);
            gfx.DrawString(text, new XFont("Arial", 10), brush, rect, format ?? XStringFormats.TopLeft);
        }
        private async Task<Dictionary<DateTime, int>> CountOrders( DateTime start, DateTime end, string dateType)
        {
            IQueryable<Order> query;
            switch (dateType)
            {
                case "DateCreated":
                    query = _dbContext.Orders.Where(r =>  r.DateCreated >= start && r.DateCreated <= end)
                                             .OrderBy(r => r.DateCreated);
                    break;
                case "DatePayment":
                    query= _dbContext.Orders
    .Where(o => o.OrderState == OrderState.Paid && o.DatePayment >= start && o.DatePayment <= end && o.DatePayment != default(DateTime));
                    break;
                case "DateShipped":
                    query = _dbContext.Orders.Where(r =>  r.DateShipped >= start && r.DateShipped <= end)
                                             .OrderBy(r => r.DateShipped);
                    break;
                case "DateDelivered":
                    query = _dbContext.Orders.Where(r =>  r.DateDelivered >= start && r.DateDelivered <= end)
                                             .OrderBy(r => r.DateDelivered);
                    break;
                default:
                    throw new ArgumentException("Invalid date type.");
            }

            var result = await query.ToListAsync();

            var groupedResult = result.GroupBy(r => TruncateDateTime(r.DateCreated, dateType))
                                      .Select(g => new { Date = g.Key, Count = g.Count() })
                                      .ToList();

            return groupedResult.ToDictionary(r => r.Date, r => r.Count);
        }

        private DateTime TruncateDateTime(DateTime dateTime, string dateType)
        {
            switch (dateType)
            {
                case "DateCreated":
                    return dateTime.Date;
                case "DatePayment":
                    return dateTime.Date;
                case "DateShipped":
                    return dateTime.Date;
                case "DateDelivered":
                    return dateTime.Date;
                default:
                    throw new ArgumentException("Invalid date type.");
            }
        }

        private decimal CalculateTotalPriceByStatus(OrderState status)
        {
            var orders = _dbContext.Orders
                .Where(o => o.OrderState == status)
                .ToList();

            decimal totalPrice = (decimal)orders.Sum(o => o.TotalPrice);

            return totalPrice;
        }


        [HttpGet]
        [Route("Raport")]
        public async Task<IActionResult> GenerateRaport(DateTime start, DateTime end)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            int pageWidth = (int)page.Width - 120;
            int marginLeft = 40;
            int marginTop = 40;
            int columnWidth = (pageWidth - 2 * marginLeft) / 4;
            int rowHeight = 20;

            // Informacje o firmie
            string companyName = "WizardShop, Inc.";
            string companyAddress = "44-100 Gliwice, Akademicka Street 11";
            string companyContact = "VAT ID: 9492107026 EU VAT ID: PL9492107026";

            // Nagłówek raportu
            string reportHeader = "Order Report";
            string reportDates = $"Period: {start.ToString("dd.MM.yyyy")} - {end.ToString("dd.MM.yyyy")}";

            // Ustalamy format dla tekstu
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Center;

            // Rysujemy informacje o firmie
            DrawText(gfx, companyName, marginLeft, marginTop, XBrushes.Black, format);
            DrawText(gfx, companyAddress, marginLeft, marginTop + rowHeight, XBrushes.Black, format);
            DrawText(gfx, companyContact, marginLeft, marginTop + 2 * rowHeight, XBrushes.Black, format);

            // Przesuwamy pozycję na dół
            marginTop += 4 * rowHeight;

            // Rysujemy nagłówek raportu
          
            DrawText(gfx, reportHeader, marginLeft+210, marginTop, XBrushes.Black,  format);
            DrawText(gfx, reportDates, marginLeft+170, marginTop+ rowHeight, XBrushes.Black, format);
            // Przesuwamy pozycję na dół
            marginTop += 2 * rowHeight;

            // Pobieramy dane raportu asynchronicznie
            Dictionary<DateTime, int> openOrders = await CountOrders( start, end, "DateCreated");
            Dictionary<DateTime, int> paymentOrders = await CountOrders( start, end, "DatePayment");
            Dictionary<DateTime, int> shippedOrders = await CountOrders( start, end, "DateShipped");
            Dictionary<DateTime, int> deliverdOrder = await CountOrders(start, end, "DateDelivered");

            // Przesuwamy pozycję na dół
            marginTop += 3 * rowHeight;

            // Rysujemy nagłówki kolumn
            DrawTableCell(gfx, marginLeft, marginTop, columnWidth, rowHeight, "Date", format);
            DrawTableCell(gfx, marginLeft + columnWidth, marginTop, columnWidth, rowHeight, "Open Orders", format);
            DrawTableCell(gfx, marginLeft + 2 * columnWidth, marginTop, columnWidth, rowHeight, "Payment Orders", format);
            DrawTableCell(gfx, marginLeft + 3 * columnWidth, marginTop, columnWidth, rowHeight, "Shipped Orders", format);
            DrawTableCell(gfx, marginLeft + 4 * columnWidth, marginTop, columnWidth, rowHeight, "DateDelivered", format);

            // Przesuwamy pozycję na dół
            marginTop += rowHeight;

            // Rysujemy dane raportu
            foreach (var date in openOrders.Keys)
            {
                int openCount = openOrders[date];
                int paymentCount = paymentOrders.ContainsKey(date) ? paymentOrders[date] : 0; 
                int shippedCount = shippedOrders.ContainsKey(date) ? shippedOrders[date] : 0;
                int deliveredCount = deliverdOrder.ContainsKey(date) ? deliverdOrder[date] : 0;

                DrawTableCell(gfx, marginLeft, marginTop, columnWidth, rowHeight, date.ToString("dd.MM.yyyy"), format);
                DrawTableCell(gfx, marginLeft + columnWidth, marginTop, columnWidth, rowHeight, openCount.ToString(), format);
                DrawTableCell(gfx, marginLeft + 2 * columnWidth, marginTop, columnWidth, rowHeight, paymentCount.ToString(), format);
                DrawTableCell(gfx, marginLeft + 3 * columnWidth, marginTop, columnWidth, rowHeight, shippedCount.ToString(), format);
                DrawTableCell(gfx, marginLeft + 4 * columnWidth, marginTop, columnWidth, rowHeight, deliveredCount.ToString(), format);
                // Przesuwamy pozycję na dół
                marginTop += rowHeight;
            }

            // Przesuwamy pozycję na dół
            marginTop += 2 * rowHeight;

            int totalOpenCount = openOrders.Values.Sum();
            int totalPaymentCount = paymentOrders.Values.Sum();
            int totalShippedCount = shippedOrders.Values.Sum();
            int totalDeliveredCount = deliverdOrder.Values.Sum();


            // Rysujemy podsumowanie
            decimal open = CalculateTotalPriceByStatus(OrderState.Open);
            decimal payment = CalculateTotalPriceByStatus(OrderState.Paid);
 


            DrawTableCell(gfx, marginLeft, marginTop, columnWidth, rowHeight, "Total", format);
            DrawTableCell(gfx, marginLeft + 1 * columnWidth, marginTop, columnWidth, rowHeight, totalOpenCount.ToString(), format);
            DrawTableCell(gfx, marginLeft + 2 * columnWidth, marginTop, columnWidth, rowHeight, totalPaymentCount.ToString(), format);
            DrawTableCell(gfx, marginLeft +3 * columnWidth, marginTop, columnWidth, rowHeight, totalShippedCount.ToString(), format);
            DrawTableCell(gfx, marginLeft + 4 * columnWidth, marginTop, columnWidth, rowHeight, totalDeliveredCount.ToString(), format);

            marginTop += 2 * rowHeight;

            DrawTableCell(gfx, marginLeft, marginTop, columnWidth, rowHeight, "Total profit", format);
            DrawTableCell(gfx, marginLeft + 1 * columnWidth, marginTop, columnWidth, rowHeight, open.ToString(), format);
            DrawTableCell(gfx, marginLeft + 2 * columnWidth, marginTop, columnWidth, rowHeight, payment.ToString(), format);
 




            byte[] response;
            using (var ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }

            string filename = $"Report_{start.ToString("yyyyMMdd")}_{end.ToString("yyyyMMdd")}.pdf";
            return File(response, "application/pdf", filename);
        }




        // Helper method to draw table cells
        private void DrawTableCell(XGraphics gfx, double x, double y, int width, int height, string text, XStringFormat format = null)
        {
            XRect rect = new XRect(x, y, width, height);

            format ??= new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Center;

            gfx.DrawRectangle(XBrushes.White, rect);
            gfx.DrawRectangle(XPens.Black, rect);
            gfx.DrawString(text, new XFont("Arial", 10), XBrushes.Black, rect, format);
        }




    }
}
