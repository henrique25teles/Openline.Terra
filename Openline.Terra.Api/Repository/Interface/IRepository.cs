using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;

namespace Openline.Terra.Api.Repository.Interface
{
    public interface IRepository<T> where T : ModelBase
    {
        Query<T> GetAll();
        T Add(T entity);
        T Update(T entity);
        T Get(int? id);
        void Delete(int? id);
    }
}
