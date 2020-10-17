using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;

namespace Openline.Terra.Api.Repository.Interface
{
    public interface IRepositoryEmpresa<T> where T : ModelBaseEmpresa
    {
        Query<T> GetAll(int? empresaId);
        T Add(T entity);
        T Update(T entity);
        T Get(int? id, int? empresaId);
        void Delete(int? id, int? empresaId);
    }
}
