using Adelys_WebSite.DAL;
using Adelys_WebSite.DAL.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Adelys_WebSite.DAL.Repositories
{
    public class Repository<TObject> : IRepository<TObject> where TObject : class
    {
        protected readonly MySqlDbContext _context = null;

        public Repository(MySqlDbContext context)
        {
            _context = context;
        }
        protected DbSet<TObject> DbSet
        {
            get
            {
                return _context.Set<TObject>();
            }
        }
        //private bool IsInModelNamespace(TObject entity)
        //{
        //    string modelNamespace = typeof(TObject).Namespace;
        //    string modelFolderName = "Model";
        //    return modelNamespace != null && modelNamespace.Contains(modelFolderName);
        //}

        public TObject GetById(int id)
        {
            return DbSet.Find(id);
        }
        public List<TObject> GetAll()
        {
            return DbSet.AsQueryable().ToList();
        }
        public virtual void Create(TObject entity)
        {
            DbSet.Add(entity);//Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TObject> newEntry = DbSet.Add(entity);
            _context.SaveChanges();
            //return newEntry.Entity;
        }
        public virtual void Update(TObject entity)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TObject> entry = _context.Entry(entity);
            DbSet.Attach(entity);
            entry.State = EntityState.Modified;
            _context.SaveChanges();
        }
        public virtual void Delete(TObject entity)
        {
            DbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}
