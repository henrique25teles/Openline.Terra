using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using System.Collections.Generic;

namespace Openline.Terra.Api.Repository.Interface
{
    public interface IRepositoryEmpresa<T> where T : ModelBaseEmpresa
    {
        IEnumerable<T> GetAll(int? empresaId);
        void Add(T entity);
        void Update(T entity);
        T Get(int? id, int? empresaId);
        void Delete(int? id, int? empresaId);
    }
}
