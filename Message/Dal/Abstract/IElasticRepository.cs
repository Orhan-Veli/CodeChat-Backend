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
        Task<List<T>> GetReportedMessagesAsync(List<Guid> messageIds,string _indexName);
        Task<long> GetAllMessageCountAsync(string _indexName);
        Task<long> GetAllReportedMessageCountAsync(List<Guid> messageIds,string _indexName);
    }
}