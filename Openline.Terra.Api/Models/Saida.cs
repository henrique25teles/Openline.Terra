using Openline.Terra.Api.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openline.Terra.Api.Models
{
    [Table("estq_sai")]
    public class Saida : ModelBaseUnidade
    {
        [Column("cd_sai")]
        public override int Id { get; set; }

        [Column("cd_emp")]
        public override int EmpresaId { get; set; }

        [Column("cd_pes_un_emp")]
        public override int UnidadeId { get; set; }
        
        [Column("cd_sai")]
        public int CodigoSaida { get; set; }

        [Column("cd_nt")]
        public int CodigoNota { get; set; }

        [Column("dt_emis_sai")]
        public DateTime DataEmissaoSaida { get; set; }

        [Column("cd_pes")]
        public int Pessoa { get; set; }

        [Column("nm_cli_sai")]
        public string NomeCliente { get; set; }

        [Column("cd_pes_vend")]
        public int CodigoPessoaVenda { get; set; }

        [Column("cd_cond")]
        public int Cd_Cond { get; set; }

        [Column("vl_desc_sai")]
        public decimal ValorDesconto { get; set; }

        [Column("vl_jur_sai")]
        public decimal ValorJuros { get; set; }

        [Column("vl_to_sai")]
        public decimal ValorTotal { get; set; }

        [Column("vl_frete_sai")]
        public decimal ValorFrete { get; set; }

        [Column("vl_frete_tn_sai")]
        public decimal Vl_Frete_Tn_Sai { get; set; }

        [Column("vl_liq_sai")]
        public decimal ValorLiquidoSaida { get; set; }

        [Column("tp_sai")]
        public int TipoSaida { get; set; }

        [Column("nr_nf")]
        public int NumeroNota { get; set; }

        [Column("serie_tp_nf")]
        public string SerieNota { get; set; }

        [Column("dt_val_sai")]
        public DateTime Dt_Val_Sai { get; set; }

        [Column("st_sai")]
        public int SituacaoSaida { get; set; }

        [Column("tp_frete_sai")]
        public int TipoFrete { get; set; }

        [Column("dt_cad_sai")]
        public DateTime DataCadastro { get; set; }

        [Column("hr_cad_sai")]
        public string HoraCadastro { get; set; }

        [Column("cd_usu")]
        public int Usuario { get; set; }

        [Column("dt_atual_sai")]
        public DateTime DataAtualizacao { get; set; }

        [Column("obs_sai")]
        public string ObservacaoSaida { get; set; }

        [Column("qt_sai")]
        public decimal Quantidade { get; set; }

        [Column("cd_term")]
        public int Terminal { get; set; }

        [Column("tp_nf")]
        public int TipoNotaFiscal { get; set; }

        [Column("end_fatur_sai")]
        public int EnderecoFaturamento { get; set; }

        [Column("vl_outros_sai")]
        public decimal ValorOutras { get; set; }

        [Column("cd_pes_transp")]
        public int Transportadora { get; set; }

        [Column("cd_tp_doc")]
        public int TipoDocumento { get; set; }

        [Column("cd_centro_sai")]
        public int CentroCusto { get; set; }

        [Column("cd_pes_carreg")]
        public int Cd_Pes_Carreg { get; set; }

        [Column("cd_area")]
        public int Area { get; set; }

        [Column("st_entrg")]
        public int St_Entrg { get; set; }

        [Column("dt_entrega")]
        public DateTime DataEntrega { get; set; }

        [Column("hr_entrega")]
        public string HoraEntrega { get; set; }

        [Column("rom_entrg")]
        public int RomaneioEntrega { get; set; }

        [Column("vl_custo_sai")]
        public decimal ValorCusto { get; set; }

        [Column("vl_dev_sai")]
        public decimal ValorDevolucao { get; set; }

        [Column("st_imp_ped")]
        public int St_Imp_Ped { get; set; }

        [Column("nr_cupom")]
        public decimal nr_cupom { get; set; }

        [Column("cd_term_cupom")]
        public decimal cd_term_cupom { get; set; }

        [Column("cd_ecf")]
        public int CodigoECF { get; set; }

        [Column("vl_custo")]
        public decimal Vl_Custo { get; set; }

        [Column("st_imp_ped_via_conf")]
        public int st_imp_ped_via_conf { get; set; }

        [Column("st_imp_ped_via_entrega")]
        public int st_imp_ped_via_entrega { get; set; }

        [Column("prc_a_def")]
        public bool PrecoADefinir { get; set; }

        [Column("venda_com_lt")]
        public int venda_com_lt { get; set; }

        [Column("nr_ped_cli")]
        public string nr_ped_cli { get; set; }

        [Column("orig_sai")]
        public int OrigemSaida { get; set; }

        [InverseProperty("cd_sai")]
        public List<SaidaItem> SaidaItems { get; set; }
    }
}
