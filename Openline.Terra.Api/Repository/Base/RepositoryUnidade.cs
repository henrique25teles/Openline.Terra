using Npgsql;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        public IEnumerable<T> GetAll(int? empresaId, int? unidadeId)
        {
            try
            {
                var nomeTabela = GetTableName(typeof(T));
                var colunas = GetColumns(typeof(T));
                var colunaEmpresa = GetColumn(typeof(T).GetProperty("EmpresaId"));
                var colunaUnidade = GetColumn(typeof(T).GetProperty("UnidadeId"));

                List<T> lstEntity = new List<T>();

                using (var conexao = new NpgsqlConnection(str))
                {
                    var sql = $"SELECT {colunas} from {nomeTabela} " + 
                        $"where {colunaEmpresa} = {empresaId} " +
                        $"and {colunaUnidade} = {unidadeId}";

                    using (var command = new NpgsqlCommand(sql, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        OpenConnection(conexao);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                T entity = (T)Activator.CreateInstance(typeof(T));

                                foreach (var prop in typeof(T).GetProperties())
                                {
                                    var nomeColuna = GetColumn(prop);
                                    var valor = reader[nomeColuna];

                                    var valorConvertido = Convert.ChangeType(valor, prop.PropertyType);

                                    prop.SetValue(entity, valorConvertido);
                                }

                                lstEntity.Add(entity);
                            }
                        }
                    }
                }

                return lstEntity;
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
