using GestorContas.Web.Models;
using GestorContas.Web.Services.Conciliacao.Dtos;
using System.Collections.Generic;

namespace GestorContas.Web.Services.Conciliacao
{
    public interface IConciliacaoExtratoStrategy
    {
        ResultadoConciliacaoDto Conciliar(List<ItemExtratoDto> itensExtrato, List<Lancamento> lancamentosDb);
        List<ResultadoVerificacaoDuplicadoDto> VerificarDuplicados(List<ItemExtratoDto> itensExtrato, List<Lancamento> lancamentosDb, List<Lancamento>? historicoCompletoDb = null);
    }
}
