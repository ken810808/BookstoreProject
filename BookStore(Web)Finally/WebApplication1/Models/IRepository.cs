using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
   public interface IRepository<TEntity>
    {
       TEntity GetByID(int id);
       IEnumerable<TEntity> GetAll();
       void Create(TEntity _TEntity);
       void Delete(TEntity _TEntity);
       void Update(TEntity _TEntity);
    }
}
