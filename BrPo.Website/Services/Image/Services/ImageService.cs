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
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        string GetFileName(int id);

        Task<List<ImageGalleryItem>> GetGalleryItemsAsync(Guid userId);

        Task<ImageGalleryItem> GetGalleryItemAsync(int galleryItemId, Guid userId);

        Task<List<ImageGallery>> GetUserGalleriesAsync(Guid userId);

        Task AddOrUpdateGalleryItem(ImageGalleryItem galleryItem);

        List<ImageTag> GetImageTags(int galleryItemId);

        Task CreateImageTag(ImageTag imageTag);

        Task DeleteImageTag(int imageTagId);

        Task AddToGallery(ImageGalleryContent galleryContent);

        Task RemoveFromGallery(int galleryId, int galleryItemId);

        List<ImageGallery> GetGalleries(int galleryItemId);

        Task DeleteGalleryItemAsync(int galleryItemId, Guid userId);

        Task<List<ImageGallery>> GetGalleries();
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
                if (model.Orientation.Contains("CW"))
                {
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
            if (imageFile == null) return null;
            RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
            switch (imageFile.Orientation)
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

        public string GetFileName(int id)
        {
            return context.ImageFiles.FirstOrDefault(i => i.Id == id).OriginalFileName;
        }

        public async Task<List<ImageGalleryItem>> GetGalleryItemsAsync(Guid userId)
        {
            await Task.Delay(1);
            var items = context.ImageGalleryItems
                .Include(g => g.ImageFile)
                .Where(g => g.UserId == userId);
            if (items.Any())
            {
                return items.ToList();
            }
            else
            {
                return new List<ImageGalleryItem>();
            }
        }

        public async Task<ImageGalleryItem> GetGalleryItemAsync(int galleryItemId, Guid userId)
        {
            return await context.ImageGalleryItems
                 .Include(g => g.ImageFile)
                 .Include(g => g.Tags)
                 .FirstOrDefaultAsync(g => g.UserId == userId && g.Id == galleryItemId);
        }

        public async Task<List<ImageGallery>> GetUserGalleriesAsync(Guid userId)
        {
            var galleries = context.ImageGalleries
                 .Where(g => g.UserId == userId);
            if (galleries.Any())
            {
                return galleries.ToList();
            }
            else
            {
                var _galleries = new List<ImageGallery>();
                var defaultGallery = new ImageGallery() { Name = "Default", DateCreated = DateTime.UtcNow, UserId = userId };
                context.ImageGalleries.Add(defaultGallery);
                await context.SaveChangesAsync();
                _galleries.Add(defaultGallery);
                return _galleries;
            }
        }

        public async Task AddOrUpdateGalleryItem(ImageGalleryItem galleryItem)
        {
            if (galleryItem == null) return;
            if (galleryItem.Id == 0)
            {
                galleryItem.DateCreated = (DateTime)galleryItem.UpdatedDate;
                context.ImageGalleryItems.Add(galleryItem);
            }
            else
            {
                var entity = context.ImageGalleryItems.Find(galleryItem.Id);
                if (entity != null)
                {
                    if (entity.UserId != galleryItem.UserId) throw new ApplicationException("Attempt to update gallery item with incorrect userId");
                    var entry = context.Entry(entity);
                    entry.CurrentValues.SetValues(galleryItem);
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<List<ImageGallery>> GetGalleries()
        {
            var galleries = context.ImageGalleries
                .Where(g => g.Content.Count() > 0)
                .Include(g => g.Content)
                .ToList();
            return galleries;
        }

        public List<ImageTag> GetImageTags(int galleryItemId)
        {
            var imageTags = context.ImageTags.Where(i => i.ImageGalleryItemId == galleryItemId);
            if (imageTags.Any())
                return imageTags.ToList();
            else
                return new List<ImageTag> { };
        }

        public async Task CreateImageTag(ImageTag imageTag)
        {
            var entity = context.ImageTags.FirstOrDefault(i => i.ImageGalleryItemId == imageTag.ImageGalleryItemId && i.Tag == imageTag.Tag);
            if (entity != null) throw new ApplicationException("Tag already exists");
            context.ImageTags.Add(imageTag);
            await context.SaveChangesAsync();
        }

        public async Task DeleteImageTag(int imageTagId)
        {
            var imageTag = context.ImageTags.Find(imageTagId);
            if (imageTag != null)
            {
                context.ImageTags.Remove(imageTag);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddToGallery(ImageGalleryContent galleryContent)
        {
            var entity = context.ImageGalleryContents.FirstOrDefault(
                i => i.ImageGalleryItemId == galleryContent.ImageGalleryItemId
                     && i.ImageGalleryId == galleryContent.ImageGalleryId);
            if (entity != null) throw new ApplicationException("Gallery entry already exists");
            context.ImageGalleryContents.Add(galleryContent);
            await context.SaveChangesAsync();
        }

        public async Task RemoveFromGallery(int galleryId, int galleryItemId)
        {
            var entity = context.ImageGalleryContents.FirstOrDefault(
                i => i.ImageGalleryItemId == galleryItemId
                     && i.ImageGalleryId == galleryId);
            if (entity != null)
            {
                context.ImageGalleryContents.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public List<ImageGallery> GetGalleries(int galleryItemId)
        {
            var contents = context.ImageGalleryContents.Where(
                c => c.ImageGalleryItemId == galleryItemId);
            var galleryIds = contents.Select(c => c.ImageGalleryId).Distinct().ToList();
            if (!galleryIds.Any()) return new List<ImageGallery>();
            var galleries = context.ImageGalleries.Where(g => galleryIds.Contains(g.Id)).ToList();
            galleries.ForEach(g => g.Content = null);
            return galleries;
        }

        public async Task DeleteGalleryItemAsync(int galleryItemId, Guid userId)
        {
            var entity = context.ImageGalleryItems.FirstOrDefault(
                i => i.Id == galleryItemId
                     && i.UserId == userId);
            if (entity == null) throw new ApplicationException("Gallery entry not found");
            var galleryContents = context.ImageGalleryContents.Where(c => c.ImageGalleryItemId == galleryItemId).ToList();
            context.ImageGalleryContents.RemoveRange(galleryContents);
            var tags = context.ImageTags.Where(c => c.ImageGalleryItemId == galleryItemId).ToList();
            context.ImageTags.RemoveRange(tags);
            context.ImageGalleryItems.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}