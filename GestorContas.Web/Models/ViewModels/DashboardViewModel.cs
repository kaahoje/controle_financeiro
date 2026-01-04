
namespace GestorContas.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public ResumoMensalViewModel ResumoMensal { get; set; } = new();
        public List<ContaSaldoViewModel> Contas { get; set; } = new();
        public GraficoViewModel GraficoFluxoCaixa { get; set; } = new();
        public GraficoViewModel GraficoDespesas { get; set; } = new();
    }
}
