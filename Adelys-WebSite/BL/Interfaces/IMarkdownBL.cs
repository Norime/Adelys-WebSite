using Microsoft.AspNetCore.Html;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IMarkdownBL
    {
        IHtmlContent Render(string? markdown);
        string? GetImageExtension(IFormFile file);
        bool IsImageFileName(string fileName);
        HashSet<string> GetReferencedImages(string? markdown);
    }
}
