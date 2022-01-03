using BrPo.Website.Data;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.ShoppingBasket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrPo.Website.Services.ShoppingBasket.Services
{
    public interface IShoppingBasketService
    {
        Task CreatePrintOrderAsync(PrintOrder printOrder);
        Task AddPrintOrderToBasketAsync(PrintOrder printOrder);
        bool PriceIsCorrect(PrintOrder printOrder, PaperModel paper);
        int GetBasketCount(string userId);
        List<BasketItem> GetBasketItems(string userId);
        decimal GetBasketTotal(string userId);
        Task ChangeQuantity(string userId, int basketItemId, int newQuantity);
        Task DeleteItem(string userId, int basketItemId);
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

        public int GetBasketCount(string userId)
        {
            Guid.TryParse(userId, out var guid);
            var count = context.basketItems.Where(b => b.UserId == guid).Count();
            return count;
        }

        public List<BasketItem> GetBasketItems(string userId)
        {
            Guid.TryParse(userId, out var guid);
            return context.basketItems
                .Include(b => b.PrintOrder)
                .Where(b => b.UserId == guid)
                .ToList();
        }

        public decimal GetBasketTotal(string userId)
        {
            Guid.TryParse(userId, out var guid);
            var value = context.basketItems
                .Include(b => b.PrintOrder)
                .Where(b => b.UserId == guid)
                .Sum(b => b.PrintOrder.Value);
            return Math.Round(value, 2);
        }

        public async Task ChangeQuantity(string userId, int basketItemId, int newQuantity)
        {
            Guid.TryParse(userId, out var guid);
            var basketItem = await context.basketItems
                .Include(b => b.PrintOrder)
                .Where(b => b.UserId == guid && b.Id == basketItemId)
                .FirstOrDefaultAsync();
            if (basketItem == null) return;
            if (basketItem.PrintOrder.Quantity == newQuantity) return;
            basketItem.PrintOrder.Quantity = newQuantity;
            var paper = await context.Papers.FindAsync(basketItem.PrintOrder.PaperId);
            basketItem.PrintOrder.Value = GetPrice(basketItem.PrintOrder, paper);
            await context.SaveChangesAsync();
        }

        public async Task DeleteItem(string userId, int basketItemId)
        {
            await Task.Delay(5);
            Guid.TryParse(userId, out var guid);
            var basketItem = await context.basketItems.FindAsync(basketItemId);
            if (basketItem == null) return;
            context.basketItems.Remove(basketItem);
            await context.SaveChangesAsync();
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
                return (((decimal)(100 - discount) / 100) * pricePerPrint);
            }
            catch (Exception ex)
            {
                _logger.LogError("from ShoppingBasketService.GetQuantityDiscountPrice", ex);
                throw;
            }
        }
    }
}
