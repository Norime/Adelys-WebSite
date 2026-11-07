namespace Adelys_WebSite.DAL.Repositories.Interfaces
{
    public interface ISiteSettingRepository
    {
        string? Get(string key);
        void Set(string key, string? value);
    }
}
