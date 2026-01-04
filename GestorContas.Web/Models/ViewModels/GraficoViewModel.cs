using System.Collections.Generic;

namespace GestorContas.Web.Models.ViewModels
{
    public class GraficoViewModel
    {
        public List<string>? Labels { get; set; }
        public List<decimal>? Valores { get; set; }
        public List<string>? Cores { get; set; }
        public string? Titulo { get; set; }
    }
}
