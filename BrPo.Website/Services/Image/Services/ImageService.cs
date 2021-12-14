using BrPo.Website.Data;
using BrPo.Website.Services.Image.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace BrPo.Website.Services.Image.Services
{
    public interface IImageService
    {
        Task<ImageFileModel> CreateImageRecord(string path, string userId, string originalFileName);
    }

    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext context;

        public ImageService(ApplicationDbContext applicationDbContext)
        {
            context = applicationDbContext;
        }

        public async Task<ImageFileModel> CreateImageRecord(string path, string userId, string originalFileName)
        {
            var model = new ImageFileModel();
            model.OriginalFileName = originalFileName;
            model.UserId = userId;
            model.Location = path;
            using (var fs = System.IO.File.OpenRead(path))
            using (var bitmap = new Bitmap(fs))
            {
                model.Width = bitmap.Width;
                model.Height = bitmap.Height;
            }
            model.DateCreated = DateTime.UtcNow;
            context.ImageFiles.Add(model);
            await context.SaveChangesAsync();
            return model;
        }
    }
}
