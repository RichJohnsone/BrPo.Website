using BrPo.Website.Data;
using BrPo.Website.Services.Image.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace BrPo.Website.Services.Image.Services
{
    public interface IImageService
    {
        Task<ImageFileModel> CreateImageRecord(string path, string userId, string originalFileName);
        string GetBase64(int id);
        string GetBase64Thumbnail(int id, int height = 300);
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

        public async Task<ImageFileModel> CreateImageRecord(string path, string userId, string originalFileName)
        {
            string dateFormat = "yyyy:MM:dd HH:mm:ss"; // TODO check that MM and dd are the correct way round with suitable input file
            var model = new ImageFileModel();
            model.OriginalFileName = originalFileName;
            model.UserId = userId;
            model.Location = path;
            await GetImageMetaData(path, dateFormat, model);
            model.DateCreated = DateTime.UtcNow;
            context.ImageFiles.Add(model);
            await context.SaveChangesAsync();
            return model;
        }

        private async Task GetImageMetaData(string path, string dateFormat, ImageFileModel model)
        {
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(path))
            {
                model.Width = image.Width;
                model.Height = image.Height;
                try
                {
                    if (image.Metadata.IptcProfile?.Values?.Any() ?? false)
                    {
                        foreach (var prop in image.Metadata.IptcProfile.Values)
                        {
                            if (prop.Tag.ToString() == "Creator") model.Creator = prop.Value;
                            if (prop.Tag.ToString() == "Description") model.Description = prop.Value;
                            if (prop.Tag.ToString() == "Keywords") model.Keywords = prop.Value;
                            if (prop.Tag.ToString() == "Credit") model.Credit = prop.Value;
                            if (prop.Tag.ToString() == "DateTimeOriginal")
                            {
                                if (DateTime.TryParseExact(prop.Value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                                    model.ImageCreatedDate = date;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("from ImageService.GetImageMetaData IptcPrifile", ex);
                }
                try
                {
                    if (image.Metadata.ExifProfile?.Values?.Any() ?? false)
                    {
                        foreach (var prop in image.Metadata.ExifProfile.Values)
                        {
                            if (prop.Tag.ToString() == "ColorSpace")
                                model.ColourSpace = prop.ToString();
                            if (prop.Tag.ToString() == "DateTimeOriginal")
                            {
                                DateTime date;
                                if (DateTime.TryParseExact(prop.ToString(), dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                    model.ImageCreatedDate = date;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("from ImageService.GetImageMetaData Exif", ex);
                }
            }
        }

        public string GetBase64(int id)
        {
            var imageFile = context.ImageFiles.FindAsync(id).Result;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imageFile.Location))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
        }

        public string GetBase64Thumbnail(int id, int height = 300)
        {
            var imageFile = context.ImageFiles.FindAsync(id).Result;
            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(imageFile.Location))
                {
                    var ratio = (double)image.Width / image.Height;
                    var width = (int)(ratio * height);
                    using var thumbnail = ResizeImage(image, height, width);
                    using (MemoryStream m = new MemoryStream())
                    {
                        thumbnail.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError("from ImageService.GetBase64Thumbnail", ex);
                throw;
            }
        }

        private System.Drawing.Image ResizeImage(System.Drawing.Image image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            using Graphics g = Graphics.FromImage((System.Drawing.Image)new_image);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return (System.Drawing.Image)new_image;
        }
    }
}
