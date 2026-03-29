
using System.Collections.Generic;
using GestorContas.Web.Models;

namespace GestorContas.Web.Models.ViewModels
{
    public class LancamentosAgrupadosViewModel
    {
        public string CategoriaNome { get; set; }
        public decimal TotalGeral { get; set; }
        public List<GrupoContaViewModel> GruposPorConta { get; set; } = new();
    }

    public class GrupoContaViewModel
    {
        public string ContaNome { get; set; }
        public decimal TotalConta { get; set; }
        public List<Lancamento> Lancamentos { get; set; } = new();
    }
}
