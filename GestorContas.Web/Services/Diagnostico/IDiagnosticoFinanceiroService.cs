using GestorContas.Web.Services.Diagnostico.Dtos;
using System.Threading.Tasks;

namespace GestorContas.Web.Services.Diagnostico
{
    public interface IDiagnosticoFinanceiroService
    {
        Task<DiagnosticoFinanceiroDto> GerarDiagnosticoCompletoAsync();
    }
}
