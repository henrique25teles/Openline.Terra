using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Openline.Terra.Api.Models.Base;

namespace Openline.Terra.Api.Models.Tabela
{
    [Table("tabela")]
    public class Tabela : ModelBase
    {
        [Key]
        [Column("codigo")]
        [Required]
        public override int Id { get; set; }

        [Column("descricao")]
        [Required]
        public string Descricao { get; set; }
    }
}
