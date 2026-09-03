using GestorContas.Web.Models;
using GestorContas.Web.Services.Conciliacao.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorContas.Web.Services.Conciliacao
{
    public interface IConciliacaoBancariaService
    {
        Task<List<ResultadoVerificacaoDuplicadoDto>> VerificarDuplicadosAsync(int contaId, List<ItemExtratoDto> lancamentos);
        Task<ResultadoConciliacaoDto> ProcessarComparacaoExtratoAsync(int contaId, List<ItemExtratoDto> lancamentos);
        Task<SugestaoDescricaoDto?> ObterSugestaoPorDescricaoAsync(int contaId, string descricao);
        Task<int> SalvarLancamentosImportadosAsync(List<Lancamento> lancamentos);
    }
}
