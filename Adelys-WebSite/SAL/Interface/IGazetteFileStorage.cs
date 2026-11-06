namespace Adelys_WebSite.SAL.Interface
{
    public interface IGazetteFileStorage
    {
        Task<string> SavePdf(IFormFile file);
        Task<string> SaveCover(IFormFile file);
        string ImportFile(string sourcePath);
        string GetPath(string fileName);
        void Delete(string? fileName);
    }
}
