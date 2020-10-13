using Npgsql;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

namespace Openline.Terra.Api.Repository.Base
{
    public class RepositoryEmpresa<T> : Connections, IRepositoryEmpresa<T> where T : ModelBaseEmpresa
    {
        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int? id, int? empresaId)
        {
            throw new NotImplementedException();
        }

        public T Get(int? id, int? empresaId)
        {
            throw new NotImplementedException();
        }

        public Query<T> GetAll(int? empresaId)
        {
            try
            {
                var query = new Query<T>();

                query.Where("EmpresaId", TipoCriterio.Igual, empresaId.Value.ToString());

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
