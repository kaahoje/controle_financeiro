using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorContas.Web.Data;
using GestorContas.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GestorContas.Web.Controllers
{
    public class ContasController : Controller
    {
        private readonly AppDbContext _context;

        public ContasController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAjaxRequest => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // GET: Contas
        public async Task<IActionResult> Index()
        {
            var contas = await _context.Contas
                .OrderBy(c => c.Nome)
                .ToListAsync();
            
            return View(contas);
        }

        // GET: Contas/Create
        public IActionResult Create()
        {
            if (IsAjaxRequest)
                return PartialView();

            return View();
        }

        // POST: Contas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,SaldoInicial,Ativa,ConsiderarNoDiagnostico")] Conta conta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conta);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Conta cadastrada com sucesso!";
                
                if (IsAjaxRequest)
                    return Json(new { success = true });

                return RedirectToAction(nameof(Index));
            }

            if (IsAjaxRequest)
                return PartialView(conta);

            return View(conta);
        }

        // GET: Contas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conta = await _context.Contas.FindAsync(id);
            if (conta == null)
            {
                return NotFound();
            }

            if (IsAjaxRequest)
                return PartialView(conta);

            return View(conta);
        }

        // POST: Contas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,SaldoInicial,Ativa,ConsiderarNoDiagnostico")] Conta conta)
        {
            if (id != conta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conta);
                    await _context.SaveChangesAsync();
                    TempData["MensagemSucesso"] = "Conta atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContaExists(conta.Id))
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

            if (IsAjaxRequest)
                return PartialView(conta);

            return View(conta);
        }

        // GET: Contas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conta = await _context.Contas
                .Include(c => c.Lancamentos)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (conta == null)
            {
                return NotFound();
            }

            if (IsAjaxRequest)
                return PartialView(conta);

            return View(conta);
        }

        // POST: Contas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conta = await _context.Contas
                .Include(c => c.Lancamentos)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (conta != null)
            {
                // Verificar se a conta possui lançamentos
                if (conta.Lancamentos != null && conta.Lancamentos.Any())
                {
                    if (IsAjaxRequest)
                        return BadRequest("Não é possível excluir esta conta pois existem lançamentos associados a ela.");

                    TempData["MensagemErro"] = "Não é possível excluir esta conta pois existem lançamentos associados a ela.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Contas.Remove(conta);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Conta excluída com sucesso!";
            }

            if (IsAjaxRequest)
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }

        private bool ContaExists(int id)
        {
            return _context.Contas.Any(e => e.Id == id);
        }
    }
}
