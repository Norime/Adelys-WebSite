namespace Adelys_WebSite.DAL.Repositories.Interface
{
    public interface IRepository<TObject>
    {
        TObject GetById(int id);
        List<TObject> GetAll();
        void Create(TObject entity);
        void Update(TObject entity);
        void Delete(TObject entity);
    }
}
