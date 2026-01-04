using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GestorContas.Web.Models.Enums;

namespace GestorContas.Web.Models.ViewModels
{
    public class ResumoMensalViewModel
    {
        [Display(Name = "Mês/Ano")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime MesAno { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        [Display(Name = "Total de Entradas")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalEntradas { get; set; }

        [Display(Name = "Total de Saídas")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalSaidas { get; set; }

        [Display(Name = "Saldo do Mês")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal SaldoMensal => TotalEntradas - TotalSaidas;
    }
}
