using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using System.Collections.Generic;

namespace Openline.Terra.Api.Repository.Interface
{
    public interface IRepository<T> where T : ModelBase
    {
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        T Get(int? id);
        void Delete(int? id);
    }
}
