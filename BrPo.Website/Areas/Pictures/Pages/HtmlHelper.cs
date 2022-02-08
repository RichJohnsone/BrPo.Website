using System.Text;
using BrPo.Website.Services.Image.Models;

namespace BrPo.Website.Areas.Pictures.Pages
{
    public static class HtmlHelpers
    {
        public static string GetImageTag(string base64, string name = "")
        {
            var html = new StringBuilder();
            html.AppendLine($"<div class=\"grid-image-container\"><img alt=\"{name}\" class=\"mt-1 mb-1\" src=\"data:image/png;base64,{base64}\" data-toggle=\"tooltip\" title=\"{name}\" /></div>");
            return html.ToString();
        }

        public static string GetGalleryItemNameTags(ImageGalleryItem imageGalleryItem)
        {
            var html = new StringBuilder();
            html.AppendLine("<table class=\"table-sm\">");
            html.AppendLine($"<span data-toggle=\"tooltip\" title=\"Gallery item name\">{imageGalleryItem.Name}</span>");
            html.AppendLine($"<tr><th class=\"\">Filename</th><td data-toggle=\"tooltip\" title=\"original filename\"><div class=\"grid-image-container\">{imageGalleryItem.ImageFile.OriginalFileName}<div></td></tr>");
            if (!string.IsNullOrEmpty(imageGalleryItem.ImageFile.ColourSpace))
                html.AppendLine($"<tr><th>Colour space</th><td>{imageGalleryItem.ImageFile.ColourSpace}</td></tr>");
            html.AppendLine("</tr></table>");
            return html.ToString();
        }

        public static string GetIsGalleryItemActiveTag(ImageGalleryItem imageGalleryItem)
        {
            var html = new StringBuilder();
            var label = imageGalleryItem.Id == 0 ? "No" : imageGalleryItem.IsActive ? "Active" : "Inactive";
            html.AppendLine($"{label}");
            return html.ToString();
        }

        public static string GetGalleryItemButtons(int galleryItemId, int imageFileId)
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

        public static string GetGalleryNameTags(ImageGallery imageGallery)
        {
            var html = new StringBuilder();
            html.AppendLine($"<span class=\"image-grid-ellipsis\" data-toggle=\"tooltip\" title=\"{imageGallery.Name}\">{imageGallery.Name}</span>");
            return html.ToString();
        }

        public static string GetGalleryDescription(string description)
        {
            var html = new StringBuilder();
            html.AppendLine($"<span class=\"image-grid-ellipsis\" data-toggle=\"tooltip\" title=\"{description}\">{description}</span>");
            return html.ToString();
        }

        public static string GetGalleryButtons(int galleryId)
        {
            var html = new StringBuilder();
            var deleteButtonClass = galleryId == 0 ? "d-none" : "";
            html.Append("<button class=\"btn-icon-sml btn-primary d-inline addOrEditButton-js\" ");
            html.Append("name=\"EditGallery\" ");
            html.Append("data-toggle=\"tooltip\" ");
            html.Append($"title=\"Edit gallery\">");
            html.Append($"<i class=\"fas fa-pen image-scroller-edit-button edit-js\" ");
            html.Append($"data-imageGalleryId=\"{galleryId}\"></i></button>");
            html.Append($"<button class=\"btn-icon-sml btn-danger d-inline ml-1 {deleteButtonClass}\" ");
            html.Append("name=\"DeleteGallery\" ");
            html.Append("data-toggle=\"tooltip\" ");
            html.Append("title=\"Delete gallery\">");
            html.Append("<i class=\"fas fa-times fa-lg image-scroller-delete-button delete-js\" ");
            html.Append($"data-imageGalleryId=\"{galleryId}\"></i></button>");
            return html.ToString();
        }
    }
}