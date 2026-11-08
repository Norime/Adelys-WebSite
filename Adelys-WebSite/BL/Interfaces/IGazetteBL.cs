using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL.Interfaces
{
    public interface IGazetteBL
    {
        List<Gazette> GetAllGazettes();
        Gazette? GetGazette(string slug);
        string GetFilePath(string fileName);
        bool IsPdf(IFormFile file);
        bool IsJpeg(IFormFile file);
        Task<Gazette> CreateGazette(GazetteFormViewModel form, string uploadedBy);
        Task UpdateGazette(Gazette gazette, GazetteFormViewModel form);
        void DeleteGazette(Gazette gazette);
    }
}
