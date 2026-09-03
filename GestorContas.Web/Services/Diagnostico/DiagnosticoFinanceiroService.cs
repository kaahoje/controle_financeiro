using GestorContas.Web.Data;
using GestorContas.Web.Models.Enums;
using GestorContas.Web.Services.Diagnostico.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestorContas.Web.Services.Diagnostico
{
    public class DiagnosticoFinanceiroService : IDiagnosticoFinanceiroService
    {
        private readonly AppDbContext _context;

        public DiagnosticoFinanceiroService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DiagnosticoFinanceiroDto> GerarDiagnosticoCompletoAsync()
        {
            var diagnostico = new DiagnosticoFinanceiroDto();

            var contas = await _context.Contas
                .Include(c => c.Lancamentos!)
                    .ThenInclude(l => l.Categoria)
                .Where(c => c.ConsiderarNoDiagnostico)
                .ToListAsync();

            var todosLancamentos = await _context.Lancamentos
                .Include(l => l.Categoria)
                .Include(l => l.Conta)
                .Where(l => l.Conta.ConsiderarNoDiagnostico)
                .OrderBy(l => l.Data)
                .ThenBy(l => l.Id)
                .ToListAsync();

            // 1. Diagnóstico por Conta
            decimal saldoTotalAcumulado = 0;
            decimal saldoInicialTotal = 0;

            foreach (var conta in contas)
            {
                var entradasNormal = conta.Lancamentos?
                    .Where(l => l.Tipo == TipoLancamento.Entrada && l.Categoria?.ParaTransferencia == false)
                    .Sum(l => l.Valor) ?? 0;

                var saidasNormal = conta.Lancamentos?
                    .Where(l => l.Tipo == TipoLancamento.Saida && l.Categoria?.ParaTransferencia == false)
                    .Sum(l => l.Valor) ?? 0;

                var transfEnviadas = conta.Lancamentos?
                    .Where(l => l.Tipo == TipoLancamento.Saida && l.Categoria?.ParaTransferencia == true)
                    .Sum(l => l.Valor) ?? 0;

                var transfRecebidas = conta.Lancamentos?
                    .Where(l => l.Tipo == TipoLancamento.Entrada && l.Categoria?.ParaTransferencia == true)
                    .Sum(l => l.Valor) ?? 0;

                var saldoConta = conta.SaldoInicial + (entradasNormal + transfRecebidas) - (saidasNormal + transfEnviadas);

                saldoInicialTotal += conta.SaldoInicial;
                saldoTotalAcumulado += saldoConta;

                var lancsConta = conta.Lancamentos?.OrderBy(l => l.Data).ToList();

                diagnostico.DiagnosticoContas.Add(new DiagnosticoContaDto
                {
                    ContaId = conta.Id,
                    NomeConta = conta.Nome,
                    SaldoInicial = conta.SaldoInicial,
                    TotalEntradas = entradasNormal,
                    TotalSaidas = saidasNormal,
                    TotalTransferenciasEnviadas = transfEnviadas,
                    TotalTransferenciasRecebidas = transfRecebidas,
                    SaldoAtualCalculado = saldoConta,
                    TotalLancamentos = conta.Lancamentos?.Count ?? 0,
                    DataPrimeiroLancamento = lancsConta?.FirstOrDefault()?.Data,
                    DataUltimoLancamento = lancsConta?.LastOrDefault()?.Data
                });
            }

            // 2. Resumo Geral
            var totalEntradasGeral = todosLancamentos
                .Where(l => l.Tipo == TipoLancamento.Entrada && l.Categoria?.ParaTransferencia == false)
                .Sum(l => l.Valor);

            var totalSaidasGeral = todosLancamentos
                .Where(l => l.Tipo == TipoLancamento.Saida && l.Categoria?.ParaTransferencia == false)
                .Sum(l => l.Valor);

            var totalTransfSaida = todosLancamentos
                .Where(l => l.Tipo == TipoLancamento.Saida && l.Categoria?.ParaTransferencia == true)
                .Sum(l => l.Valor);

            var totalTransfEntrada = todosLancamentos
                .Where(l => l.Tipo == TipoLancamento.Entrada && l.Categoria?.ParaTransferencia == true)
                .Sum(l => l.Valor);

            diagnostico.ResumoGeral = new ResumoDiagnosticoGeralDto
            {
                SaldoInicialTotalContas = saldoInicialTotal,
                SaldoAtualTotalSistema = saldoTotalAcumulado,
                TotalEntradasSemTransferencia = totalEntradasGeral,
                TotalSaidasSemTransferencia = totalSaidasGeral,
                ResultadoRealAcumulado = totalEntradasGeral - totalSaidasGeral,
                TotalTransferenciasSaida = totalTransfSaida,
                TotalTransferenciasEntrada = totalTransfEntrada,
                DesbalancoTransferencias = totalTransfEntrada - totalTransfSaida
            };

            // 3. Detecção de Duplicados Suspeitos na Base
            var gruposDuplicados = todosLancamentos
                .GroupBy(l => new { l.Data.Date, l.Valor, l.Tipo, l.ContaId })
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var g in gruposDuplicados)
            {
                var contaNome = g.First().Conta?.Nome ?? "N/A";
                var qtd = g.Count();
                var valorUnitario = g.Key.Valor;
                // Excesso = (Qtd - 1) * Valor (o quanto duplicados inflacionaram)
                var impacto = (qtd - 1) * valorUnitario;

                diagnostico.DuplicadosSuspeitos.Add(new DiagnosticoDuplicadoDto
                {
                    Data = g.Key.Date,
                    Valor = valorUnitario,
                    Tipo = g.Key.Tipo == TipoLancamento.Entrada ? "Entrada" : "Saída",
                    ContaNome = contaNome,
                    QuantidadeDuplicados = qtd,
                    ImpactoFinanceiro = impacto,
                    LancamentosIds = g.Select(x => x.Id).ToList(),
                    Descricoes = g.Select(x => x.Descricao).Distinct().ToList()
                });
            }

            diagnostico.ResumoGeral.TotalDuplicadosIdentificados = diagnostico.DuplicadosSuspeitos.Sum(x => x.QuantidadeDuplicados - 1);
            diagnostico.ResumoGeral.ValorTotalDuplicados = diagnostico.DuplicadosSuspeitos.Sum(x => x.ImpactoFinanceiro);

            // 4. Detecção de Transferências Atípicas / Não pareadas
            var lancamentosTransferencia = todosLancamentos
                .Where(l => l.Categoria?.ParaTransferencia == true)
                .ToList();

            foreach (var t in lancamentosTransferencia)
            {
                // Verificar se existe um par em outra conta no mesmo dia (ou dia proximo +-1 dia) com o mesmo valor e tipo oposto
                var temPar = lancamentosTransferencia.Any(outra =>
                    outra.Id != t.Id &&
                    outra.ContaId != t.ContaId &&
                    outra.Valor == t.Valor &&
                    outra.Tipo != t.Tipo &&
                    Math.Abs((outra.Data - t.Data).TotalDays) <= 1
                );

                if (!temPar)
                {
                    diagnostico.TransferenciasAtipicas.Add(new DiagnosticoTransferenciaAtipicaDto
                    {
                        Id = t.Id,
                        Data = t.Data,
                        Descricao = t.Descricao,
                        Valor = t.Valor,
                        Tipo = t.Tipo == TipoLancamento.Entrada ? "Entrada" : "Saída",
                        ContaNome = t.Conta?.Nome ?? "N/A",
                        CategoriaNome = t.Categoria?.Nome ?? "N/A",
                        MotivoAlert = "Transferência sem lançamento correspondente de contrapartida na outra conta."
                    });
                }
            }

            // 5. Suspeitas de Erro de Sinal / Descrição divergente
            foreach (var l in todosLancamentos)
            {
                var descUpper = (l.Descricao + " " + (l.DescricaoNoExtrato ?? "")).ToUpperInvariant();

                if (l.Tipo == TipoLancamento.Saida && (descUpper.Contains("RENDIMENTO") || descUpper.Contains("RESGATE") || descUpper.Contains("ESTORNO") || descUpper.Contains("PIX RECEBIDO")))
                {
                    diagnostico.ErrosSinalOuTipo.Add(new PossivelErroSinalDto
                    {
                        Id = l.Id,
                        Data = l.Data,
                        Descricao = l.Descricao,
                        Valor = l.Valor,
                        Tipo = "Saída",
                        CategoriaNome = l.Categoria?.Nome ?? "N/A",
                        Suspeita = "Registrado como SAÍDA mas possui termos de RECEITA/ESTORNO na descrição."
                    });
                }
                else if (l.Tipo == TipoLancamento.Entrada && (descUpper.Contains("PAGAMENTO") || descUpper.Contains("TARIFA") || descUpper.Contains("COMPRA") || descUpper.Contains("PIX ENVIADO") || descUpper.Contains("SAQUE")))
                {
                    diagnostico.ErrosSinalOuTipo.Add(new PossivelErroSinalDto
                    {
                        Id = l.Id,
                        Data = l.Data,
                        Descricao = l.Descricao,
                        Valor = l.Valor,
                        Tipo = "Entrada",
                        CategoriaNome = l.Categoria?.Nome ?? "N/A",
                        Suspeita = "Registrado como ENTRADA mas possui termos de DESPESA/PAGAMENTO na descrição."
                    });
                }
            }

            // 6. Evolução Mensal Histórica Completa
            if (todosLancamentos.Any())
            {
                var minData = todosLancamentos.Min(x => x.Data);
                var maxData = todosLancamentos.Max(x => x.Data);

                var cursor = new DateTime(minData.Year, minData.Month, 1);
                var fim = new DateTime(maxData.Year, maxData.Month, 1);

                decimal acumuladoHistorico = saldoInicialTotal;
                decimal acumuladoPuroResultados = saldoInicialTotal;

                while (cursor <= fim)
                {
                    var primeiroDia = cursor;
                    var ultimoDia = cursor.AddMonths(1).AddDays(-1);

                    var lancsMes = todosLancamentos
                        .Where(x => x.Data >= primeiroDia && x.Data <= ultimoDia)
                        .ToList();

                    var entOp = lancsMes.Where(x => x.Tipo == TipoLancamento.Entrada && x.Categoria?.ParaTransferencia == false).Sum(x => x.Valor);
                    var saiOp = lancsMes.Where(x => x.Tipo == TipoLancamento.Saida && x.Categoria?.ParaTransferencia == false).Sum(x => x.Valor);
                    var resOp = entOp - saiOp;

                    // Somando saldo acumulado de todas as movimentações
                    var entTotal = lancsMes.Where(x => x.Tipo == TipoLancamento.Entrada).Sum(x => x.Valor);
                    var saiTotal = lancsMes.Where(x => x.Tipo == TipoLancamento.Saida).Sum(x => x.Valor);

                    acumuladoHistorico += (entTotal - saiTotal);
                    acumuladoPuroResultados += resOp;

                    var detalhesLancs = lancsMes.Select(l => new LancamentoDetalheMesDto
                    {
                        Id = l.Id,
                        Data = l.Data,
                        Descricao = l.Descricao,
                        Valor = l.Valor,
                        Tipo = l.Tipo == TipoLancamento.Entrada ? "Entrada" : "Saída",
                        ContaNome = l.Conta?.Nome ?? "N/A",
                        CategoriaNome = l.Categoria?.Nome ?? "N/A",
                        IsTransferencia = l.Categoria?.ParaTransferencia ?? false
                    }).ToList();

                    diagnostico.EvolucaoMensal.Add(new EvolucaoMensalDiagnosticoDto
                    {
                        Ano = cursor.Year,
                        Mes = cursor.Month,
                        MesAnoLabel = cursor.ToString("MMM/yyyy"),
                        EntradasOperacionais = entOp,
                        SaidasOperacionais = saiOp,
                        ResultadoOperacionalMes = resOp,
                        SaldoAcumuladoResultadoMes = acumuladoPuroResultados,
                        TransferenciasMes = lancsMes.Where(x => x.Categoria?.ParaTransferencia == true).Sum(x => x.Valor),
                        SaldoAcumuladoFimMes = acumuladoHistorico,
                        DivergenciaSaldo = acumuladoHistorico - acumuladoPuroResultados,
                        LancamentosMes = detalhesLancs
                    });

                    cursor = cursor.AddMonths(1);
                }
            }

            return diagnostico;
        }
    }
}
