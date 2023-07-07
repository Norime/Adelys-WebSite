using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interface;
using Adelys_WebSite.DAL.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Adelys_WebSite.BL
{
    public abstract class BaseBL<TObject> : IBaseBL<TObject> where TObject : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected abstract IRepository<TObject> Repository { get; }

        protected BaseBL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TObject GetById(int id)
        {
            try
            {
                return Repository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'appel à GetById({id})", ex);
            }
        }

        public IEnumerable<TObject> GetAll()
        {
            try
            {
                return Repository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'appel à GetAll()", ex);
            }
        }

        public void Create(TObject TObject)
        {
            try
            {
                Repository.Create(TObject);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'appel à Update({TObject})", ex);
            }
        }

        public void Update(TObject TObject)
        {
            try
            {
                Repository.Update(TObject);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'appel à Update({TObject})", ex);
            }
        }

        public void Delete(TObject TObject)
        {
            try
            {
                Repository.Delete(TObject);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'appel à Update({TObject})", ex);
            }
        }
    }
}
