using Npgsql;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Openline.Terra.Api.Repository.Base
{
    public class RepositoryUnidade<T> : Connections, IRepositoryUnidade<T> where T : ModelBaseUnidade
    {
        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int? id, int? empresaId, int? unidadeId)
        {
            throw new NotImplementedException();
        }

        public T Get(int? id, int? empresaId, int? unidadeId)
        {
            throw new NotImplementedException();
        }

        public Query<T> GetAll(int? empresaId, int? unidadeId)
        {
            try
            {
                var query = new Query<T>();

                query.Where("EmpresaId", TipoCriterio.Igual, empresaId.Value.ToString());
                query.Where("UnidadeId", TipoCriterio.Igual, unidadeId.Value.ToString());

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro", ex);
            }
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

    }
}
