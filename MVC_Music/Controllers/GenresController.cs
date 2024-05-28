using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Music.Data;
using MVC_Music.Models;

namespace MVC_Music.Controllers
{
    [Authorize] // Ensures all actions in this controller require authorization
    public class GenresController : Controller
    {
        private readonly MusicContext _context;

        public GenresController(MusicContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres
                .AsNoTracking()
                .ToListAsync()); // Returns the list of genres
        }

        // GET: Genres/Create
        [Authorize(Roles = "Admin,Supervisor")] // Role-based authorization
        public IActionResult Create()
        {
            return View(); // Returns the create genre view
        }

        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Create([Bind("ID,Name")] Genre genre)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(genre);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Redirects to the index on successful creation
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to create the Genre. Try again, and if the problem persists see your system administrator.");
            }

            return View(genre); // Returns the create genre view with model errors if any
        }

        // GET: Genres/Edit/5
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Genres == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre); // Returns the edit genre view
        }

        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Edit(int id)
        {
            var genreToUpdate = await _context.Genres.FindAsync(id);

            if (genreToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Genre>(genreToUpdate, "", d => d.Name))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Redirects to the index on successful edit
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genreToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes to the Genre. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(genreToUpdate); // Returns the edit genre view with model errors if any
        }

        // GET: Genres/Delete/5
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Genres == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre); // Returns the delete genre view
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Genres == null)
            {
                return Problem("Entity set 'Genres' is null.");
            }
            var genre = await _context.Genres.FindAsync(id);
            try
            {
                if (genre != null)
                {
                    _context.Genres.Remove(genre);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirects to the index on successful deletion
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Genre. Remember, you cannot delete a Genre specified for either an Album or Song.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(genre); // Returns the delete genre view with model errors if any
        }

        private bool GenreExists(int id)
        {
            // Checks if a genre exists by ID
            return _context.Genres.Any(e => e.ID == id);
        }
    }
}
