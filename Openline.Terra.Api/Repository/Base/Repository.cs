using Npgsql;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
                var colunasLista = GetColumnsList(typeof(T));
                var colunaId = GetIdColumn(typeof(T));

                using (var conexao = new NpgsqlConnection(str))
                {
                    int proximoId = GetProximoId(nomeTabela, colunaId, conexao);

                    var propriedadesMapeadasSemId = GetMappedProperties(typeof(T))
                        .Where(prop => prop.Name != "Id");

                    var propriedadesParaInserir = propriedadesMapeadasSemId
                        .Select(prop => $"@{prop.Name}");

                    var valoresInsert = String.Join(",", propriedadesParaInserir);

                    var sql = $"INSERT INTO {nomeTabela} ({colunas}) VALUES ({proximoId} , {valoresInsert}); ";

                    sql += GetInsertInverse(entity);

                    using (var command = new NpgsqlCommand(sql, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        OpenConnection(conexao);

                        InsertDadosPrincipais(entity, propriedadesMapeadasSemId, command);

                        InsertDadosInversos(entity, proximoId, command);

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
            if (!id.HasValue) throw new Exception("Id não informado");

            var query = new Query<T>();

            query.Where(x => x.Id, TipoCriterio.Igual, id.Value.ToString());

            var retorno = query.Run();

            return retorno.FirstOrDefault();
        }

        public virtual Query<T> GetAll()
        {
            try
            {
                return new Query<T>();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro", ex);
            }
        }

        public virtual void Update(T entity)
        {
            throw new NotImplementedException();
        }

        private int GetProximoId(string nomeTabela, string colunaId, NpgsqlConnection conexao)
        {
            int proximoId = 0;

            var sqlProximoId = $"Select (Max({colunaId}) + 1) codigo from {nomeTabela}";

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

            return proximoId;
        }

    }
}
