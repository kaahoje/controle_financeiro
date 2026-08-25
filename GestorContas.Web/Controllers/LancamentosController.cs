using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestorContas.Web.Data;
using GestorContas.Web.Models;
using GestorContas.Web.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using GestorContas.Web.Models.ViewModels;

namespace GestorContas.Web.Controllers
{
    public class LancamentosController : Controller
    {
        private readonly AppDbContext _context;

        public LancamentosController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAjaxRequest => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // GET: Lancamentos
        public async Task<IActionResult> Index(int? mes, int? ano, int? categoriaId, int? contaId, TipoLancamento? tipo, bool? paraTransferencia, bool agruparPorData = false, bool todos = false)
        {
            var dataAtual = DateTime.Today;
            var mesAtual = mes ?? dataAtual.Month;
            var anoAtual = ano ?? dataAtual.Year;

            ViewBag.AgruparPorData = agruparPorData;

            ViewBag.MesSelecionado = mes ?? dataAtual.Month;
            ViewBag.AnoSelecionado = anoAtual;
            ViewBag.Meses = Enumerable.Range(1, 12)
                .Select(m => new { Id = m, Nome = new DateTime(2000, m, 1).ToString("MMMM") })
                .ToList();
            ViewBag.Anos = Enumerable.Range(DateTime.Today.Year - 5, 10).ToList();
            ViewBag.Tipos = Enum.GetValues(typeof(TipoLancamento)).Cast<TipoLancamento>();
            ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", categoriaId);
            ViewBag.Contas = new SelectList(await _context.Contas.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", contaId);

            var query = _context.Lancamentos
                .Include(l => l.Categoria)
                .Include(l => l.Conta)
                .AsQueryable();

            if (mes != 0 && ano != 0)
            {
                query = query.Where(l => l.Data.Year == anoAtual && l.Data.Month == mesAtual);
            }
            else if (mes != 0)
            {
                query = query.Where(l => l.Data.Month == mesAtual);
            }
            else if (ano != 0)
            {
                query = query.Where(l => l.Data.Year == anoAtual);
            }
            
            ViewBag.MesSelecionado = mes ?? (todos ? 0 : dataAtual.Month);
            ViewBag.AnoSelecionado = ano ?? (todos ? 0 : dataAtual.Year);

            if (categoriaId.HasValue)
            {
                query = query.Where(l => l.CategoriaId == categoriaId.Value);
            }

            if (contaId.HasValue)
            {
                query = query.Where(l => l.ContaId == contaId.Value);
            }

            if (tipo.HasValue)
            {
                query = query.Where(l => l.Tipo == tipo.Value);
            }
            if (paraTransferencia.HasValue)
            {
                query = query.Where(l => l.Categoria.ParaTransferencia == paraTransferencia.Value);
            }

            var lancamentos = await query
                .OrderByDescending(l => l.Data)
                .ThenByDescending(l => l.Id)
                .ToListAsync();

            // Calcular totais - Ignorando transferências internas para refletir receitas/despesas reais
            ViewBag.TotalEntradas = lancamentos.Where(x => x.Categoria?.ParaTransferencia == false).Where(l => l.Tipo == TipoLancamento.Entrada).Sum(l => l.Valor);
            ViewBag.TotalSaidas = lancamentos.Where(x => x.Categoria?.ParaTransferencia == false).Where(l => l.Tipo == TipoLancamento.Saida).Sum(l => l.Valor);
            ViewBag.Saldo = ViewBag.TotalEntradas - ViewBag.TotalSaidas;

            return View(lancamentos);
        }

        private async Task<DateTime> ObterDataUltimoLancamentoFiltrado(int? mes, int? ano, int? categoriaId, int? contaId, TipoLancamento? tipo, bool? paraTransferencia)
        {
            var dataAtual = DateTime.Today;
            var mesAtual = mes ?? dataAtual.Month;
            var anoAtual = ano ?? dataAtual.Year;

            var query = _context.Lancamentos.AsQueryable();

            if (mes != 0 && ano != 0)
            {
                query = query.Where(l => l.Data.Year == anoAtual && l.Data.Month == mesAtual);
            }
            else if (mes != 0)
            {
                query = query.Where(l => l.Data.Month == mesAtual);
            }
            else if (ano != 0)
            {
                query = query.Where(l => l.Data.Year == anoAtual);
            }

            if (categoriaId.HasValue)
            {
                query = query.Where(l => l.CategoriaId == categoriaId.Value);
            }

            if (contaId.HasValue)
            {
                query = query.Where(l => l.ContaId == contaId.Value);
            }

            if (tipo.HasValue)
            {
                query = query.Where(l => l.Tipo == tipo.Value);
            }
            if (paraTransferencia.HasValue)
            {
                query = query.Where(l => l.Categoria.ParaTransferencia == paraTransferencia.Value);
            }

            var ultimo = await query.OrderByDescending(l => l.Id).FirstOrDefaultAsync();
            return ultimo?.Data ?? dataAtual;
        }

        // GET: Lancamentos/Create
        public async Task<IActionResult> Create(int? mes, int? ano, int? categoriaId, int? contaId, TipoLancamento? tipo, bool? paraTransferencia)
        {
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            
            var dataInicial = await ObterDataUltimoLancamentoFiltrado(mes, ano, categoriaId, contaId, tipo, paraTransferencia);
            var model = new Lancamento { Data = dataInicial };

            if (IsAjaxRequest)
                return PartialView(model);
                
            return View(model);
        }

        // POST: Lancamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Valor,Tipo,Data,CategoriaId,ContaId")] Lancamento lancamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lancamento);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Lançamento cadastrado com sucesso!";
                
                if (IsAjaxRequest)
                    return Json(new { success = true });
                    
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.CategoriaId);
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.ContaId);
            
