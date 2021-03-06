﻿using Npgsql;
using Openline.Terra.Api.Context.Schema;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Openline.Terra.Api.Context
{
    public class Connections
    {
        protected readonly string str = $"User ID=postgres;Password=5862709402;Server=localhost;Port=5432;Database=rmelao_utf8;Integrated Security=true;Pooling=true;";

        protected readonly NpgsqlConnectionStringBuilder sqlBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Database = "rmelao_utf8",
            Username = "postgres",
            Password = "5862709402",
            Pooling = true,
            IncludeErrorDetails = true,
        };

        #region Connection

        protected void OpenConnection(NpgsqlConnection conexao)
        {
            if (conexao.State == ConnectionState.Broken)
            {
                conexao.Close();
            }

            if (conexao.State != ConnectionState.Open)
            {
                conexao.Open();
            }
        }

        protected void CloseConnection(NpgsqlConnection conexao)
        {
            if (conexao.State == ConnectionState.Open || conexao.State == ConnectionState.Broken)
            {
                conexao.Close();
            }
        }

        #endregion

        #region Util

        protected string GetTableName(Type type)
        {
            var atributo = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();

            return atributo.Name;
        }

        protected string GetIdColumn(Type type)
        {
            var campo = type.GetProperties()
                .FirstOrDefault(property => property.Name == "Id");

            var coluna = (ColumnAttribute)campo.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();

            return coluna.Name;
        }

        protected List<string> GetColumnsList(Type type)
        {
            var colunas = GetMappedProperties(type)
                .Select(property => (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault())
                .Select(property => property.Name);

            return colunas.ToList();
        }

        protected string GetColumns(Type type)
        {
            var colunas = GetColumnsList(type);

            var joinColunas = String.Join(" , ", colunas);

            return joinColunas;
        }

        protected PropertyInfo[] GetMappedProperties(Type tabela)
        {
            var properties = tabela.GetProperties()
                .Where(property => property.GetCustomAttributes(typeof(ColumnAttribute), false).Any())
                .Where(property => !property.GetCustomAttributes(typeof(NotMappedAttribute), false).Any()).ToArray();

            return properties;
        }

        protected PropertyInfo[] GetInverseProperties(Type tabela)
        {
            var properties = tabela.GetProperties()
                .Where(property => property.GetCustomAttributes(typeof(InversePropertyAttribute), false).Any())
                .Where(property => !property.GetCustomAttributes(typeof(NotMappedAttribute), false).Any()).ToArray();

            return properties;
        }

        protected string GetColumn(PropertyInfo property)
        {
            var atributo = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();

            return atributo.Name;
        }

        #endregion

        #region Insert Métodos

        protected string GetInsertInverse<TPai>(TPai entity, int paiId)
        {
            string sql = "";

            foreach (var prop in GetInverseProperties(typeof(TPai)))
            {
                var atributo = (InversePropertyAttribute)prop.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();
                var foreignKey = atributo.Property;

                var propertyType = prop.PropertyType;
                var tipoFilho = propertyType.GetGenericArguments().FirstOrDefault();
                var tabelaAtributo = tipoFilho.GetCustomAttributes(typeof(TableAttribute), false).First();

                var nomeTabelaFilho = typeof(TableAttribute).GetProperty("Name").GetValue(tabelaAtributo);
                var colunasTabelaFilhoList = GetColumnsList(tipoFilho)
                    .Where(x => x != foreignKey);
                var colunasTabelaFilho = String.Join(',', colunasTabelaFilhoList);

                var filhos = typeof(TPai).GetProperty(prop.Name).GetValue(entity) as System.Collections.IEnumerable;

                var idLancamento = 0;

                foreach (var filho in filhos)
                {
                    idLancamento++;

                    var propriedadesMapeadasSemId = GetMappedProperties(filho.GetType())
                        .Where(x => ((ColumnAttribute)x.GetCustomAttributes(typeof(ColumnAttribute), false).First()).Name != foreignKey)
                        .Where(x => x.Name != "Id");

                    var propriedadesParaInserir = propriedadesMapeadasSemId
                        .Select(x => $"@{prop.Name}{idLancamento}_{x.Name}");

                    var valoresInsert = String.Join(",", propriedadesParaInserir);

                    sql += $"INSERT INTO {nomeTabelaFilho} ({colunasTabelaFilho},{foreignKey}) VALUES ({idLancamento},{valoresInsert},{paiId}); ";

                    sql += GetInsertInverse(filho, idLancamento);
                }
            }

            return sql;
        }
        
        protected static void InsertDadosPrincipais<T>(T entity, IEnumerable<PropertyInfo> propriedadesMapeadasSemId, NpgsqlCommand command)
        {
            foreach (var prop in propriedadesMapeadasSemId)
            {
                var valor = prop.GetValue(entity);

                if (valor != null)
                {
                    if (valor is bool)
                        command.Parameters.AddWithValue(prop.Name, Convert.ToInt32(valor));
                    else if (valor is DateTime)
                        command.Parameters.AddWithValue(prop.Name, (DateTime)valor);
                    else if (valor.GetType().IsEnum)
                        command.Parameters.AddWithValue(prop.Name, (int)valor);
                    else
                        command.Parameters.AddWithValue(prop.Name, valor);
                }
                else
                {
                    command.CommandText = command.CommandText.Replace($"@{prop.Name}", "NULL");
                }
            }
        }
        
        protected void InsertDadosInversos<T>(T entity, int proximoId, NpgsqlCommand command)
        {
            foreach (var it in GetInverseProperties(entity.GetType()))
            {
                var atributo = (InversePropertyAttribute)it.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();

                var propertyType = it.PropertyType;
                var tipoFilho = propertyType.GetGenericArguments().FirstOrDefault();
                var tabelaAtributo = tipoFilho.GetCustomAttributes(typeof(TableAttribute), false).First();

                var propriedadesMapeadasSemIdFilho = GetMappedProperties(tipoFilho)
                    .Where(prop => prop.Name != "Id");

                var filhos = entity.GetType().GetProperty(it.Name).GetValue(entity) as System.Collections.IEnumerable;

                var idLancamento = 0;

                foreach (var filho in filhos)
                {
                    idLancamento++;

                    foreach (var prop in propriedadesMapeadasSemIdFilho)
                    {
                        if (prop.GetCustomAttribute(typeof(ForeignKeyAttribute), false) != null)
                        {
                            var coluna = GetColumn(prop);

                            //Valida se o campo atual, é a foreignkey da classe pai
                            if (coluna == atributo.Property)
                            {
                                //command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", proximoId);
                                continue;
                            }
                        }

                        var valor = prop.GetValue(filho);

                        if (valor != null)
                        {
                            if (valor is bool)
                                command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", Convert.ToInt32(valor));
                            else if (valor is DateTime)
                                command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", (DateTime)valor);
                            else if (valor.GetType().IsEnum)
                                command.Parameters.AddWithValue(prop.Name, (int)valor);
                            else
                                command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", valor);
                        }
                        else
                        {
                            command.CommandText = command.CommandText.Replace($"@{it.Name}{idLancamento}_{prop.Name}", "NULL");
                        }

                    }
                }
            }
        }

        #endregion

        #region Update Métodos

        protected string GetUpdateInverse<TPai>(TPai entity, int paiId)
        {
            string sql = "";

            foreach (var prop in GetInverseProperties(typeof(TPai)))
            {
                var atributo = (InversePropertyAttribute)prop.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();
                var foreignKey = atributo.Property;

                var propertyType = prop.PropertyType;
                var tipoFilho = propertyType.GetGenericArguments().FirstOrDefault();
                var tabelaAtributo = tipoFilho.GetCustomAttributes(typeof(TableAttribute), false).First();

                var nomeTabelaFilho = typeof(TableAttribute).GetProperty("Name").GetValue(tabelaAtributo);
                var colunasTabelaFilho = GetColumns(tipoFilho);

                var filhos = typeof(TPai).GetProperty(prop.Name).GetValue(entity) as System.Collections.IEnumerable;


                foreach (var filho in filhos)
                {
                    var idLancamento = filho.GetType().GetProperty("Id").GetValue(filho);
                    var empresaId = filho.GetType().GetProperty("EmpresaId")?.GetValue(filho);
                    var unidadeId = filho.GetType().GetProperty("UnidadeId")?.GetValue(filho);

                    var propriedadesMapeadasSemId = GetMappedProperties(filho.GetType())
                        .Where(x => !x.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any())
                        .Where(x => x.Name != "EmpresaId")
                        .Where(x => x.Name != "UnidadeId")
                        .Where(x => x.Name != "Id");

                    var propriedadesParaAtualizar = propriedadesMapeadasSemId
                        .Select(x => $"{GetColumn(x)} = @{prop.Name}{idLancamento}_{x.Name}");

                    var valoresUpdate = String.Join(",", propriedadesParaAtualizar);

                    sql += $"UPDATE {nomeTabelaFilho} SET {valoresUpdate} WHERE ";

                    sql += $"{foreignKey} = {paiId} ";
                    sql += $"and {GetColumn(filho.GetType().GetProperty("Id"))} = {idLancamento} ";

                    if (filho.GetType().BaseType == typeof(ModelBaseEmpresa))
                    {
                        sql += $"and {GetColumn(filho.GetType().GetProperty("EmpresaId"))} = {empresaId}; ";
                    }
                    else if (filho.GetType().BaseType == typeof(ModelBaseUnidade))
                    {
                        sql += $"and {GetColumn(filho.GetType().GetProperty("EmpresaId"))} = {empresaId} ";
                        sql += $"and {GetColumn(filho.GetType().GetProperty("UnidadeId"))} = {unidadeId}; ";
                    }
                    else if (filho.GetType().BaseType == typeof(ModelBaseUnidade))
                    {
                        sql += "; ";
                    }

                    sql += GetUpdateInverse(filho, (int)idLancamento);
                }
            }

            return sql;
        }

        protected void UpdateDadosInversos<T>(T entity, NpgsqlCommand command)
        {
            foreach (var it in GetInverseProperties(entity.GetType()))
            {
                var atributo = (InversePropertyAttribute)it.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();

                var propertyType = it.PropertyType;
                var tipoFilho = propertyType.GetGenericArguments().FirstOrDefault();
                var tabelaAtributo = tipoFilho.GetCustomAttributes(typeof(TableAttribute), false).First();

                var propriedadesMapeadasSemIdFilho = GetMappedProperties(tipoFilho)
                    .Where(prop => !prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any())
                    .Where(prop => prop.Name != "EmpresaId")
                    .Where(prop => prop.Name != "UnidadeId")
                    .Where(prop => prop.Name != "Id");

                var filhos = entity.GetType().GetProperty(it.Name).GetValue(entity) as System.Collections.IEnumerable;

                var idLancamento = 0;

                foreach (var filho in filhos)
                {
                    idLancamento++;

                    foreach (var prop in propriedadesMapeadasSemIdFilho)
                    {
                        var valor = prop.GetValue(filho);

                        if (valor != null)
                        {
                            if (valor is bool)
                                command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", Convert.ToInt32(valor));
                            else if (valor is DateTime)
                                command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", (DateTime)valor);
                            else if (valor.GetType().IsEnum)
                                command.Parameters.AddWithValue(prop.Name, (int)valor);
                            else
                                command.Parameters.AddWithValue($"{it.Name}{idLancamento}_{prop.Name}", valor);
                        }
                        else
                        {
                            command.CommandText = command.CommandText.Replace($"@{it.Name}{idLancamento}_{prop.Name}", "NULL");
                        }

                    }
                }
            }
        }

        #endregion

        #region Delete Métodos

        protected string GetDeleteInverse<TPai>(TPai entity, int? paiId)
        {
            string sql = "";

            foreach (var prop in GetInverseProperties(typeof(TPai)))
            {
                var atributo = (InversePropertyAttribute)prop.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();
                var foreignKey = atributo.Property;

                var propertyType = prop.PropertyType;
                var tipoFilho = propertyType.GetGenericArguments().FirstOrDefault();
                var tabelaAtributo = tipoFilho.GetCustomAttributes(typeof(TableAttribute), false).First();

                var nomeTabelaFilho = typeof(TableAttribute).GetProperty("Name").GetValue(tabelaAtributo);
                var colunasTabelaFilho = GetColumns(tipoFilho);

                var filhos = typeof(TPai).GetProperty(prop.Name).GetValue(entity) as System.Collections.IEnumerable;


                foreach (var filho in filhos)
                {
                    var idLancamento = filho.GetType().GetProperty("Id").GetValue(filho);
                    var empresaId = filho.GetType().GetProperty("EmpresaId")?.GetValue(filho);
                    var unidadeId = filho.GetType().GetProperty("UnidadeId")?.GetValue(filho);

                    sql += $"DELETE FROM {nomeTabelaFilho} WHERE ";

                    sql += $"{foreignKey} = {paiId} ";
                    sql += $"and {GetColumn(filho.GetType().GetProperty("Id"))} = {idLancamento} ";

                    if (filho.GetType().BaseType == typeof(ModelBaseEmpresa))
                    {
                        sql += $"and {GetColumn(filho.GetType().GetProperty("EmpresaId"))} = {empresaId}; ";
                    }
                    else if (filho.GetType().BaseType == typeof(ModelBaseUnidade))
                    {
                        sql += $"and {GetColumn(filho.GetType().GetProperty("EmpresaId"))} = {empresaId} ";
                        sql += $"and {GetColumn(filho.GetType().GetProperty("UnidadeId"))} = {unidadeId}; ";
                    }
                    else if (filho.GetType().BaseType == typeof(ModelBaseUnidade))
                    {
                        sql += "; ";
                    }

                    sql += GetDeleteInverse(filho, (int)idLancamento);
                }
            }

            return sql;
        }

        #endregion
    }
}
