using Adelys_WebSite.SAL.Interface;

namespace Adelys_WebSite.SAL
{
    /// <summary>
    /// Range les fichiers de la gazette hors de wwwroot, sous un nom généré par le site
    /// </summary>
    public class GazetteFileStorage : IGazetteFileStorage
    {
        private readonly string _rootPath;

        public GazetteFileStorage(IConfiguration configuration, IWebHostEnvironment environment)
        {
            string storagePath = configuration["Gazette:StoragePath"] ?? "App_Data/Gazette";
            _rootPath = Path.GetFullPath(storagePath, environment.ContentRootPath);
            Directory.CreateDirectory(_rootPath);
        }

        public Task<string> SavePdf(IFormFile file)
        {
            return Save(file, ".pdf");
        }

        public Task<string> SaveCover(IFormFile file)
        {
            return Save(file, ".jpg");
        }

        public string ImportFile(string sourcePath)
        {
            string fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(sourcePath);
            File.Copy(sourcePath, GetPath(fileName));
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

        private async Task<string> Save(IFormFile file, string extension)
        {
            string fileName = Guid.NewGuid().ToString("N") + extension;
            await using FileStream stream = new FileStream(GetPath(fileName), FileMode.CreateNew);
            await file.CopyToAsync(stream);
            return fileName;
        }
    }
}
