using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Repository<TEntity>:IRepository<TEntity> where TEntity:class
    {
        DbSet<TEntity> Dbset;
        DbContext Dbcontext;
        public Repository(DbContext _dbcontext)
        {
            Dbcontext = _dbcontext;
            Dbset = Dbcontext.Set<TEntity>();
        }

        public TEntity GetByID(int id)
        {
            return Dbset.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Dbset;
        }

        public void Create(TEntity _TEntity)
        {
            Dbset.Add(_TEntity);
            Dbcontext.SaveChanges();
        }

        public void Delete(TEntity _TEntity)
        {
            Dbset.Remove(_TEntity);
            Dbcontext.SaveChanges();
        }

        public void Update(TEntity _TEntity)
        {
            Dbcontext.Entry(_TEntity).State = EntityState.Modified;
            Dbcontext.SaveChanges();
        }
    }
}