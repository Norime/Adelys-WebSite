namespace Adelys_WebSite.SAL.Interface
{
    public interface IServiceMojangAccess
    {
        Task<string> GetSkinData(string uuid);
    }
}
