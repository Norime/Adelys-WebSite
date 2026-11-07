using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using Adelys_WebSite.ViewModels;

namespace Adelys_WebSite.BL
{
    /// <summary>
    /// Image de fond du haut de la page d'accueil
    /// </summary>
    public class HomeHeroBL : IHomeHeroBL
    {
        public static readonly HomeHero DefaultHero = new HomeHero
        {
            ImageUrl = "/content/Images/Home/hero-pristo.jpg",
            AltText = "La porte fortifiée de Pristo et ses quatre tours",
            IsDefault = true
        };

        private readonly ISiteSettingRepository _settings;
        private readonly IContentImageStorage _imageStorage;
        private readonly IMarkdownBL _markdownBL;

        public HomeHeroBL(ISiteSettingRepository settings, IContentImageStorage imageStorage, IMarkdownBL markdownBL)
        {
            _settings = settings;
            _imageStorage = imageStorage;
            _markdownBL = markdownBL;
        }

        public HomeHero GetHero()
        {
            string? imageUrl = _settings.Get(SiteSettingKeys.HeroImageUrl);
            if (imageUrl == null)
            {
                return DefaultHero;
            }
            return new HomeHero
            {
                ImageUrl = imageUrl,
                AltText = _settings.Get(SiteSettingKeys.HeroImageAlt) ?? string.Empty
            };
        }

        public async Task UpdateHero(HomeHeroFormViewModel form)
        {
            string? oldImageUrl = _settings.Get(SiteSettingKeys.HeroImageUrl);
            if (form.Image != null)
            {
                string fileName = await _imageStorage.Save(form.Image, _markdownBL.GetImageExtension(form.Image)!);
                try
                {
                    _settings.Set(SiteSettingKeys.HeroImageUrl, MarkdownBL.ImageRequestPath + fileName);
                }
                catch
                {
                    _imageStorage.Delete(fileName);
                    throw;
                }
                DeleteUploadedImage(oldImageUrl);
            }
            else if (oldImageUrl == null)
            {
                // Seule la description change : l'image par défaut devient un réglage pour la garder
                _settings.Set(SiteSettingKeys.HeroImageUrl, DefaultHero.ImageUrl);
            }
            _settings.Set(SiteSettingKeys.HeroImageAlt, form.AltText.Trim());
        }

        public void ResetHero()
        {
            string? oldImageUrl = _settings.Get(SiteSettingKeys.HeroImageUrl);
            _settings.Set(SiteSettingKeys.HeroImageUrl, null);
            _settings.Set(SiteSettingKeys.HeroImageAlt, null);
            DeleteUploadedImage(oldImageUrl);
        }

        /// <summary>
        /// Supprime le fichier d'une image envoyée depuis le site, l'image livrée avec le site reste en place
        /// </summary>
        private void DeleteUploadedImage(string? imageUrl)
        {
            if (imageUrl != null && imageUrl.StartsWith(MarkdownBL.ImageRequestPath))
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
