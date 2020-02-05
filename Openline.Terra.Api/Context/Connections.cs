﻿using Npgsql;
using Openline.Terra.Api.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Openline.Terra.Api.Context
{
    public class Connections
    {
        protected readonly string str = $"User ID=postgres;Password=5862709402;Server=localhost;Port=5432;Database=simonio;Integrated Security=true;Pooling=true;";

        protected readonly NpgsqlConnectionStringBuilder sqlBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Database = "teste",
            Username = "postgres",
            Password = "5862709402",
            Pooling = true,
        };

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

        protected string GetTableName(Type type)
        {
            var atributo = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();

            return atributo.Name;
        }

        protected string GetColumns(Type type)
        {
            var colunas = type.GetProperties()
                .Select(property => (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault())
                .Select(property => property.Name);

            var joinColunas = String.Join(',', colunas);

            return joinColunas;
        }

        protected string GetColumn(PropertyInfo property)
        {
            var atributo = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();

            return atributo.Name;
        }
    }
}
