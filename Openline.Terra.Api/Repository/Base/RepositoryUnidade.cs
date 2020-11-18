using Npgsql;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Context.Schema;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Openline.Terra.Api.Repository.Base
{
    public class RepositoryUnidade<T> : Connections, IRepositoryUnidade<T> where T : ModelBaseUnidade
    {
        public virtual T Add(T entity)
        {
            try
            {
                var nomeTabela = GetTableName(typeof(T));
                var colunas = GetColumns(typeof(T));
                var colunasLista = GetColumnsList(typeof(T));
                var colunaId = GetIdColumn(typeof(T));

                var empresaId = entity.EmpresaId;
                var empresaColuna = ((ColumnAttribute)typeof(T).GetProperty("EmpresaId").GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name;
                var unidadeId = entity.UnidadeId;
                var unidadeColuna = ((ColumnAttribute)typeof(T).GetProperty("UnidadeId").GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name;

                using (var conexao = new NpgsqlConnection(str))
                {
                    int proximoId = GetProximoId(nomeTabela, colunaId, empresaId, empresaColuna, unidadeId, unidadeColuna, conexao);

                    var propriedadesMapeadasSemId = GetMappedProperties(typeof(T))
                        .Where(prop => prop.Name != "Id");

                    var propriedadesParaInserir = propriedadesMapeadasSemId
                        .Select(prop => $"@{prop.Name}");

                    var valoresInsert = String.Join(",", propriedadesParaInserir);

                    var sql = $"INSERT INTO {nomeTabela} ({colunas}) VALUES ({proximoId},{valoresInsert}); ";

                    sql += GetInsertInverse(entity, proximoId);

                    using (var command = new NpgsqlCommand(sql, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        OpenConnection(conexao);

                        InsertDadosPrincipais(entity, propriedadesMapeadasSemId, command);
                        
                        InsertDadosInversos(entity, proximoId, command);

                        command.ExecuteNonQuery();

                        CloseConnection(conexao);
                    }

                    return this.Get(proximoId, empresaId, unidadeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro", ex);
            }
        }

        public virtual void Delete(int? id, int? empresaId, int? unidadeId)
        {
            try
            {
                var nomeTabela = GetTableName(typeof(T));
                var colunaId = GetIdColumn(typeof(T));

                var empresaColuna = ((ColumnAttribute)typeof(T).GetProperty("EmpresaId").GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name;
                var unidadeColuna = ((ColumnAttribute)typeof(T).GetProperty("UnidadeId").GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name;

                var entity = Get(id, empresaId, unidadeId);

                using (var conexao = new NpgsqlConnection(str))
                {
                    var sql = "";

                    sql += GetDeleteInverse(entity, id);
                    
                    sql += $"DELETE FROM {nomeTabela} " +
                        $"WHERE {empresaColuna} = {empresaId} " +
                        $"AND {unidadeColuna} = {unidadeId} " +
                        $"AND {colunaId} = {id}; ";

                    using (var command = new NpgsqlCommand(sql, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        OpenConnection(conexao);

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

        public virtual T Get(int? id, int? empresaId, int? unidadeId)
        {
            try
            {
                if (!id.HasValue) throw new Exception("Id não informado");
                if (!empresaId.HasValue) throw new Exception("Id Empresa não informado");
                if (!unidadeId.HasValue) throw new Exception("Id Unidade não informado");

                var query = new Query<T>();

                query.Where("EmpresaId", TipoCriterio.Igual, empresaId.Value.ToString());
                query.Where("UnidadeId", TipoCriterio.Igual, unidadeId.Value.ToString());
                query.Where(x => x.Id, TipoCriterio.Igual, id.Value.ToString());

                var retorno = query.Run();

                return retorno.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro", ex);
            }
        }

        public virtual Query<T> GetAll(int? empresaId, int? unidadeId)
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

        public virtual T Update(T entity)
        {
            try
            {
                var nomeTabela = GetTableName(typeof(T));
                var colunas = GetColumns(typeof(T));
                var colunasLista = GetColumnsList(typeof(T));
                var colunaId = GetIdColumn(typeof(T));

                var empresaId = entity.EmpresaId;
                var empresaColuna = ((ColumnAttribute)typeof(T).GetProperty("EmpresaId").GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name;
                var unidadeId = entity.UnidadeId;
                var unidadeColuna = ((ColumnAttribute)typeof(T).GetProperty("UnidadeId").GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name;

                using (var conexao = new NpgsqlConnection(str))
                {
                    var propriedadesMapeadasSemId = GetMappedProperties(typeof(T))
                        .Where(prop => !prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any())
                        .Where(prop => prop.Name != "EmpresaId")
                        .Where(prop => prop.Name != "UnidadeId")
                        .Where(prop => prop.Name != "Id");

                    var propriedadesParaAtualizar = propriedadesMapeadasSemId
                        .Select(prop => $"{GetColumn(prop)} = @{prop.Name}");

                    var valoresUpdate = String.Join(",", propriedadesParaAtualizar);

                    var sql = $"UPDATE {nomeTabela} SET {valoresUpdate} " +
                        $"WHERE {empresaColuna} = {empresaId} " +
                        $"AND {unidadeColuna} = {unidadeId} " +
                        $"AND {colunaId} = {entity.Id}; ";

                    sql += GetUpdateInverse(entity, entity.Id);

                    using (var command = new NpgsqlCommand(sql, conexao))
                    {
                        command.CommandType = CommandType.Text;
                        OpenConnection(conexao);

                        InsertDadosPrincipais(entity, propriedadesMapeadasSemId, command);

                        UpdateDadosInversos(entity, command);

                        command.ExecuteNonQuery();

                        CloseConnection(conexao);
                    }

                    return this.Get(entity.Id, empresaId, unidadeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro", ex);
            }
        }

        private int GetProximoId(string nomeTabela, string colunaId, int empresaId, string empresaColuna, int unidadeId, string unidadeColuna, NpgsqlConnection conexao)
        {
            int proximoId = 0;

            var sqlProximoId = $"Select (Max({colunaId}) + 1) codigo from {nomeTabela} " +
                $"where {empresaColuna} = {empresaId} and {unidadeColuna} = {unidadeId} ";

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
