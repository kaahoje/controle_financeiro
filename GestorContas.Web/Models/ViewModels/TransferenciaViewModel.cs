using System.ComponentModel.DataAnnotations;

namespace GestorContas.Web.Models.ViewModels
{
    public class TransferenciaViewModel
    {
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        [Display(Name = "Valor")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A categoria de transferência é obrigatória")]
        [Display(Name = "Categoria de Transferência")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "A conta de saída é obrigatória")]
        [Display(Name = "Conta de Saída")]
        public int ContaSaidaId { get; set; }

        [Required(ErrorMessage = "A conta de entrada é obrigatória")]
        [Display(Name = "Conta de Entrada")]
        public int ContaEntradaId { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Data { get; set; } = DateTime.Today;
    }
}
