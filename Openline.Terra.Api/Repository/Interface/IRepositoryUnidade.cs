using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using System.Collections.Generic;

namespace Openline.Terra.Api.Repository.Interface
{
    public interface IRepositoryUnidade<T> where T : ModelBaseUnidade
    {
        IEnumerable<T> GetAll(int? empresaId, int? unidadeId);
        void Add(T entity);
        void Update(T entity);
        T Get(int? id, int? empresaId, int? unidadeId);
        void Delete(int? id, int? empresaId, int? unidadeId);
    }
}
