using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Message.Dal.Abstract
{
    public interface IEntityRepository<T> : IElasticRepository
    {
        Task<bool> Create(Guid id,T model,string _indexName);
        Task<T> Update(Guid id,T model,string _indexName);
        Task<T> Get(Guid id,string _indexName);
        Task<List<T>> GetAll(string _indexName);
        Task<bool> Delete(Guid id,string _indexName);
    }
}