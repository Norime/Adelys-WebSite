using Adelys_WebSite.DAL.Repositories.Interfaces;

namespace Adelys_WebSite.DAL.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        IPlayerRepository PlayerRepositoryDAO { get; }
        IUserPermissionRepository UserPermissionRepositoryDAO { get; }
    }
}
