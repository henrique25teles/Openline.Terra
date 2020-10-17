using Npgsql;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Openline.Terra.Api.Context
{
    public class Query<T> : Connections
    {
        #region Construtor

        public Query()
        {
            foreach(var prop in typeof(T).GetProperties())
            {
                var isColuna = prop.CustomAttributes.Any(x => x.AttributeType == typeof(ColumnAttribute));

                if (isColuna)
                {
                    _colunas.Add(prop.Name, GetColumn(prop));
                }
            }
        }

        #endregion

        Dictionary<string, string> _colunas = new Dictionary<string, string>();
        List<Criterio> _criterios = new List<Criterio>();
        List<Order> _ordenacao = new List<Order>();
        int? _limit;
        int? _offset;

        #region Métodos Montagem Query

        public void Where(Expression<Func<T, object>> property, TipoCriterio tipoCriterio, string valor)
        {
            var expressao = property.Body.ToString();
            var indexInicial = expressao.IndexOf('.');
            var indexFinal = expressao.IndexOf(',');

            var campo = expressao.Substring(indexInicial + 1, indexFinal - indexInicial - 1);

            var coluna = _colunas[campo];

            _criterios.Add(new Criterio
            {
                Coluna = coluna,
                TipoCriterio = tipoCriterio,
                Valor = valor,
            });
        }

        public void Where(string campo, TipoCriterio tipoCriterio, string valor)
        {
            string coluna = "";

            bool isPossuiChave = _colunas.TryGetValue(campo, out coluna);
            
            if (isPossuiChave)
            {
                coluna = _colunas[campo];
            }
            else
            {
                coluna = campo;
            }

            _criterios.Add(new Criterio
            {
                Coluna = coluna,
                TipoCriterio = tipoCriterio,
                Valor = valor,
            });
        }

        public void OrderBy(Expression<Func<T, object>> property, OrderDirection orderDirection)
        {
            var expressao = property.Body.ToString();
            var indexInicial = expressao.IndexOf('.');
            var indexFinal = expressao.IndexOf(',');

            var campo = expressao.Substring(indexInicial + 1, indexFinal - indexInicial - 1);

            var coluna = _colunas[campo];

            _ordenacao.Add(new Order
            {
                Coluna = coluna,
                OrderDirection = orderDirection,
            });
        }

        public void OrderBy(string campo, OrderDirection orderDirection)
        {
            string coluna = "";

            bool isPossuiChave = _colunas.TryGetValue(campo, out coluna);

            if (isPossuiChave)
            {
                coluna = _colunas[campo];
            }
            else
            {
                coluna = campo;
            }

            _ordenacao.Add(new Order
            {
                Coluna = coluna,
                OrderDirection = orderDirection,
            });
        }

        public void Take(int limit)
        {
            _limit = limit;
        }

        public void Skip(int offset)
        {
            _offset = offset;
        }

        public string SQL()
        {
            var nomeTabela = GetTableName(typeof(T));
            var colunas = GetColumns(typeof(T));

            var sql = $"SELECT {colunas} FROM {nomeTabela} ";

            sql = sql + this.GeraWhere();
            sql = sql + this.GeraOrderBy();

            if (_limit.HasValue) sql = sql + $"LIMIT {_limit.Value} ";
            if (_offset.HasValue) sql = sql + $"OFFSET {_offset.Value} ";

            return sql;
        }

        private string GeraWhere()
        {
            var sql = "";

            if (this._criterios.Any())
            {
                sql = sql + "WHERE ";

                foreach (var criterio in this._criterios)
                {
                    if (this._criterios.IndexOf(criterio) > 0)
                    {
                        sql = sql + "AND ";
                    }

                    sql = sql + criterio.Coluna + " ";

                    switch (criterio.TipoCriterio)
                    {
                        case TipoCriterio.Igual:
                            sql = sql + "= ";
                            sql = sql + criterio.Valor + " ";
                            break;
                        case TipoCriterio.Diferente:
                            sql = sql + "<> ";
                            sql = sql + criterio.Valor + " ";
                            break;
                        case TipoCriterio.Maior:
                            sql = sql + "> ";
                            sql = sql + criterio.Valor + " ";
                            break;
                        case TipoCriterio.MaiorOuIgual:
                            sql = sql + ">= ";
                            sql = sql + criterio.Valor + " ";
                            break;
                        case TipoCriterio.Menor:
                            sql = sql + "< ";
                            sql = sql + criterio.Valor + " ";
                            break;
                        case TipoCriterio.MenorOuIgual:
                            sql = sql + "<= ";
                            sql = sql + criterio.Valor + " ";
                            break;
                        case TipoCriterio.Contem:
                            sql = sql + "= ";
                            sql = sql + $"({criterio.Valor}) ";
                            break;
                        case TipoCriterio.NaoContem:
                            sql = sql + "= ";
                            sql = sql + $"({criterio.Valor}) ";
                            break;
                    }
                }
            }

            return sql;
        }

        private string GeraOrderBy()
        {
            var sql = "";

            if (this._ordenacao.Any())
            {
                sql = sql + "ORDER BY ";

                foreach (var order in this._ordenacao)
                {
                    if (this._ordenacao.IndexOf(order) > 0)
                    {
                        sql = sql + ", ";
                    }

                    sql = sql + $"{order.Coluna} ";

                    switch (order.OrderDirection)
                    {
                        case OrderDirection.Asc:
                            sql = sql + "ASC ";
                            break;
                        case OrderDirection.Desc:
                            sql = sql + "DESC ";
                            break;
                    }
                }
            }

            return sql;
        }

        #endregion

        #region Roda Query

        public IEnumerable<T> Run()
        {
            List<T> lstEntity = new List<T>();

            using (var conexao = new NpgsqlConnection(str))
            {
                var sql = this.SQL();

                using (var command = new NpgsqlCommand(sql, conexao))
                {
                    command.CommandType = CommandType.Text;
                    OpenConnection(conexao);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T entity = (T)Activator.CreateInstance(typeof(T));
                            
                            BuscaColunasMapeadas(reader, entity);
                            
                            BuscaColunasInversas(entity);

                            lstEntity.Add(entity);
                        }
                    }
                }
            }

            return lstEntity;
        }
        
        private void BuscaColunasMapeadas(NpgsqlDataReader reader, T entity)
        {
            var mappedProperties = GetMappedProperties(typeof(T));

            foreach (var prop in mappedProperties)
            {
                var nomeColuna = GetColumn(prop);
                var valor = reader[nomeColuna];

                Type tipo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                if (valor is System.DBNull) continue;

                if (tipo.IsEnum)
                {
                    var valorInteiro = Convert.ToInt32(valor);

                    var valorConvertido = Enum.ToObject(tipo, valorInteiro);
                    prop.SetValue(entity, valorConvertido);
                }
                else
                {
                    var valorConvertido = Convert.ChangeType(valor, tipo);
                    prop.SetValue(entity, valorConvertido);
                }
            }
        }

        private void BuscaColunasInversas(T entity)
        {
            var inverseProperties = GetInverseProperties(typeof(T));

            foreach (var prop in inverseProperties)
            {
                var atributo = (InversePropertyAttribute)prop.GetCustomAttributes(typeof(InversePropertyAttribute), false).FirstOrDefault();
                var foreignKey = atributo.Property;

                var propertyType = prop.PropertyType;
                var tipoLista = propertyType.UnderlyingSystemType;
                var tipoItemLista = propertyType.GetGenericArguments().FirstOrDefault();
                var tipoItemBase = tipoItemLista.BaseType;

                var valorForeignKey = entity.GetType().GetProperty("Id").GetValue(entity).ToString();
                var valorEmpresaId = (int)entity.GetType().GetProperty("EmpresaId").GetValue(entity);
                var ValorUnidadeId = (int)entity.GetType().GetProperty("UnidadeId").GetValue(entity);

                var novaLista = Activator.CreateInstance(tipoLista);

                if (tipoItemBase == typeof(ModelBaseUnidade))
                {

                    var tipoRepositorio = typeof(RepositoryUnidade<>);
                    var repositorioConstruido = tipoRepositorio.MakeGenericType(tipoItemLista);

                    var repository = Activator.CreateInstance(repositorioConstruido);

                    var tipoQuery = typeof(Query<>);
                    var queryConstruida = tipoQuery.MakeGenericType(tipoItemLista);

                    var query = Activator.CreateInstance(queryConstruida);

                    query = repository.GetType()
                        .GetMethod("GetAll").Invoke(repository, new object[] { valorEmpresaId, ValorUnidadeId });

                    query.GetType().GetMethod("Where", 0, new Type[] { typeof(string), typeof(TipoCriterio), typeof(string) })
                        .Invoke(query, new object[] { foreignKey, TipoCriterio.Igual, valorForeignKey });

                    var resultado = query.GetType().GetMethod("Run").Invoke(query, null);

                    prop.SetValue(entity, resultado);
                }
                else if (tipoItemBase == typeof(ModelBaseEmpresa))
                {
                    var tipoRepositorio = typeof(RepositoryEmpresa<>);
                    var repositorioConstruido = tipoRepositorio.MakeGenericType(tipoItemLista);

                    var repository = Activator.CreateInstance(repositorioConstruido);

                    var tipoQuery = typeof(Query<>);
                    var queryConstruida = tipoQuery.MakeGenericType(tipoItemLista);

                    var query = Activator.CreateInstance(queryConstruida);

                    query = repository.GetType()
                        .GetMethod("GetAll").Invoke(repository, new object[] { valorEmpresaId });

                    query.GetType().GetMethod("Where", 0, new Type[] { typeof(string), typeof(TipoCriterio), typeof(string) })
                        .Invoke(query, new object[] { foreignKey, TipoCriterio.Igual, valorForeignKey });

                    var resultado = query.GetType().GetMethod("Run").Invoke(query, null);

                    prop.SetValue(entity, resultado);
                }
                else if (tipoItemBase == typeof(ModelBase))
                {
                    var tipoRepositorio = typeof(Repository<>);
                    var repositorioConstruido = tipoRepositorio.MakeGenericType(tipoItemLista);

                    var repository = Activator.CreateInstance(repositorioConstruido);

                    var tipoQuery = typeof(Query<>);
                    var queryConstruida = tipoQuery.MakeGenericType(tipoItemLista);

                    var query = Activator.CreateInstance(queryConstruida);

                    query = repository.GetType()
                        .GetMethod("GetAll").Invoke(repository, null);

                    query.GetType().GetMethod("Where", 0, new Type[] { typeof(string), typeof(TipoCriterio), typeof(string) })
                        .Invoke(query, new object[] { foreignKey, TipoCriterio.Igual, valorForeignKey });

                    var resultado = query.GetType().GetMethod("Run").Invoke(query, null);

                    prop.SetValue(entity, resultado);
                }

            }
        }

        #endregion
    }

    internal class Criterio
    {
        public string Coluna { get; set; }
        public TipoCriterio TipoCriterio { get; set; }
        public object Valor { get; set; }
    }

    internal class Order
    {
        public string Coluna { get; set; }
        public OrderDirection OrderDirection { get; set; }
    }

    public enum TipoCriterio
    {
        Igual,
        Diferente,
        Maior,
        MaiorOuIgual,
        Menor,
        MenorOuIgual,
        Contem,
        NaoContem,
    }

    public enum OrderDirection
    {
        Asc,
        Desc
    }
}
