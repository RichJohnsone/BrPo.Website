using BrPo.Website.Data;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.ShoppingBasket.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BrPo.Website.Services.ShoppingBasket.Services
{
    public interface IShoppingBasketService
    {
        Task CreatePrintOrderAsync(PrintOrder printOrder);
        Task AddPrintOrderToBasketAsync(PrintOrder printOrder);
        bool PriceIsCorrect(PrintOrder printOrder, PaperModel paper);
    }

    public class ShoppingBasketService : IShoppingBasketService
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<ShoppingBasketService> _logger;

        public ShoppingBasketService(
            ApplicationDbContext applicationDbContext,
            ILogger<ShoppingBasketService> logger)
        {
            context = applicationDbContext;
            _logger = logger;
        }

        public async Task CreatePrintOrderAsync(PrintOrder printOrder)
        {
            try
            {
                context.PrintOrders.Add(printOrder);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("from ShoppingBasketService.CreatePrintOrderAsync", ex);
                throw;
            }
        }

        public async Task AddPrintOrderToBasketAsync(PrintOrder printOrder)
        {
            try
            {
                var basketItem = new BasketItem();
                basketItem.UserId = printOrder.UserId;
                basketItem.PrintOrderId = printOrder.Id;
                context.basketItems.Add(basketItem);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("from ShoppingBasketService.AddPrintOrderToBasketAsync", ex);
                throw;
            }
        }

        public bool PriceIsCorrect(PrintOrder printOrder, PaperModel paper)
        {
            if (printOrder == null) return false;
            if (paper == null) return false;
            var price = GetPrice(printOrder, paper);
            return printOrder.Value == price;
        }

        private decimal GetPrice(PrintOrder printOrder, PaperModel paper)
        {
            try
            {
                if (paper.RollPaper == true)
                {
                    var paperLength = printOrder.Height > printOrder.Width ? printOrder.Height + 100 : printOrder.Width + 100;
                    var pricePerPrint = (paperLength / 1000) * paper.CostPerMeter;
                    pricePerPrint = GetQuantityDiscountPrice(pricePerPrint, printOrder.Quantity);
                    if (printOrder.IsDraft) 
                        pricePerPrint = pricePerPrint / 2;
                    var orderValue = Math.Round((pricePerPrint * printOrder.Quantity), 2);
                    return orderValue;
                }
                else
                {
                    var pricePerPrint = GetQuantityDiscountPrice(paper.CostPerSheet, printOrder.Quantity);
                    if (printOrder.IsDraft)
                        pricePerPrint = pricePerPrint / 2;
                    var orderValue = Math.Round((pricePerPrint * printOrder.Quantity), 2);
                    return orderValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("from ShoppingBasketService.AddPrintOrderToBasketAsync", ex);
                throw;
            }
        }

        private decimal GetQuantityDiscountPrice(decimal pricePerPrint, int quantity)
        {
            try
            {
                var discount = 0;
                switch (quantity)
                {
                    case 1:
                        break;
                    case int n when (n <= 4):
                        discount = 5;
                        break;
                    case int n when (n <= 9):
                        discount = 10;
                        break;
                    case int n when (n <= 19):
                        discount = 15;
                        break;
                    case int n when (n <= 29):
                        discount = 20;
                        break;
                    case int n when (n <= 39):
                        discount = 25;
                        break;
                    case int n when (n >= 80):
                        discount = 30;
                        break;
                }
                if (discount == 0) return pricePerPrint;
                return (((100 - discount) / 100) * pricePerPrint);
            }
            catch (Exception ex)
            {
                _logger.LogError("from ShoppingBasketService.GetQuantityDiscountPrice", ex);
                throw;
            }
        }
    }
}
