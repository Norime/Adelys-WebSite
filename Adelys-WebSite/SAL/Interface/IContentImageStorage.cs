namespace Adelys_WebSite.SAL.Interface
{
    public interface IContentImageStorage
    {
        Task<string> Save(IFormFile file, string extension);
        string GetPath(string fileName);
        void Delete(string? fileName);
    }
}
