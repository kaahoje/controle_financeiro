using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestorContas.Web.Data;
using GestorContas.Web.Models;
using GestorContas.Web.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestorContas.Web.Controllers
{
    public class LancamentosController : Controller
    {
        private readonly AppDbContext _context;

        public LancamentosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Lancamentos
        public async Task<IActionResult> Index(int? mes, int? ano, int? categoriaId, int? contaId, TipoLancamento? tipo, bool? paraTransferencia)
        {
            var dataAtual = DateTime.Today;
            var mesAtual = mes ?? dataAtual.Month;
            var anoAtual = ano ?? dataAtual.Year;

            var primeiroDiaMes = new DateTime(anoAtual, mesAtual, 1);
            var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

            ViewBag.MesSelecionado = mesAtual;
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
                .Where(l => l.Data.Year == anoAtual && l.Data.Month == mesAtual);

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
                .ThenBy(l => l.Descricao).ToListAsync();

            // Calcular totais
            ViewBag.TotalEntradas = lancamentos.Where(l => l.Tipo == TipoLancamento.Entrada).Sum(l => l.Valor);
            ViewBag.TotalSaidas = lancamentos.Where(l => l.Tipo == TipoLancamento.Saida).Sum(l => l.Valor);
            ViewBag.Saldo = ViewBag.TotalEntradas - ViewBag.TotalSaidas;

            return View(lancamentos);
        }

        // GET: Lancamentos/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");
            return View();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.CategoriaId);
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.ContaId);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.CategoriaId);
            ViewData["ContaId"] = new SelectList(await _context.Contas.Where(c => c.Ativa).OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", lancamento.ContaId);
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
            return RedirectToAction(nameof(Index));
        }

        private bool LancamentoExists(int id)
        {
            return _context.Lancamentos.Any(e => e.Id == id);
        }
    }
}
