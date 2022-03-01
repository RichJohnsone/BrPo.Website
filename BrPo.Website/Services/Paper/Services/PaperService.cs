using BrPo.Website.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BrPo.Website.Services.Paper.Models;
using System.Collections.Generic;
using System.Linq;
using static BrPo.Website.Services.Paper.Models.Enums;

namespace BrPo.Website.Services.Paper.Services;

public interface IPaperService
{
    Task<PaperModel> CreatePaperRecord(PaperModel model);

    Task<PaperModel> GetPaperAsync(int id);

    List<PaperModel> GetPapers();

    string GetPaperName(int id);

    List<List<PaperEnumItem>> GetPaperEnums();
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

    public List<List<PaperEnumItem>> GetPaperEnums()
    {
        var papers = context.Papers.Where(p => p.IsActive).ToList();
        var returnList = new List<List<PaperEnumItem>>();
        var paperSurfaces = Enum.GetValues(typeof(PaperSurface)).Cast<PaperSurface>();
        var surfaceList = new List<PaperEnumItem>() { new PaperEnumItem() { Id = -1, Name = String.Empty, IsAvailable = true } };
        var i = 0;
        var id = 0;
        foreach (var paperSurface in paperSurfaces)
        {
            id = i++;
            surfaceList.Add(new PaperEnumItem()
            {
                Id = id,
                Name = paperSurface.ToString(),
                IsAvailable = papers.Any(p => (int)p.PaperSurface == id)
            });
        }
        returnList.Add(surfaceList);
        i = 0;
        var paperTextures = Enum.GetValues(typeof(PaperTexture)).Cast<PaperTexture>();
        var textureList = new List<PaperEnumItem>() { new PaperEnumItem() { Id = -1, Name = String.Empty, IsAvailable = true } };
        foreach (var paperTexture in paperTextures)
        {
            id = i++;
            textureList.Add(new PaperEnumItem()
            {
                Id = id,
                Name = paperTexture.ToString(),
                IsAvailable = papers.Any(p => (int)p.PaperSurface == id)
            });
        }
        returnList.Add(textureList);
        return returnList;
    }
}