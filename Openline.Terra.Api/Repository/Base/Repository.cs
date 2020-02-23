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
        public virtual void Add(T entity)
        {
            try
            {
                var nomeTabela = GetTableName(typeof(T));
                var colunas = GetColumns(typeof(T));
                var colunasInsert = GetColumnsInsert(typeof(T));
                var colunasLista = GetColumnsList(typeof(T));
                var colunaId = GetIdColumn(typeof(T));

                using (var conexao = new NpgsqlConnection(str))
                {
                    var sqlProximoId = $"Select (Max({colunaId}) + 1) codigo from {nomeTabela}";

                    int proximoId = 0;

                    using (var command = new NpgsqlCommand(sqlProximoId, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        OpenConnection(conexao);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                proximoId = Convert.ToInt32(reader["codigo"]);
                            }
                        }

                        CloseConnection(conexao);
                    }

                    var sql = $"Insert into {nomeTabela} ({colunas}) values ({proximoId},{colunasInsert}) ";

                    using (var command = new NpgsqlCommand(sql, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        OpenConnection(conexao);

                        for (var index = 1; index < colunasLista.Count; index++)
                        {
                            var valor = entity.GetType().GetProperties()[index].GetValue(entity);

                            if (valor.GetType() == typeof(bool))
                                command.Parameters.AddWithValue(colunasLista[index], Convert.ToInt32(valor));
                            else
                                command.Parameters.AddWithValue(colunasLista[index], valor);
                        }

                        command.ExecuteNonQuery();

                        CloseConnection(conexao);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro", ex);
            }
        }

        public virtual void Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public virtual T Get(int? id)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> GetAll()
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

                                foreach (var prop in GetMappedProperties(typeof(T)))
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

        public virtual void Update(T entity)
        {
            throw new NotImplementedException();
        }

    }
}
