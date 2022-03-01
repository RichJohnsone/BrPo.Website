using AutoMapper;
using BrPo.Website.Areas.ShoppingBasket.Models;
using BrPo.Website.Data;
using BrPo.Website.Services.Email;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.ShoppingBasket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrPo.Website.Services.ShoppingBasket.Services;

public interface IShoppingBasketService
{
    Task CreatePrintOrderItemAsync(PrintOrderItem printOrder);

    Task AddPrintOrderItemToBasketAsync(PrintOrderItem printOrder);

    bool PriceIsCorrect(PrintOrderItem printOrder, PaperModel paper);

    int GetBasketCount(string userId);

    List<BasketItem> GetBasketItems(string userId);

    decimal GetBasketTotal(string userId);

    Task ChangeQuantityAsync(string userId, int basketItemId, int newQuantity);

    Task DeleteItemAsync(string userId, int basketItemId);

    Task<Invoice> CreateInvoiceAsync(string userId, CreateInvoiceModel createInvoiceModel);

    Task<Invoice> GetInvoiceAsync(int invoiceId, string userId);

    Task MarkInvoiceAsPaidAsync(int invoiceId, string userId, string paymentIntentId);

    Task<Decimal> GetInvoiceTotalAsync(int invoiceId, string userId);
}

public class ShoppingBasketService : IShoppingBasketService
{
    private readonly ApplicationDbContext context;
    private readonly ILogger<ShoppingBasketService> _logger;
    private readonly IMapper _mapper;

    public ShoppingBasketService(
        ApplicationDbContext applicationDbContext,
        ILogger<ShoppingBasketService> logger,
        IMapper mapper)
    {
        context = applicationDbContext;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task CreatePrintOrderItemAsync(PrintOrderItem printOrder)
    {
        try
        {
            context.PrintOrderItems.Add(printOrder);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("from ShoppingBasketService.CreatePrintOrderAsync", ex);
            throw;
        }
    }

    public async Task AddPrintOrderItemToBasketAsync(PrintOrderItem printOrder)
    {
        try
        {
            var basketItem = new BasketItem();
            basketItem.UserId = printOrder.UserId;
            basketItem.PrintOrderItemId = printOrder.Id;
            context.basketItems.Add(basketItem);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("from ShoppingBasketService.AddPrintOrderToBasketAsync", ex);
            throw;
        }
    }

    public bool PriceIsCorrect(PrintOrderItem printOrder, PaperModel paper)
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
            .Include(b => b.PrintOrderItem)
            .Where(b => b.UserId == guid)
            .ToList();
    }

    public decimal GetBasketTotal(string userId)
    {
        Guid.TryParse(userId, out var guid);
        var value = context.basketItems
            .Include(b => b.PrintOrderItem)
            .Where(b => b.UserId == guid)
            .Sum(b => b.PrintOrderItem.Value);
        return Math.Round(value, 2);
    }

