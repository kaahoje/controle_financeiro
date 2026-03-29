using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorContas.Web.Models;
using GestorContas.Web.Models.ViewModels;
using GestorContas.Web.Models.Enums;
using GestorContas.Web.Data;

namespace GestorContas.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(int? mes, int? ano)
    {
        var dashboard = new DashboardViewModel();
        
        // Determinar mês/ano a exibir
        var mesReferencia = mes ?? DateTime.Today.Month;
        var anoReferencia = ano ?? DateTime.Today.Year;
        var dataReferencia = new DateTime(anoReferencia, mesReferencia, 1);
        
        // Passar para ViewBag para o seletor
        ViewBag.MesSelecionado = mesReferencia;
        ViewBag.AnoSelecionado = anoReferencia;
        
        // 1. Calcular saldos das contas (Saldo Inicial + Entradas - Saídas)
        var contas = await _context.Contas
            .Include(c => c.Lancamentos)
            .Where(c => c.Ativa)
            .ToListAsync();

        foreach (var conta in contas)
        {
            var totalEntradas = conta.Lancamentos?
                .Where(l => l.Tipo == TipoLancamento.Entrada)
                .Sum(l => l.Valor) ?? 0;

            var totalSaidas = conta.Lancamentos?
                .Where(l => l.Tipo == TipoLancamento.Saida)
                .Sum(l => l.Valor) ?? 0;

            dashboard.Contas.Add(new ContaSaldoViewModel
            {
                Id = conta.Id,
                Nome = conta.Nome,
                Descricao = conta.Descricao,
                Ativa = conta.Ativa,
                Saldo = conta.SaldoInicial + totalEntradas - totalSaidas
            });
        }

        // 2. Resumo do Mês Selecionado
        var primeiroDiaMes = dataReferencia;
        var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

        var lancamentosMes = await _context.Lancamentos
            .Include(l => l.Categoria)
            .Where(l => l.Data >= primeiroDiaMes && l.Data <= ultimoDiaMes)
            .ToListAsync();
        
        // Filtro para excluir categorias de transferência interna do resumo mensal (visão de resultado real)
        lancamentosMes = lancamentosMes
            .Where(x => x.Categoria?.ParaTransferencia == false)
            .ToList();
        
        dashboard.ResumoMensal = new ResumoMensalViewModel
        {
            MesAno = primeiroDiaMes,
            TotalEntradas = lancamentosMes
                .Where(l => l.Tipo == TipoLancamento.Entrada)
                .Sum(l => l.Valor),
            TotalSaidas = lancamentosMes
                .Where(l => l.Tipo == TipoLancamento.Saida)
                .Sum(l => l.Valor)
        };

        // 3. Gráfico de Entradas x Saídas (Mês Selecionado)
        dashboard.GraficoFluxoCaixa = new GraficoViewModel
        {
            Titulo = "Fluxo de Caixa (Mês Selecionado)",
            Labels = new List<string> { "Entradas", "Saídas" },
            Valores = new List<decimal> 
            { 
                dashboard.ResumoMensal.TotalEntradas, 
                dashboard.ResumoMensal.TotalSaidas 
            },
            Cores = new List<string> { "#198754", "#dc3545" } // Success (Green), Danger (Red)
        };

        // 4. Gráfico de Despesas por Categoria (Mês Selecionado)
        var gastosPorCategoriaQuery = _context.Lancamentos
            .Include(l => l.Categoria)
            .Where(l => !l.Categoria.ParaTransferencia)
            .Where(l => l.Tipo == TipoLancamento.Saida && 
                       l.Data >= primeiroDiaMes && 
                       l.Data <= ultimoDiaMes)
            .ToList()
            .GroupBy(l => new { l.Categoria.Id, l.Categoria.Nome })
            .Select(g => new CategoriaResumoViewModel 
            { 
                CategoriaId = g.Key.Id, 
                CategoriaNome = g.Key.Nome, 
                Total = g.Sum(l => l.Valor) 
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        dashboard.DespesasPorCategoria = gastosPorCategoriaQuery;

        dashboard.GraficoDespesas = new GraficoViewModel
        {
            Titulo = "Top Despesas por Categoria",
            Labels = gastosPorCategoriaQuery.Take(10).Select(x => x.CategoriaNome).ToList(),
            Valores = gastosPorCategoriaQuery.Take(10).Select(x => x.Total).ToList(),
            Cores = new List<string> 
            { 
                "#0d6efd", "#6610f2", "#6f42c1", "#d63384", "#dc3545", 
                "#fd7e14", "#ffc107", "#198754", "#20c997", "#0dcaf0" 
            }
        };

        // 5. Gráfico de Evolução de Saldo (Últimos 6 meses)
        var fimPeriodo = DateTime.Today;
        var inicioPeriodo = fimPeriodo.AddMonths(-5); // 6 meses total
        inicioPeriodo = new DateTime(inicioPeriodo.Year, inicioPeriodo.Month, 1);

        var mesesLabels = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            mesesLabels.Add(inicioPeriodo.AddMonths(i).ToString("MMM/yyyy"));
        }

        dashboard.GraficoEvolucaoSaldo = new GraficoViewModel
        {
            Titulo = "Evolução do Saldo (Últimos 6 meses)",
            Labels = mesesLabels,
            Datasets = new List<DatasetViewModel>()
        };

        var coresGrafico = new List<string> { "#0d6efd", "#198754", "#dc3545", "#ffc107", "#0dcaf0", "#6610f2", "#fd7e14", "#20c997" };
        int corIndex = 0;

        foreach (var conta in contas)
        {
            var dataset = new DatasetViewModel
            {
                Label = conta.Nome,
                Data = new List<decimal>(),
                BorderColor = coresGrafico[corIndex % coresGrafico.Count],
                BackgroundColor = coresGrafico[corIndex % coresGrafico.Count],
                Fill = false
            };
            corIndex++;

            for (int i = 0; i < 6; i++)
            {
                var dataReferenciaEvolucao = inicioPeriodo.AddMonths(i + 1).AddDays(-1); // Último dia do mês

                var totalEntradas = conta.Lancamentos?
                    .Where(l => l.Tipo == TipoLancamento.Entrada && l.Data <= dataReferenciaEvolucao)
                    .Sum(l => l.Valor) ?? 0;

                var totalSaidas = conta.Lancamentos?
                    .Where(l => l.Tipo == TipoLancamento.Saida && l.Data <= dataReferenciaEvolucao)
                    .Sum(l => l.Valor) ?? 0;

                dataset.Data.Add(conta.SaldoInicial + totalEntradas - totalSaidas);
            }

            dashboard.GraficoEvolucaoSaldo.Datasets.Add(dataset);
        }

        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
