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
    public interface IElasticRepository
    {
        Task<bool> CreateUser(string id,OnlineUserModel onlineUserModel, string _indexName);

        Task<bool> DeleteUser(string id,string _indexName);
        
        Task<OnlineUserModel> GetUser(string name,string _indexName);
    }
}