            if (IsAjaxRequest)
                return PartialView(lancamento);
                
            return View(lancamento);
        }

        // GET: Lancamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lancamento = await _context.Lancamentos.FindAsync(id);
            if (lancamento == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.CategoriaId);
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.ContaId);
            
            if (IsAjaxRequest)
                return PartialView(lancamento);
                
            return View(lancamento);
        }

        // POST: Lancamentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Valor,Tipo,Data,CategoriaId,ContaId")] Lancamento lancamento)
        {
            if (id != lancamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lancamento);
                    await _context.SaveChangesAsync();
                    TempData["MensagemSucesso"] = "Lançamento atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LancamentoExists(lancamento.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                if (IsAjaxRequest)
                    return Json(new { success = true });
                    
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.CategoriaId);
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.ContaId);
            
            if (IsAjaxRequest)
                return PartialView(lancamento);
                
            return View(lancamento);
        }

        // GET: Lancamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lancamento = await _context.Lancamentos
                .Include(l => l.Categoria)
                .Include(l => l.Conta)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lancamento == null)
            {
                return NotFound();
            }

            if (IsAjaxRequest)
                return PartialView(lancamento);

            return View(lancamento);
        }

        // POST: Lancamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lancamento = await _context.Lancamentos.FindAsync(id);
            if (lancamento != null)
            {
                _context.Lancamentos.Remove(lancamento);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Lançamento excluído com sucesso!";
            }
            
            if (IsAjaxRequest)
                return Json(new { success = true });
                
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transferencia(int? mes, int? ano, int? categoriaId, int? contaId, TipoLancamento? tipo, bool? paraTransferencia)
        {
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.Where(c => c.ParaTransferencia).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            
            var dataInicial = await ObterDataUltimoLancamentoFiltrado(mes, ano, categoriaId, contaId, tipo, paraTransferencia);
            var model = new GestorContas.Web.Models.ViewModels.TransferenciaViewModel { Data = dataInicial };

            if (IsAjaxRequest)
                return PartialView(model);
                
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transferencia(TransferenciaViewModel model)
        {
            if (model.ContaSaidaId == model.ContaEntradaId)
            {
                ModelState.AddModelError("ContaEntradaId", "A conta de entrada deve ser diferente da conta de saída.");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Lançamento de Saída
                    var lancamentoSaida = new Lancamento
                    {
                        Descricao = $"{model.Descricao} (Saída)",
                        Valor = model.Valor,
                        Tipo = TipoLancamento.Saida,
                        Data = model.Data,
                        CategoriaId = model.CategoriaId,
                        ContaId = model.ContaSaidaId
                    };

                    // 2. Lançamento de Entrada
                    var lancamentoEntrada = new Lancamento
                    {
                        Descricao = $"{model.Descricao} (Entrada)",
                        Valor = model.Valor,
                        Tipo = TipoLancamento.Entrada,
                        Data = model.Data,
                        CategoriaId = model.CategoriaId,
                        ContaId = model.ContaEntradaId
                    };

                    _context.Lancamentos.AddRange(lancamentoSaida, lancamentoEntrada);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["MensagemSucesso"] = "Transferência realizada com sucesso!";
                    
                    if (IsAjaxRequest)
                        return Json(new { success = true });
                        
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Erro ao processar a transferência.");
                }
            }

            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.Where(c => c.ParaTransferencia).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", model.CategoriaId);
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            
            if (IsAjaxRequest)
                return PartialView(model);
                
            return View(model);
        }

        public async Task<IActionResult> GetLancamentosPorCategoria(int mes, int ano, int categoriaId)
        {
            var primeiroDiaMes = new DateTime(ano, mes, 1);
            var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

            var lancamentos = await _context.Lancamentos
                .Include(l => l.Conta)
                .Include(l => l.Categoria)
                .Where(l => l.CategoriaId == categoriaId && l.Data >= primeiroDiaMes && l.Data <= ultimoDiaMes)
                .OrderByDescending(l => l.Data)
                .ToListAsync();

            var viewModel = new LancamentosAgrupadosViewModel
            {
                CategoriaNome = (await _context.Categorias.FindAsync(categoriaId))?.Nome,
                TotalGeral = lancamentos.Sum(l => l.Valor),
                GruposPorConta = lancamentos
                    .GroupBy(l => l.Conta?.Nome ?? "Sem Conta")
                    .Select(g => new GrupoContaViewModel
                    {
                        ContaNome = g.Key,
                        TotalConta = g.Sum(l => l.Valor),
                        Lancamentos = g.ToList()
                    })
                    .OrderBy(x => x.ContaNome)
                    .ToList()
            };

            return PartialView("_LancamentosPorCategoria", viewModel);
        }

        public async Task<IActionResult> ImportarExtrato()
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            ViewBag.Contas = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            
            if (IsAjaxRequest)
                return PartialView();
                
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarLancamentosImportados([FromBody] List<Lancamento> lancamentos)
        {
            if (lancamentos == null || !lancamentos.Any())
            {
                return BadRequest("Nenhum lançamento para salvar.");
            }

            try
            {
                foreach (var lancamento in lancamentos)
                {
                    lancamento.Id = 0;
                    _context.Lancamentos.Add(lancamento);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"{lancamentos.Count} lançamentos salvos com sucesso!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao salvar lançamentos: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDescricoes(string term)
        {
            var query = _context.Lancamentos.AsQueryable();
            
            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(l => l.Descricao.Contains(term));
            }

            var descricoes = await query
                .Select(l => l.Descricao)
                .Distinct()
                .OrderBy(d => d)
                .Take(1000)
                .ToListAsync();

            return Json(descricoes);
        }

        private bool LancamentoExists(int id)
        {
            return _context.Lancamentos.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificarDuplicados([FromBody] VerificarDuplicadosRequest request)
        {
            if (request == null || request.Lancamentos == null || !request.Lancamentos.Any())
            {
                return Json(new { success = true, resultados = new List<object>() });
            }

            var datas = request.Lancamentos.Select(l => l.Data.Date).Distinct().ToList();
            if (!datas.Any())
            {
                return Json(new { success = true, resultados = new List<object>() });
            }

            var minDate = datas.Min();
            var maxDate = datas.Max();

            var existentes = await _context.Lancamentos
                .Where(l => l.ContaId == request.ContaId && l.Data >= minDate && l.Data <= maxDate)
                .Select(l => new { l.Id, l.Data, l.Valor, l.Tipo, l.Descricao })
                .ToListAsync();

            var matchedIds = new HashSet<int>();
            var resultados = new List<object>();

            foreach (var item in request.Lancamentos)
            {
                var match = existentes.FirstOrDefault(e => 
                    e.Data.Date == item.Data.Date && 
                    e.Valor == item.Valor && 
                    (int)e.Tipo == item.Tipo &&
                    !matchedIds.Contains(e.Id));

                if (match != null)
                {
                    matchedIds.Add(match.Id);
                    resultados.Add(new { status = "duplicado", dbId = match.Id, descricaoDb = match.Descricao });
                }
                else
                {
                    resultados.Add(new { status = "novo" });
                }
            }

            return Json(new { success = true, resultados });
        }

        public async Task<IActionResult> CompararExtrato()
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            ViewBag.Contas = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            
            if (IsAjaxRequest)
                return PartialView();
                
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessarComparacaoExtrato([FromBody] CompararExtratoRequest request)
        {
            if (request == null || request.Lancamentos == null || !request.Lancamentos.Any())
            {
                return Json(new { success = true, conciliados = new List<object>(), apenasExtrato = new List<object>(), apenasSistema = new List<object>(), totalExtrato = 0, totalSistemaNoPeriodo = 0 });
            }

            var datas = request.Lancamentos.Select(l => l.Data.Date).Distinct().ToList();
            if (!datas.Any())
            {
                return Json(new { success = true, conciliados = new List<object>(), apenasExtrato = new List<object>(), apenasSistema = new List<object>(), totalExtrato = 0, totalSistemaNoPeriodo = 0 });
            }

            var minDate = datas.Min();
            var maxDate = datas.Max();

            var lancamentosDb = await _context.Lancamentos
                .Include(l => l.Categoria)
                .Include(l => l.Conta)
                .Where(l => l.ContaId == request.ContaId && l.Data.Date >= minDate && l.Data.Date <= maxDate)
                .OrderBy(l => l.Data)
                .ToListAsync();

            var matchedDbIds = new HashSet<int>();
            var conciliados = new List<object>();
            var apenasExtrato = new List<object>();

            foreach (var item in request.Lancamentos)
            {
                var match = lancamentosDb.FirstOrDefault(e => 
                    !matchedDbIds.Contains(e.Id) &&
                    e.Data.Date == item.Data.Date && 
                    e.Valor == item.Valor && 
                    (int)e.Tipo == item.Tipo);

                if (match != null)
                {
                    matchedDbIds.Add(match.Id);
                    conciliados.Add(new {
                        extrato = new {
                            data = item.Data.ToString("yyyy-MM-dd"),
                            descricao = item.Descricao,
                            valor = item.Valor,
                            tipo = item.Tipo
                        },
                        sistema = new {
                            id = match.Id,
                            data = match.Data.ToString("yyyy-MM-dd"),
                            descricao = match.Descricao,
                            valor = match.Valor,
                            tipo = (int)match.Tipo,
                            categoriaNome = match.Categoria?.Nome ?? "Sem Categoria"
                        }
                    });
                }
                else
                {
                    apenasExtrato.Add(new {
                        data = item.Data.ToString("yyyy-MM-dd"),
                        descricao = item.Descricao,
                        valor = item.Valor,
                        tipo = item.Tipo
                    });
                }
            }

            var apenasSistema = lancamentosDb
                .Where(e => !matchedDbIds.Contains(e.Id))
                .Select(e => new {
                    id = e.Id,
                    data = e.Data.ToString("yyyy-MM-dd"),
                    descricao = e.Descricao,
                    valor = e.Valor,
                    tipo = (int)e.Tipo,
                    categoriaNome = e.Categoria?.Nome ?? "Sem Categoria"
                })
                .ToList();

            return Json(new {
                success = true,
                conciliados,
                apenasExtrato,
                apenasSistema,
                totalExtrato = request.Lancamentos.Count,
                totalSistemaNoPeriodo = lancamentosDb.Count
            });
        }
    }

    public class VerificarDuplicadosRequest
    {
        public int ContaId { get; set; }
        public List<LancamentoImportacaoDto> Lancamentos { get; set; } = new();
    }

    public class LancamentoImportacaoDto
    {
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public int Tipo { get; set; }
    }

    public class CompararExtratoRequest
    {
        public int ContaId { get; set; }
        public List<LancamentoComparacaoItemDto> Lancamentos { get; set; } = new();
    }

    public class LancamentoComparacaoItemDto
    {
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public int Tipo { get; set; }
        public string Descricao { get; set; }
    }
}

