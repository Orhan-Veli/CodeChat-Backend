using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Message.Dal.Abstract
{
    public interface IEntityRepository<T>
    {
        Task<bool> Create(T model);
        Task<T> Update(T model);
        Task<T> Get(Guid id);
        Task<List<T>> GetAll();
        Task<bool> Delete(Guid id);
    }
}