using Adelys_WebSite.SAL.Interface;

namespace Adelys_WebSite.SAL
{
    /// <summary>
    /// Range les images des textes du site (règlement, events) hors de wwwroot, sous un nom généré par le site
    /// </summary>
    public class ContentImageStorage : IContentImageStorage
    {
        private readonly string _rootPath;

        public ContentImageStorage(IConfiguration configuration, IWebHostEnvironment environment)
        {
            string storagePath = configuration["Images:StoragePath"] ?? "App_Data/Images";
            _rootPath = Path.GetFullPath(storagePath, environment.ContentRootPath);
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> Save(IFormFile file, string extension)
        {
            string fileName = Guid.NewGuid().ToString("N") + extension;
            await using FileStream stream = new FileStream(GetPath(fileName), FileMode.CreateNew);
            await file.CopyToAsync(stream);
            return fileName;
        }

        public string GetPath(string fileName)
        {
            return Path.Combine(_rootPath, fileName);
        }

        public void Delete(string? fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                File.Delete(GetPath(fileName));
            }
        }
    }
}
