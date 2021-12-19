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
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System.Collections.Generic;

namespace BrPo.Website.Services.Image.Services
{
    public interface IImageService
    {
        Task<ImageFileModel> CreateImageRecord(string path, string userId, string originalFileName);
        string GetBase64(int id);
        string GetBase64Thumbnail(int id, int height = 300);
        Task<string> GetBase64ThumbnailAsync(int id, int height = 300);
        Task<ImageFileModel> GetImageAsync(int id);
        List<string> GetIds(string userId);
        List<ImageFileModel> GetImages(string userId);
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
            //model.Orientation = model.Height > model.Width ? "Portrait" : "Landscape";
            context.ImageFiles.Add(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<ImageFileModel> GetImageAsync(int id)
        {
            try
            {
                var imageFile = await context.ImageFiles.FindAsync(id);
                return imageFile ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError("from ImageService.GetImageAsync", ex);
                throw;
            }
        }

        private async Task GetImageMetaData(string path, string dateFormat, ImageFileModel model)
        {
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(path))
            {
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
                        model.Orientation = image.Metadata?.ExifProfile?.GetValue(ExifTag.Orientation)?.ToString()?.Replace("Rotate ", string.Empty);
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
                if (model.Orientation.Contains("CW")){
                    model.Width = image.Height;
                    model.Height = image.Width;
                }
                else
                {
                    model.Width = image.Width;
                    model.Height = image.Height;
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

        public string GetBase64Thumbnail(int id, int height = 170)
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

        public async Task<string> GetBase64ThumbnailAsync(int id, int height = 170)
        {
            var imageFile = await context.ImageFiles.FindAsync(id);
            RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
            switch(imageFile.Orientation)
            {
                case "90 CW":
                    rotate = RotateFlipType.Rotate90FlipNone;
                    break;
            }
            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(imageFile.Location))
                {
                    var ratio = (double)image.Width / image.Height;
                    var width = (int)(ratio * height);
                    using System.Drawing.Image thumbnail = ResizeImage(image, height, width);
                    using (MemoryStream m = new MemoryStream())
                    {
                        thumbnail.RotateFlip(rotate);
                        thumbnail.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
                //using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imageFile.Location))
                //{
                //    var ratio = (double)image.Width / image.Height;
                //    var width = (int)(ratio * height);
                //    var tmp = image.Mutate(x => x.Rotate(90));
                //}
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
    
        public List<string> GetIds(string userId)
        {
            try
            {
                var imageFiles = context.ImageFiles.Where(i => i.UserId == userId);
                if (imageFiles.Any())
                {
                    return imageFiles.Select(i => i.Id.ToString()).ToList<string>();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError("from ImageService.GetIds", ex);
                throw;
            }
        }

        public List<ImageFileModel> GetImages(string userId)
        {
            try
            {
                var imageFiles = context.ImageFiles.Where(i => i.UserId == userId);
                if (imageFiles.Any())
                {
                    return imageFiles.ToList<ImageFileModel>();
                }
                return new List<ImageFileModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError("from ImageService.GetIds", ex);
                throw;
            }
        }
    }
}
