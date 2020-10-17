using Npgsql;
using Openline.Terra.Api.Context;
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
        public void Add(T entity)
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

        //#region Insert Métodos

        //private string GetInsertInverse<TPai>(TPai entity)
        //{
        //    string sql = "";

        //    foreach (var prop in GetInverseProperties(typeof(TPai)))
        //    {
        //        var atributo = (InversePropertyAttribute)prop.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();
        //        var foreignKey = atributo.Property;

        //        var propertyType = prop.PropertyType;
        //        var tipoFilho = propertyType.GetGenericArguments().FirstOrDefault();
        //        var tabelaAtributo = tipoFilho.GetCustomAttributes(typeof(TableAttribute), false).First();

        //        var nomeTabelaFilho = typeof(TableAttribute).GetProperty("Name").GetValue(tabelaAtributo);
        //        var colunasTabelaFilho = GetColumns(tipoFilho);

        //        var filhos = typeof(TPai).GetProperty(prop.Name).GetValue(entity) as IEnumerable;

        //        var idLancamento = 0;

        //        foreach (var filho in filhos)
        //        {
        //            idLancamento++;

        //            var propriedadesMapeadasSemId = GetMappedProperties(filho.GetType())
        //                .Where(x => x.Name != "Id");

        //            var propriedadesParaInserir = propriedadesMapeadasSemId
        //                .Select(x => $"@{prop.Name}{idLancamento}_{x.Name}");

        //            var valoresInsert = String.Join(",", propriedadesParaInserir);

        //            sql = sql + $"INSERT INTO {nomeTabelaFilho} ({colunasTabelaFilho}) VALUES ({idLancamento},{valoresInsert}); ";

        //            sql += GetInsertInverse(filho);
        //        }
        //    }

        //    return sql;
        //}

        //private static void InsertDadosPrincipais(T entity, IEnumerable<PropertyInfo> propriedadesMapeadasSemId, NpgsqlCommand command)
        //{
        //    foreach (var prop in propriedadesMapeadasSemId)
        //    {
        //        var valor = prop.GetValue(entity);

        //        if (valor != null)
        //        {
        //            if (valor is bool)
        //                command.Parameters.AddWithValue(prop.Name, Convert.ToInt32(valor));
        //            else if (valor is DateTime)
        //                command.Parameters.AddWithValue(prop.Name, (DateTime)valor);
        //            else if (valor.GetType().IsEnum)
        //                command.Parameters.AddWithValue(prop.Name, (int)valor);
        //            else
        //                command.Parameters.AddWithValue(prop.Name, valor);
        //        }
        //        else
        //        {
        //            command.CommandText = command.CommandText.Replace($"@{prop.Name}", "NULL");
        //        }
        //    }
        //}

        //private void InsertDadosInversos(T entity, int proximoId, NpgsqlCommand command)
        //{
        //    foreach (var it in GetInverseProperties(entity.GetType()))
        //    {
        //        var atributo = (InversePropertyAttribute)it.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();

        //        var propertyType = it.PropertyType;
        //        var tipoFilho = propertyType.GetGenericArguments().FirstOrDefault();
        //        var tabelaAtributo = tipoFilho.GetCustomAttributes(typeof(TableAttribute), false).First();

        //        var propriedadesMapeadasSemIdFilho = GetMappedProperties(tipoFilho)
        //            .Where(prop => prop.Name != "Id");

        //        var propriedadesParaInserirFilho = propriedadesMapeadasSemIdFilho
        //            .Select(prop => $"@{prop.Name}");

        //        var filhos = entity.GetType().GetProperty(it.Name).GetValue(entity) as IEnumerable;

        //        var idLancamento = 0;

        //        foreach (var filho in filhos)
        //        {
        //            idLancamento++;

        //            foreach (var prop in propriedadesMapeadasSemIdFilho)
        //            {
        //                if (prop.GetCustomAttribute(typeof(ForeignKeyAttribute), false) != null)
        //                {
        //                    var coluna = GetColumn(prop);

        //                    //Valida se o campo atual, é a foreignkey da classe pai
        //                    if (coluna == atributo.Property)
        //                    {
        //                        command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", proximoId);
        //                        continue;
        //                    }
        //                }

        //                var valor = prop.GetValue(filho);

        //                if (valor != null)
        //                {
        //                    if (valor is bool)
        //                        command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", Convert.ToInt32(valor));
        //                    else if (valor is DateTime)
        //                        command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", (DateTime)valor);
        //                    else if (valor.GetType().IsEnum)
        //                        command.Parameters.AddWithValue(prop.Name, (int)valor);
        //                    else
        //                        command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", valor);
        //                }
        //                else
        //                {
        //                    command.CommandText = command.CommandText.Replace($"@{it.Name}{idLancamento}_{prop.Name}", "NULL");
        //                }

        //            }
        //        }
        //    }
        //}

        //#endregion

    }
}
