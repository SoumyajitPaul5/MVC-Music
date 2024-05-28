using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Music.Data;
using MVC_Music.Models;
using MVC_Music.Utilities;

namespace MVC_Music.Controllers
{
    public class SongDetailController : Controller
    {
        private readonly MusicContext _context;

        public SongDetailController(MusicContext context)
        {
            _context = context; // Injects the music context into the controller
        }

        // GET: SongDetails
        public IActionResult Index()
        {
            // Redirects to the index action of the Songs controller
            return RedirectToAction("Index", "Songs");
        }

        // GET: SongDetails/Edit/{ID}
        public async Task<PartialViewResult> Edit(int ID)
        {
            // Retrieves the SongDetail to edit based on the given ID
            var songDetail = await _context.SongDetail
                .Where(s => s.SongID == ID)
                .FirstOrDefaultAsync();

            // Returns the partial view for editing song details
            return PartialView("_SongDetailEdit", songDetail);
        }

        // POST: SongDetails/Edit/{ID}
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int ID)
        {
            // Retrieves the SongDetail to update based on the given ID
            var songDetailToUpdate = await _context.SongDetail
                .FirstOrDefaultAsync(m => m.ID == ID);

            if (songDetailToUpdate == null)
            {
                return NotFound("Song Detail has been deleted."); // Returns a not found response if the song detail is not found
            }

            if (await TryUpdateModelAsync<SongDetail>(songDetailToUpdate, "",
                d => d.Comments, d => d.Length, d => d.Rating))
            {
                try
                {
                    await _context.SaveChangesAsync(); // Saves the changes to the database
                    return Ok(); // Returns an OK response on successful update
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongDetailExists(songDetailToUpdate.ID))
                    {
                        return NotFound("Song Detail has been deleted."); // Returns a not found response if the song detail is not found during concurrency check
                    }
                    else
                    {
                        throw; // Throws the exception if there is a concurrency issue
                    }
                }
                catch (DbUpdateException)
                {
                    // Adds a model state error if there is a database update exception
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            // Returns a bad request response with error messages if the update fails
            return BadRequest(BuildMessages.ErrorMessage(ModelState));
        }

        // Checks if a SongDetail exists based on the given ID
        private bool SongDetailExists(int id)
        {
            return _context.SongDetail.Any(e => e.ID == id);
        }
    }
}
