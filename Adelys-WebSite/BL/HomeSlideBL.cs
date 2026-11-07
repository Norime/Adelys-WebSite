using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL
{
    public class HomeSlideBL : IHomeSlideBL
    {
        private readonly IHomeSlideRepository _repository;
        private readonly IContentImageStorage _imageStorage;
        private readonly IMarkdownBL _markdownBL;

        public HomeSlideBL(IHomeSlideRepository repository, IContentImageStorage imageStorage, IMarkdownBL markdownBL)
        {
            _repository = repository;
            _imageStorage = imageStorage;
            _markdownBL = markdownBL;
        }

        public List<HomeSlide> GetAllSlides()
        {
            return _repository.GetAll();
        }

        public HomeSlide? GetSlide(int id)
        {
            return _repository.GetById(id);
        }

        public async Task CreateSlide(HomeSlideFormViewModel form)
        {
            HomeSlide slide = new HomeSlide
            {
                SortOrder = _repository.GetAll().Select(s => s.SortOrder).DefaultIfEmpty(0).Max() + 1
            };
            Fill(slide, form);
            string fileName = await SaveImage(form.Image!);
            slide.ImageUrl = MarkdownBL.ImageRequestPath + fileName;

            try
            {
                _repository.Create(slide);
            }
            catch
            {
                _imageStorage.Delete(fileName);
                throw;
            }
        }

        public async Task UpdateSlide(HomeSlide slide, HomeSlideFormViewModel form)
        {
            string oldImageUrl = slide.ImageUrl;
            string? newFileName = form.Image != null ? await SaveImage(form.Image) : null;

            try
            {
                Fill(slide, form);
                if (newFileName != null)
                {
                    slide.ImageUrl = MarkdownBL.ImageRequestPath + newFileName;
                }
                _repository.Update();
            }
            catch
            {
                _imageStorage.Delete(newFileName);
                throw;
            }

            if (slide.ImageUrl != oldImageUrl)
            {
                DeleteUploadedImage(oldImageUrl);
            }
        }

        public void DeleteSlide(HomeSlide slide)
        {
            _repository.Delete(slide);
            DeleteUploadedImage(slide.ImageUrl);
        }

        /// <summary>
        /// Échange la place de l'image avec sa voisine : -1 vers le début, +1 vers la fin
        /// </summary>
        public void Move(HomeSlide slide, int direction)
        {
            List<HomeSlide> slides = _repository.GetAll();
            int index = slides.FindIndex(s => s.Id == slide.Id);
            int target = index + Math.Sign(direction);
            if (index < 0 || target < 0 || target >= slides.Count)
            {
                return;
            }

            // Les positions sont renumérotées pour que deux images n'aient jamais la même
            (slides[index], slides[target]) = (slides[target], slides[index]);
            for (int i = 0; i < slides.Count; i++)
            {
                slides[i].SortOrder = i + 1;
            }
            _repository.Update();
        }

        private static void Fill(HomeSlide slide, HomeSlideFormViewModel form)
        {
            slide.Title = form.Title.Trim();
            slide.Caption = string.IsNullOrWhiteSpace(form.Caption) ? null : form.Caption.Trim();
            slide.AltText = form.AltText.Trim();
        }

        private Task<string> SaveImage(IFormFile image)
        {
            return _imageStorage.Save(image, _markdownBL.GetImageExtension(image)!);
        }

        /// <summary>
        /// Supprime le fichier d'une image envoyée depuis le site, les images livrées avec le site restent en place
        /// </summary>
        private void DeleteUploadedImage(string imageUrl)
        {
            if (imageUrl.StartsWith(MarkdownBL.ImageRequestPath))
            {
                string fileName = imageUrl[MarkdownBL.ImageRequestPath.Length..];
                if (_markdownBL.IsImageFileName(fileName))
                {
                    _imageStorage.Delete(fileName);
                }
            }
        }
    }
}
