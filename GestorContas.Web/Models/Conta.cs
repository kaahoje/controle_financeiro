using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorContas.Web.Models
{
    public class Conta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Saldo Inicial")]
        public decimal SaldoInicial { get; set; } = 0;

        [Display(Name = "Ativa")]
        public bool Ativa { get; set; } = true;

        // Navigation property
        public ICollection<Lancamento>? Lancamentos { get; set; }
    }
}
