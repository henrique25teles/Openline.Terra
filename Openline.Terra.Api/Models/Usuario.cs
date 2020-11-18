using Openline.Terra.Api.Context.Schema;
using Openline.Terra.Api.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Openline.Terra.Api.Models
{
    [Table("pub_usu")]
    public class Usuario : ModelBase
    {
        [PrimaryKey]
        [Column("cd_usu")]
        public override int Id { get; set; }

        [Column("nm_usu")]
        public string Nome { get; set; }

        [Column("senha_usu")]
        public string Senha { get; set; }

        [Column("sit_usu")]
        public bool IsAtivo { get; set; }

        [Column("alt_senha_prox_con")]
        public bool AlteraSenhaProximoLogin { get; set; }

    }
}
