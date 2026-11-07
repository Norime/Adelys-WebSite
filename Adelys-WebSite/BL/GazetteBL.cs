using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.ViewModels;
using System.Text;

namespace Adelys_WebSite.BL
{
    public class GazetteBL : IGazetteBL
    {
        public const long MaxPdfBytes = 100L * 1024 * 1024;
        public const long MaxCoverBytes = 5L * 1024 * 1024;
        public const long MaxRequestBytes = MaxPdfBytes + MaxCoverBytes + 1024 * 1024;

        private static readonly byte[] PdfSignature = Encoding.ASCII.GetBytes("%PDF-");
        private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF };

        private readonly IGazetteRepository _repository;
        private readonly IGazetteFileStorage _fileStorage;

        public GazetteBL(IGazetteRepository repository, IGazetteFileStorage fileStorage)
        {
            _repository = repository;
            _fileStorage = fileStorage;
        }

        public List<Gazette> GetAllGazettes()
        {
            return _repository.GetAll();
        }

        public Gazette? GetGazette(string slug)
        {
            return _repository.GetBySlug(slug);
        }

        public string GetFilePath(string fileName)
        {
            return _fileStorage.GetPath(fileName);
        }

        public bool IsPdf(IFormFile file)
        {
            return StartsWith(file, PdfSignature);
        }

        public bool IsJpeg(IFormFile file)
        {
            return StartsWith(file, JpegSignature);
        }

        public async Task<Gazette> CreateGazette(GazetteFormViewModel form, string uploadedBy)
        {
            Gazette gazette = new Gazette
            {
                Title = form.Title,
                Slug = BuildUniqueSlug(form.Title),
                Description = form.Description,
                PublishedOn = form.PublishedOn!.Value.Date,
                UploadedBy = uploadedBy,
                CreatedAtUtc = DateTime.UtcNow,
                PdfSizeBytes = form.Pdf!.Length,
                PdfFileName = await _fileStorage.SavePdf(form.Pdf)
            };

            try
            {
                if (form.Cover != null)
                {
                    gazette.CoverFileName = await _fileStorage.SaveCover(form.Cover);
                }
                _repository.Create(gazette);
            }
            catch
            {
                _fileStorage.Delete(gazette.PdfFileName);
                _fileStorage.Delete(gazette.CoverFileName);
                throw;
            }
            return gazette;
        }

        /// <summary>
        /// Modifie un numéro, l'adresse reste la même pour ne pas casser les liens déjà partagés
        /// </summary>
        public async Task UpdateGazette(Gazette gazette, GazetteFormViewModel form)
        {
            string? newPdf = form.Pdf != null ? await _fileStorage.SavePdf(form.Pdf) : null;
            string? newCover = null;
            string? oldPdf = gazette.PdfFileName;
            string? oldCover = gazette.CoverFileName;

            try
            {
                newCover = form.Cover != null ? await _fileStorage.SaveCover(form.Cover) : null;
                gazette.Title = form.Title;
                gazette.Description = form.Description;
                gazette.PublishedOn = form.PublishedOn!.Value.Date;
                if (newPdf != null)
                {
                    gazette.PdfFileName = newPdf;
                    gazette.PdfSizeBytes = form.Pdf!.Length;
                    gazette.CoverFileName = newCover;
                }
                else if (newCover != null)
                {
                    gazette.CoverFileName = newCover;
                }
                _repository.Update(gazette);
            }
            catch
            {
                _fileStorage.Delete(newPdf);
                _fileStorage.Delete(newCover);
                throw;
            }

            if (newPdf != null)
            {
                _fileStorage.Delete(oldPdf);
            }
            if (gazette.CoverFileName != oldCover)
            {
                _fileStorage.Delete(oldCover);
            }
        }

        public void DeleteGazette(Gazette gazette)
        {
            _repository.Delete(gazette);
            _fileStorage.Delete(gazette.PdfFileName);
            _fileStorage.Delete(gazette.CoverFileName);
        }

        private string BuildUniqueSlug(string title)
        {
            return SlugHelper.MakeUnique(SlugHelper.Slugify(title, "gazette"), slug => _repository.GetBySlug(slug) != null);
        }

        private static bool StartsWith(IFormFile file, byte[] signature)
        {
            using Stream stream = file.OpenReadStream();
            byte[] header = new byte[signature.Length];
            return stream.Read(header, 0, header.Length) == header.Length && header.SequenceEqual(signature);
        }
    }
}
