using System.Text;
using BrPo.Website.Services.Image.Models;

namespace BrPo.Website.Areas.Sell.Pages;

public static class HtmlHelpers
{
    public static string GetGalleryItemImageTag(string base64, string name = "")
    {
        var html = new StringBuilder();
        html.AppendLine($"<div class=\"grid-image-container\"><img alt=\"{name}\" class=\"mt-1 mb-1\" src=\"data:image/png;base64,{base64}\" data-toggle=\"tooltip\" title=\"Galley item name: {name}\" /></div>");
        return html.ToString();
    }

    public static string GetGalleryItemNameTags(ImageGalleryItem imageGalleryItem)
    {
        var html = new StringBuilder();
        html.AppendLine("<table class=\"table-sm\">");
        html.AppendLine($"<span class=\"image-grid-ellipsis\" data-toggle=\"tooltip\" title=\"Gallery item name: {imageGalleryItem.Name}\">{imageGalleryItem.Name}</span>");
        html.AppendLine($"<tr><td data-toggle=\"tooltip\" title=\"original filename\"><div class=\"\">{imageGalleryItem.ImageFile.OriginalFileName}<div></td></tr>");
        if (!string.IsNullOrEmpty(imageGalleryItem.ImageFile.ColourSpace))
            html.AppendLine($"<tr><td data-toggle=\"tooltip\" title=\"Colour space\">{imageGalleryItem.ImageFile.ColourSpace}</td></tr>");
        html.AppendLine("</tr></table>");
        return html.ToString();
    }

    public static string GetIsGalleryItemActiveTag(ImageGalleryItem imageGalleryItem)
    {
        var html = new StringBuilder();
        var label = imageGalleryItem.Id == 0
            ? "<span data-toggle=\"tooltip\" title=\"This image is not a gallery item\">No</span>"
            : imageGalleryItem.IsActive
                ? "<span data-toggle=\"tooltip\" title=\"This image is a active gallery item\">Active</span>"
                : "<span data-toggle=\"tooltip\" title=\"This image is a inactive gallery item, it will not be publicly available in any galleries\">Inactive</span>";
        html.AppendLine($"{label}");
        return html.ToString();
    }

    public static string GetGalleryItemButtons(int galleryItemId, int imageFileId)
    {
        var html = new StringBuilder();
        var title = galleryItemId != 0 ? "Edit" : "Make this file a ";
        var iconClass = galleryItemId == 0 ? "fa-plus" : "fa-pen";
        var deleteButtonClass = galleryItemId == 0 ? "d-none" : "";
        html.Append("<button class=\"btn-icon-sml btn-primary mr-3 mt-1\" ");
        html.Append("name=\"AddOrEditItem\" ");
        html.Append("data-toggle=\"tooltip\" ");
        html.Append($"title=\"{title} gallery item\">");
        html.Append($"<i class=\"fas {iconClass} image-scroller-edit-button edit-js\" ");
        html.Append($"data-imageGalleryItemId=\"{galleryItemId}\" ");
        html.Append($"data-imageFileId=\"{imageFileId}\"></i></button>");
        html.Append($"<button class=\"btn-icon-sml btn-danger mr-3 mt-1 {deleteButtonClass}\" ");
        html.Append("name=\"DeleteItem\" ");
        html.Append("data-toggle=\"tooltip\" ");
        html.Append("title=\"Delete gallery item\">");
        html.Append("<i class=\"fas fa-times fa-lg image-scroller-delete-button delete-js\" ");
        html.Append($"data-imageGalleryItemId=\"{galleryItemId}\"");
        html.Append($"data-imageFileId=\"{imageFileId}\"></i></button>");
        return html.ToString();
    }

    public static string GetGalleryImageTag(string base64, string name = "")
    {
        var html = new StringBuilder();
        html.AppendLine($"<div class=\"grid-image-container\"><img alt=\"{name}\" class=\"mt-1 mb-1\" src=\"data:image/png;base64,{base64}\" data-toggle=\"tooltip\" title=\"Cover image name: {name}\" /></div>");
        return html.ToString();
    }

    public static string GetGalleryNameTags(ImageGallery imageGallery)
    {
        var html = new StringBuilder();
        html.AppendLine($"<span class=\"image-grid-ellipsis\" data-toggle=\"tooltip\" title=\"Gallery name: {imageGallery.Name}\">{imageGallery.Name}</span>");
        return html.ToString();
    }

    public static string GetIsGalleryActiveTag(bool isActive)
    {
        var html = new StringBuilder();
        var label = isActive
            ? "<span data-toggle=\"tooltip\" title=\"This gallery is active\">Yes</span>"
            : "<span data-toggle=\"tooltip\" title=\"This gallery is inactive, it will not be publicly available\">No</span>";
        html.AppendLine($"{label}");
        return html.ToString();
    }

    public static string GetDescription(string description)
    {
        var html = new StringBuilder();
        html.AppendLine($"<span class=\"image-grid-ellipsis\" data-toggle=\"tooltip\" title=\"{description}\">{description}</span>");
        return html.ToString();
    }

    public static string GetGalleryButtons(int galleryId)
    {
        var html = new StringBuilder();
        var deleteButtonClass = galleryId == 0 ? "d-none" : "";
        html.Append("<button class=\"btn-icon-sml btn-primary addOrEditButton-js mr-3 mt-1\" ");
        html.Append("name=\"EditGallery\" ");
        html.Append("data-toggle=\"tooltip\" ");
        html.Append($"title=\"Edit gallery\">");
        html.Append($"<i class=\"fas fa-pen image-scroller-edit-button edit-js\" ");
        html.Append($"data-imageGalleryId=\"{galleryId}\"></i></button>");
        html.Append($"<button class=\"btn-icon-sml btn-danger  mr-3 mt-1 {deleteButtonClass}\" ");
        html.Append("name=\"DeleteGallery\" ");
        html.Append("data-toggle=\"tooltip\" ");
        html.Append("title=\"Delete gallery\">");
        html.Append("<i class=\"fas fa-times fa-lg image-scroller-delete-button delete-js\" ");
        html.Append($"data-imageGalleryId=\"{galleryId}\"></i></button>");
        return html.ToString();
    }
}