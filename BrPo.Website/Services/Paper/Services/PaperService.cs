using BrPo.Website.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BrPo.Website.Services.Paper.Models;
using System.Collections.Generic;
using System.Linq;

namespace BrPo.Website.Services.Paper.Services
{
    public interface IPaperService
    {
        Task<PaperModel> CreatePaperRecord(PaperModel model);
        Task<PaperModel> GetPaperAsync(int id);
        List<PaperModel> GetPapers();
        string GetPaperName(int id);
    }

    public class PaperService : IPaperService
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<PaperService> _logger;

        public PaperService(
            ApplicationDbContext applicationDbContext,
            ILogger<PaperService> logger)
        {
            context = applicationDbContext;
            _logger = logger;
        }

        public async Task<PaperModel> CreatePaperRecord(PaperModel model)
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
                _logger.LogError("from PaperService.GetPaperAsync", ex);
                throw;
            }
        }

        public List<PaperModel> GetPapers()
        {
            try
            {
                var paperModels = context.Papers.ToList<PaperModel>();
                return paperModels ?? new List<PaperModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError("from PaperService.GetPaperAsync", ex);
                throw;
            }
        }

        public string GetPaperName(int id)
        {
            return context.Papers.FirstOrDefault(p => p.Id == id).Name;
        }
    }
}
