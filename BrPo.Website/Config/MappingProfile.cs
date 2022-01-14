using AutoMapper;
using BrPo.Website.Services.ShoppingBasket.Models;

namespace BrPo.Website.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PrintOrderItem, PrintInvoiceItem>();
        }
    }
}