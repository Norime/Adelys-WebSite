using Adelys_WebSite.DAL;
using Adelys_WebSite.DAL.Repositories;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.DAL.UnitOfWork.Interfaces;

namespace Adelys_WebSite.DAL.UnitOfWork
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly MySqlDbContext context;
        private PlayerRepository _player;
        private UserPermissionRepository _userPermission;

        public UnitOfWork(MySqlDbContext context) => this.context = context ?? throw new ArgumentNullException("Context not supplied");

        public void Dispose() { 
            context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void SaveChange()
        {
            try
            {
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception("Erreur lors de la sauvegarde de la base de données", ex);
            }
        }

        public IPlayerRepository PlayerRepositoryDAO { 
            get {
                _player ??= new PlayerRepository(context);
                return _player; 
            } 
        }

        public IUserPermissionRepository UserPermissionRepositoryDAO
        {
            get
            {
                _userPermission ??= new UserPermissionRepository(context);
                return _userPermission;
            }
        }
    }
}
