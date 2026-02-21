using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorContas.Web.Data;
using GestorContas.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GestorContas.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAjaxRequest => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias
                .OrderBy(c => c.Nome)
                .ToListAsync();
            
            return View(categorias);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            if (IsAjaxRequest)
                return PartialView();

            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,ParaTransferencia")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Categoria cadastrada com sucesso!";
                
                if (IsAjaxRequest)
                    return Json(new { success = true });

                return RedirectToAction(nameof(Index));
            }
            
            if (IsAjaxRequest)
                return PartialView(categoria);

            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            if (IsAjaxRequest)
                return PartialView(categoria);

            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,ParaTransferencia")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["MensagemSucesso"] = "Categoria atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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
                return PartialView(categoria);

            return View(categoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .Include(c => c.Lancamentos)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (categoria == null)
            {
                return NotFound();
            }

            if (IsAjaxRequest)
                return PartialView(categoria);

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Lancamentos)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (categoria != null)
            {
                // Verificar se a categoria possui lançamentos
                if (categoria.Lancamentos != null && categoria.Lancamentos.Any())
                {
                    if (IsAjaxRequest)
                        return BadRequest("Não é possível excluir esta categoria pois existem lançamentos associados a ela.");

                    TempData["MensagemErro"] = "Não é possível excluir esta categoria pois existem lançamentos associados a ela.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Categoria excluída com sucesso!";
            }

            if (IsAjaxRequest)
                return Json(new { success = true });

            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}