    public async Task ChangeQuantityAsync(string userId, int basketItemId, int newQuantity)
    {
        Guid.TryParse(userId, out var guid);
        var basketItem = await context.basketItems
            .Include(b => b.PrintOrderItem)
            .Where(b => b.UserId == guid && b.Id == basketItemId)
            .FirstOrDefaultAsync();
        if (basketItem == null) return;
        if (basketItem.PrintOrderItem.Quantity == newQuantity) return;
        basketItem.PrintOrderItem.Quantity = newQuantity;
        var paper = await context.Papers.FindAsync(basketItem.PrintOrderItem.PaperId);
        basketItem.PrintOrderItem.Value = GetPrice(basketItem.PrintOrderItem, paper);
        await context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(string userId, int basketItemId)
    {
        Guid.TryParse(userId, out var guid);
        var basketItem = await context.basketItems.FindAsync(basketItemId);
        if (basketItem == null) return;
        context.basketItems.Remove(basketItem);
        await context.SaveChangesAsync();
    }

    public async Task<Invoice> CreateInvoiceAsync(string userId, CreateInvoiceModel createInvoiceModel)
    {
        var invoice = await CreateInvoiceRecordAsync(userId, createInvoiceModel);
        await CreateInviceItemRecordsAsync(userId, invoice.Id);
        return invoice;
    }

    private async Task<Invoice> CreateInvoiceRecordAsync(string userId, CreateInvoiceModel createInvoiceModel)
    {
        try
        {
            Guid.TryParse(userId, out var guid);
            var invoice = new Invoice(guid, GetBasketTotal(userId), createInvoiceModel.VoucherDiscount, createInvoiceModel.Delivery);
            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();
            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError("from ShoppingBasketService.CreateInvoiceRecordAsync", ex);
            throw new ApplicationException($"There was an error creating the invoice for this order:" +
                                           $"" +
                                           $" {ex.ToMessageString()}");
        }
    }

    private async Task CreateInviceItemRecordsAsync(string userId, int invoiceId)
    {
        try
        {
            foreach (var basketItem in GetBasketItems(userId))
            {
                var invoiceItem = _mapper.Map<PrintInvoiceItem>(basketItem.PrintOrderItem);
                invoiceItem.Id = 0;
                invoiceItem.InvoiceId = invoiceId;
                context.PrintInvoiceItems.Add(invoiceItem);
            }
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("from ShoppingBasketService.CreateInviceItemRecordsAsync", ex);
            throw new ApplicationException($"There was an error creating the invoice items for this order:" +
                                           $"" +
                                           $" {ex.ToMessageString()}");
        }
    }

    public async Task<Invoice> GetInvoiceAsync(int invoiceId, string userId)
    {
        Guid.TryParse(userId, out var guid);
        var invoice = await context.Invoices
            .Include(b => b.Items)
            .Where(b => b.UserId == guid && b.Id == invoiceId)
            .FirstOrDefaultAsync();
        if (invoice == null) return null;
        return invoice;
    }

    public async Task MarkInvoiceAsPaidAsync(int invoiceId, string userId, string paymentIntentId)
    {
        try
        {
            var invoice = await GetInvoiceAsync(invoiceId, userId);
            if (invoice == null)
            {
                // replace with logging
                throw new ApplicationException("failed to recall invoice - one of the supplied id's is incorrect");
            }
            var entity = context.Invoices.FirstOrDefault(i => i.Id == invoice.Id);
            if (entity != null)
            {
                entity.PaymentDate = DateTime.UtcNow;
                entity.IsComplete = true;
                entity.StrikePaymentId = paymentIntentId;
                await context.SaveChangesAsync();
                await this.Emptybasket(userId);
            }
        }
        catch (Exception ex)
        {
            var message = $"invoiceId; {invoiceId}, userId: {userId}, paymentIntentId: {paymentIntentId}";
            _logger.LogError($"from ShoppingBasketService.MarkInvoiceAsPaidAsync: {message}", ex);
        }
    }

    public async Task<Decimal> GetInvoiceTotalAsync(int invoiceId, string userId)
    {
        Guid.TryParse(userId, out var guid);
        var invoice = await context.Invoices
            .Include(b => b.Items)
            .Where(b => b.UserId == guid && b.Id == invoiceId)
            .FirstOrDefaultAsync();
        if (invoice == null) return 0;
        return invoice.Total;
    }

    private async Task Emptybasket(string userId)
    {
        try
        {
            context.basketItems.RemoveRange(this.GetBasketItems(userId));
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            var message = $"userId: {userId}";
            _logger.LogError($"from ShoppingBasketService.Emptybasket: {message}", ex);
        }
    }

    private decimal GetPrice(PrintOrderItem printOrder, PaperModel paper)
    {
        try
        {
            if (paper.RollPaper == true)
            {
                var paperLength = printOrder.Height > printOrder.Width ? printOrder.Height + 100 : printOrder.Width + 100;
                var pricePerPrint = ((Decimal)paperLength / 1000) * paper.CostPerMeter;
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