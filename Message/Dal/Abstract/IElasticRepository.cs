using Message.Dal.Model;
using Message.Dal.Abstract;
using Nest;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
namespace Message.Dal.Abstract
{
    public interface IElasticRepository<T>
    {
        Task<bool> CreateAsync(Guid id,T model,string _indexName);
        Task<T> UpdateAsync(Guid id,T model,string _indexName);
        Task<T> GetAsync(Guid id,string _indexName);
        Task<List<T>> GetAllAsync(string _indexName);
        Task<bool> DeleteAsync(Guid id,string _indexName);
        Task<bool> CreateUserAsync(string id,OnlineUserModel onlineUserModel, string _indexName);
        Task<bool> DeleteUserAsync(string id,string _indexName);
        Task<T> GetUserAsync(string name,string _indexName);
    }
}