using Openline.Terra.Api.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openline.Terra.Api.Models
{
    [Table("estq_sai_it")]
    public class SaidaItem : ModelBaseUnidade
    {
        [Column("nr_item")]
        public override int Id { get; set; }

        [Column("cd_emp")]
        public override int EmpresaId { get; set; }

        [Column("cd_pes_un_emp")]
        public override int UnidadeId { get; set; }

        [Column("cd_sai")]
        [ForeignKey("estq_sai_it_estq_sai_fk")]
        public int Saida_id { get; set; }

        [Column("cd_prod")]
        public int Produto_id { get; set; }

        [Column("st_it")]
        public string Cst { get; set; }
        
        [Column("qtde_it")]
        public decimal Quantidade { get; set; }

        [Column("preco_it")]
        public decimal Preco { get; set; }

        [Column("vl_desc_it")]
        public decimal? Desconto { get; set; }

        [Column("tx_icms_it")]
        public decimal? TaxaIcms { get; set; }

        [Column("tx_red_it")]
        public decimal? TaxaReducao { get; set; }

        [Column("tx_ipi_it")]
        public decimal? TaxaIpi { get; set; }

        [Column("vl_to_it")]
        public decimal ValorTotal { get; set; }

        [Column("sts_it")]
        public int? sts_it { get; set; }

        [Column("vl_jur_it")]
        public decimal? ValorJuros { get; set; }

        [Column("vl_frete_it")]
        public decimal? ValorFrete { get; set; }

        [Column("tx_jur_it")]
        public decimal? TaxaJuros { get; set; }

        [Column("preco_custo_it")]
        public decimal PrecoCusto { get; set; }

        [Column("cfop_it")]
        public int Cfop { get; set; }

        [Column("cd_un_med_conv")]
        public string UnidadeMedidaConversao { get; set; }

        [Column("qtde_conv_it")]
        public decimal? QuantidadeUnidadeMedidaConversao { get; set; }

        [Column("preco_conv_it")]
        public decimal? PrecoConversao { get; set; }

        [Column("qtde_emb_it")]
        public decimal? QuantidadeEmbalagem { get; set; }

        [Column("ds_prod_especial")]
        public string DescricaoEspecial { get; set; }

        [Column("rateio_frete")]
        public decimal? RateioFrete { get; set; }

        [Column("rateio_desc_financ")]
        public decimal? RateioDescontoFinanceiro { get; set; }

        [Column("rateio_desc_real")]
        public decimal? RateioDescontoRealizado { get; set; }

        [Column("qtde_dev")]
        public decimal? QuantidadeDevolvida { get; set; }

        [Column("cd_tbl_prc")]
        public int? TabelaPreco { get; set;}

        [Column("csosn_it")]
        public int Csosn { get; set; }

        [Column("perc_apr_cred_it")]
        public decimal perc_apr_cred_it { get; set; }

        [Column("vl_custo")]
        public decimal? ValorCusto { get; set; }

        [Column("cd_rastreio")]
        public long? CodigoRastreio { get; set; }

        [Column("cd_lt_produtor")]
        public long? cd_lt_produtor { get; set; }

        [Column("cd_prod_cliente")]
        public long? cd_prod_cliente { get; set; }

    }
}
