namespace Adelys_WebSite.SAL.Interface
{
    public interface ICrafatarAccess
    {
        Task<string> GetPlayerSkin(string uuid);
    }
}
