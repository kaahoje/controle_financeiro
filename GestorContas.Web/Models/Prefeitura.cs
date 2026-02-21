using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorContas.Web.Models
{
    public class Prefeitura
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        public DateTime? Data { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal? Valor { get; set; }

        [Display(Name = "É Recebimento?")]
        public bool Entrada { get; set; }

        [Required(ErrorMessage = "A data de vencimento é obrigatória")]
        [Display(Name = "Vencimento da Parcela")]
        public DateTime? VencimentoDaParcela { get; set; }
    }
}
