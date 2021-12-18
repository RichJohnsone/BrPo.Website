using BrPo.Website.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BrPo.Website.Services.Paper.Models;

namespace BrPo.Website.Services.Paper.Services
{
    public interface IImageService
    {
        Task<PaperModel> CreatepaperRecord(PaperModel model);
        Task<PaperModel> GetPaperAsync(int id);
    }

    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<ImageService> _logger;

        public ImageService(
            ApplicationDbContext applicationDbContext,
            ILogger<ImageService> logger)
        {
            context = applicationDbContext;
            _logger = logger;
        }

        public async Task<PaperModel> CreatepaperRecord(PaperModel model)
        {
            model.DateCreated = DateTime.UtcNow;
            context.Papers.Add(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<PaperModel> GetPaperAsync(int id)
        {
            try
            {
                var paperModel = await context.Papers.FindAsync(id);
                return paperModel ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError("from PaperService.GetImageAsync", ex);
                throw;
            }
        }
    }
}
