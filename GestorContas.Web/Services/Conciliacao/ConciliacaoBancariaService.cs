using GestorContas.Web.Data;
using GestorContas.Web.Models;
using GestorContas.Web.Services.Conciliacao.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorContas.Web.Services.Conciliacao
{
    public class ConciliacaoBancariaService : IConciliacaoBancariaService
    {
        private readonly AppDbContext _context;
        private readonly IConciliacaoExtratoStrategy _strategy;

        public ConciliacaoBancariaService(AppDbContext context, IConciliacaoExtratoStrategy strategy)
        {
            _context = context;
            _strategy = strategy;
        }

        public async Task<List<ResultadoVerificacaoDuplicadoDto>> VerificarDuplicadosAsync(int contaId, List<ItemExtratoDto> lancamentos)
        {
            if (lancamentos == null || !lancamentos.Any())
                return new List<ResultadoVerificacaoDuplicadoDto>();

            var datas = lancamentos.Select(l => l.Data.Date).Distinct().ToList();
            if (!datas.Any())
                return new List<ResultadoVerificacaoDuplicadoDto>();

            var minDate = datas.Min();
            var maxDate = datas.Max();

            var existentesNoPeriodo = await _context.Lancamentos
                .Include(l => l.Categoria)
                .Where(l => l.ContaId == contaId && l.Data.Date >= minDate && l.Data.Date <= maxDate)
                .ToListAsync();

            var historicoCompleto = await _context.Lancamentos
                .Include(l => l.Categoria)
                .Where(l => l.ContaId == contaId)
                .OrderByDescending(l => l.Id)
                .Take(2000)
                .ToListAsync();

            return _strategy.VerificarDuplicados(lancamentos, existentesNoPeriodo, historicoCompleto);
        }

        public async Task<ResultadoConciliacaoDto> ProcessarComparacaoExtratoAsync(int contaId, List<ItemExtratoDto> lancamentos)
        {
            if (lancamentos == null || !lancamentos.Any())
                return new ResultadoConciliacaoDto();

            var datas = lancamentos.Select(l => l.Data.Date).Distinct().ToList();
            if (!datas.Any())
                return new ResultadoConciliacaoDto();

            var minDate = datas.Min();
            var maxDate = datas.Max();

            var lancamentosDb = await _context.Lancamentos
                .Include(l => l.Categoria)
                .Include(l => l.Conta)
                .Where(l => l.ContaId == contaId && l.Data.Date >= minDate && l.Data.Date <= maxDate)
                .OrderBy(l => l.Data)
                .ToListAsync();

            return _strategy.Conciliar(lancamentos, lancamentosDb);
        }

        public async Task<SugestaoDescricaoDto?> ObterSugestaoPorDescricaoAsync(int contaId, string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                return null;

            var descNorm = descricao.Trim().ToLowerInvariant();

            var lancamentos = await _context.Lancamentos
                .Include(l => l.Categoria)
                .Where(l => l.ContaId == contaId)
                .OrderByDescending(l => l.Id)
                .Take(2000)
                .ToListAsync();

            // 1ª Preferência: Descricao exata
            var match = lancamentos.FirstOrDefault(l => l.Descricao.Trim().ToLowerInvariant() == descNorm);
            
            // 2ª Preferência: DescricaoNoExtrato exata
            if (match == null)
                match = lancamentos.FirstOrDefault(l => !string.IsNullOrEmpty(l.DescricaoNoExtrato) && l.DescricaoNoExtrato.Trim().ToLowerInvariant() == descNorm);

            // 3ª Preferência: Descricao contida/que contém
            if (match == null)
                match = lancamentos.FirstOrDefault(l => l.Descricao.Trim().ToLowerInvariant().Contains(descNorm) || descNorm.Contains(l.Descricao.Trim().ToLowerInvariant()));

            // 4ª Preferência: DescricaoNoExtrato contida/que contém
            if (match == null)
                match = lancamentos.FirstOrDefault(l => !string.IsNullOrEmpty(l.DescricaoNoExtrato) && (l.DescricaoNoExtrato.Trim().ToLowerInvariant().Contains(descNorm) || descNorm.Contains(l.DescricaoNoExtrato.Trim().ToLowerInvariant())));

            if (match == null)
                return null;

            return new SugestaoDescricaoDto
            {
                CategoriaId = match.CategoriaId,
                CategoriaNome = match.Categoria?.Nome ?? "Sem Categoria",
                Tipo = match.Tipo
            };
        }

        public async Task<int> SalvarLancamentosImportadosAsync(List<Lancamento> lancamentos)
        {
            if (lancamentos == null || !lancamentos.Any())
                return 0;

            foreach (var lancamento in lancamentos)
            {
                lancamento.Id = 0;
                _context.Lancamentos.Add(lancamento);
            }

            return await _context.SaveChangesAsync();
        }
    }
}
