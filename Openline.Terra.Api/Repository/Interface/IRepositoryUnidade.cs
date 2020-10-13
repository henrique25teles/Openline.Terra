using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;

namespace Openline.Terra.Api.Repository.Interface
{
    public interface IRepositoryUnidade<T> where T : ModelBaseUnidade
    {
        Query<T> GetAll(int? empresaId, int? unidadeId);
        void Add(T entity);
        void Update(T entity);
        T Get(int? id, int? empresaId, int? unidadeId);
        void Delete(int? id, int? empresaId, int? unidadeId);
    }
}
