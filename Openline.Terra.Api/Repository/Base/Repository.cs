using Npgsql;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;

namespace Openline.Terra.Api.Repository.Base
{
    public class Repository<T> : Connections, IRepository<T> where T : ModelBase
    {
        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public T Get(int? id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            try
            {
                var nomeTabela = GetTableName(typeof(T));
                var colunas = GetColumns(typeof(T));

                List<T> lstEntity = new List<T>();

                using (var conexao = new NpgsqlConnection(str))
                {
                    var sql = $"SELECT {colunas} from {nomeTabela}";

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
            catch(Exception ex)
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
