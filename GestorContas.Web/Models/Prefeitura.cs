using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorContas.Web.Models
{
    public class Prefeitura
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        public DateTime? Data { get; set; }

        public decimal? Valor { get; set; }

        public bool? Entrada { get; set; }

        [Display(Name = "Vencimento da Parcela")]
        public DateTime? VencimentoDaParcela { get; set; }
    }
}
