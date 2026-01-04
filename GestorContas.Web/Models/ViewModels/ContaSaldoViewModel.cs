
using System.ComponentModel.DataAnnotations;

namespace GestorContas.Web.Models.ViewModels
{
    public class ContaSaldoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Saldo { get; set; }
        
        public bool Ativa { get; set; }
        public string? Descricao { get; set; }
    }
}
