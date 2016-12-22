using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BookStore_Web_.Models
{
    public class Repository<TEntity>:Irepository<TEntity>where TEntity:class
    {
        DbSet<TEntity> dbset;
        DbContext dbcontext;

        public Repository(DbContext _dbcontext)
        {
            dbcontext = _dbcontext;
            dbset = dbcontext.Set<TEntity>();
        }

        public TEntity GetByID(int id)
        {
            return dbset.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbset;
        }

        public void Create(TEntity _TEntity)
        {
            dbset.Add(_TEntity);
            dbcontext.SaveChanges();
        }

        public void Delete(TEntity _TEntity)
        {
            dbset.Remove(_TEntity);
            dbcontext.SaveChanges();
        }

        public void Update(TEntity _TEntity)
        {
            dbcontext.Entry(_TEntity).State = EntityState.Modified;
            dbcontext.SaveChanges();
        }
    }
}