using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorContas.Web.Data;
using GestorContas.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GestorContas.Web.Controllers
{
    public class PrefeiturasController : Controller
    {
        private readonly AppDbContext _context;

        public PrefeiturasController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAjaxRequest => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // GET: Prefeituras
        public async Task<IActionResult> Index(int? mes, int? ano, string? busca, bool? tipo)
        {
            var dataAtual = DateTime.Today;
            var mesAtual = mes ?? dataAtual.Month;
            var anoAtual = ano ?? dataAtual.Year;

            ViewBag.MesSelecionado = mesAtual;
            ViewBag.AnoSelecionado = anoAtual;
            ViewBag.Busca = busca;
            ViewBag.Tipo = tipo;
            
            ViewBag.Meses = Enumerable.Range(1, 12)
                .Select(m => new { Id = m, Nome = new DateTime(2000, m, 1).ToString("MMMM") })
                .ToList();
            ViewBag.Anos = Enumerable.Range(DateTime.Today.Year - 10, 15).ToList();

            // Base query for all calculations
            var baseQuery = _context.Prefeituras.AsNoTracking();

            // 1. Totalizadores Cumulativos (Até o fim do mês selecionado)
            var dataCorte = new DateTime(anoAtual, mesAtual, 1).AddMonths(1);
            var queryTotalizador = baseQuery.Where(p => p.VencimentoDaParcela < dataCorte);
            
            ViewBag.TotalRecebimentos = (decimal)await queryTotalizador.Where(p => p.Entrada == true).SumAsync(p => (double)(p.Valor ?? 0));
            ViewBag.TotalPagamentos = (decimal)await queryTotalizador.Where(p => p.Entrada == false).SumAsync(p => (double)(p.Valor ?? 0));
            ViewBag.SaldoMes = (decimal)ViewBag.TotalRecebimentos - (decimal)ViewBag.TotalPagamentos;

            // 2. Query do Grid (Filtrada pelo mês selecionado e filtros de busca)
            var gridQuery = baseQuery.AsQueryable();

            if (mes.HasValue || ano.HasValue || !IsAjaxRequest)
            {
                gridQuery = gridQuery.Where(p => p.VencimentoDaParcela.HasValue && 
                                           p.VencimentoDaParcela.Value.Month == mesAtual && 
                                           p.VencimentoDaParcela.Value.Year == anoAtual);
            }

            if (!string.IsNullOrEmpty(busca))
            {
                gridQuery = gridQuery.Where(p => p.Descricao != null && p.Descricao.Contains(busca));
            }

            if (tipo.HasValue)
            {
                gridQuery = gridQuery.Where(p => p.Entrada == tipo.Value);
            }

            var prefeiturasList = await gridQuery.OrderByDescending(p => p.Entrada).ThenBy(p => p.VencimentoDaParcela).ToListAsync();

            if (IsAjaxRequest)
                return PartialView("_GridPrefeituras", prefeiturasList);

            return View(prefeiturasList);
        }

        // GET: Prefeituras/Create
        public IActionResult Create()
        {
            if (IsAjaxRequest)
                return PartialView();

            return View();
        }

        // POST: Prefeituras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Data,Valor,Entrada,VencimentoDaParcela")] Prefeitura prefeitura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prefeitura);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Registro salvo com sucesso!";
                
                if (IsAjaxRequest)
                    return Json(new { success = true });

                return RedirectToAction(nameof(Index));
            }

            if (IsAjaxRequest)
                return PartialView(prefeitura);

            return View(prefeitura);
        }

        // GET: Prefeituras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var prefeitura = await _context.Prefeituras.FindAsync(id);
            if (prefeitura == null) return NotFound();

            if (IsAjaxRequest)
                return PartialView(prefeitura);

            return View(prefeitura);
        }

        // POST: Prefeituras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Data,Valor,Entrada,VencimentoDaParcela")] Prefeitura prefeitura)
        {
            if (id != prefeitura.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prefeitura);
                    await _context.SaveChangesAsync();
                    TempData["MensagemSucesso"] = "Registro atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrefeituraExists(prefeitura.Id)) return NotFound();
                    else throw;
                }

                if (IsAjaxRequest)
                    return Json(new { success = true });

                return RedirectToAction(nameof(Index));
            }

            if (IsAjaxRequest)
                return PartialView(prefeitura);

            return View(prefeitura);
        }

        // GET: Prefeituras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var prefeitura = await _context.Prefeituras.FindAsync(id);
            if (prefeitura == null) return NotFound();

            if (IsAjaxRequest)
                return PartialView(prefeitura);

            return View(prefeitura);
        }

        // POST: Prefeituras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prefeitura = await _context.Prefeituras.FindAsync(id);
            if (prefeitura != null)
            {
                _context.Prefeituras.Remove(prefeitura);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Registro excluído com sucesso!";
            }

            if (IsAjaxRequest)
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }

        private bool PrefeituraExists(int id)
        {
            return _context.Prefeituras.Any(e => e.Id == id);
        }
    }
}
