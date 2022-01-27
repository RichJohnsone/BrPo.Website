using System.Text;
using BrPo.Website.Services.Image.Models;

namespace BrPo.Website.Areas.Pictures.Pages
{
    public static class HtmlHelpers
    {
        public static string GetImageTag(string base64)
        {
            var html = new StringBuilder();
            html.AppendLine($"<img class=\"mt-1 mb-1\" src=\"data:image/png;base64,{base64}\" />");
            return html.ToString();
        }

        public static string GetNameTags(ImageGalleryItem imageGalleryItem)
        {
            var html = new StringBuilder();
            html.AppendLine("<table class=\"table-sm\">");
            html.AppendLine($"<span data-toggle=\"tooltip\" title=\"Gallery item name\">{imageGalleryItem.Name}</span>");
            html.AppendLine($"<tr><th class=\"\">Filename</th><td data-toggle=\"tooltip\" title=\"original filename\">{imageGalleryItem.ImageFile.OriginalFileName}</td></tr>");
            //html.AppendLine($"<tr><th>Dimensions</th><td><span data-toggle=\"tooltip\" title=\"Height in pixels\">{imageGalleryItem.ImageFile.Height}</span>* <span data-toggle=\"tooltip\" title=\"Width in pixels\">{imageGalleryItem.ImageFile.Width}</span></td></tr>");
            if (!string.IsNullOrEmpty(imageGalleryItem.ImageFile.ColourSpace))
                html.AppendLine($"<tr><th>Colour space</th><td>{imageGalleryItem.ImageFile.ColourSpace}</td></tr>");
            //if (imageGalleryItem.ImageFile.ImageCreatedDate != null)
            //    html.AppendLine($"<tr><th>Created</th><td>{imageGalleryItem.ImageFile.ImageCreatedDate.ToString()}</td></tr>");
            //if (imageGalleryItem.ImageFile.Orientation != null)
            //    html.AppendLine($"<tr><th>Orientation</th><td>{imageGalleryItem.ImageFile.Orientation}</td>");
            html.AppendLine("</tr></table>");
            return html.ToString();
        }

        public static string GetIsActiveTag(ImageGalleryItem imageGalleryItem)
        {
            var html = new StringBuilder();
            var label = imageGalleryItem.Id == 0 ? "No" : imageGalleryItem.IsActive ? "Active" : "Inactive";
            html.AppendLine($"{label}");
            return html.ToString();
        }

        public static string GetButtons(int galleryItemId, int imageFileId)
        {
            var html = new StringBuilder();
            var title = galleryItemId != 0 ? "Edit" : "Make this file a ";
            var iconClass = galleryItemId == 0 ? "fa-plus" : "fa-pen";
            var deleteButtonClass = galleryItemId == 0 ? "d-none" : "";
            html.Append("<button class=\"btn btn-primary mr-3 mt-1 addOrEditButton-js\" ");
            html.Append("name=\"AddOrEditItem\" ");
            html.Append("data-toggle=\"tooltip\" ");
            html.Append($"title=\"{title} gallery item\" ");
            html.Append($"data-imageGalleryItemId=\"{galleryItemId}\" ");
            html.Append($"data-fileId=\"{imageFileId}\">");
            html.Append($"<i class=\"fas {iconClass} fa-lg\"></i></button>");
            html.Append($"<button class=\"btn btn-danger mr-3 mt-1 deleteButton-js {deleteButtonClass}\" ");
            html.Append("name=\"DeleteItem\" ");
            html.Append("data-toggle=\"tooltip\" ");
            html.Append("title=\"Delete gallery item\" ");
            html.Append($"data-imageGalleryItemId=\"{galleryItemId}\"");
            html.Append($"data-fileId=\"{imageFileId}\">");
            html.Append("<i class=\"fas fa-trash fa-lg\"></i></button>");
            return html.ToString();
        }
    }
}