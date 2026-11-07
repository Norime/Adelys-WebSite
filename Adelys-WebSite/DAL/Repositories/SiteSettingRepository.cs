using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.DAL.Repositories
{
    public class SiteSettingRepository : ISiteSettingRepository
    {
        private readonly SiteDbContext _context;

        public SiteSettingRepository(SiteDbContext context)
        {
            _context = context;
        }

        public string? Get(string key)
        {
            return _context.SiteSettings.Find(key)?.Value;
        }

        /// <summary>
        /// Enregistre le réglage, une valeur null le supprime pour revenir au comportement par défaut
        /// </summary>
        public void Set(string key, string? value)
        {
            SiteSetting? setting = _context.SiteSettings.Find(key);
            if (value == null)
            {
                if (setting != null)
                {
                    _context.SiteSettings.Remove(setting);
                }
            }
            else if (setting == null)
            {
                _context.SiteSettings.Add(new SiteSetting { Key = key, Value = value });
            }
            else
            {
                setting.Value = value;
            }
            _context.SaveChanges();
        }
    }
}
