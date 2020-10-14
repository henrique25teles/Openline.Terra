using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Openline.Terra.Api.Context
{
    public class Query<T> : Connections
    {
        public Query()
        {
            foreach(var prop in typeof(T).GetProperties())
            {
                _colunas.Add(prop.Name, GetColumn(prop));
            }
        }

        Dictionary<string, string> _colunas = new Dictionary<string, string>();
        List<Criterio> _criterios = new List<Criterio>();
        List<Order> _ordenacao = new List<Order>();
        int? _limit;
        int? _offset;

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

                            foreach (var prop in GetMappedProperties(typeof(T)))
                            {
                                var nomeColuna = GetColumn(prop);
                                var valor = reader[nomeColuna];

                                if (valor is System.DBNull) continue;
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
