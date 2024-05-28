using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Music.Data;
using MVC_Music.Models;
using MVC_Music.Utilities;

namespace MVC_Music.Controllers
{
    [Authorize] // Ensures all actions in this controller require authorization
    public class AlbumSongsController : Controller
    {
        private readonly MusicContext _context;

        public AlbumSongsController(MusicContext context)
        {
            _context = context;
        }

        // GET: Songs
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Albums"); // Redirects to the Albums index
        }

        [Authorize(Roles = "Admin,Supervisor,Staff")] // Role-based authorization
        public PartialViewResult CreateSong(int? GenreID, int? AlbumID)
        {
            // Populates the Genre and Album dropdown lists
            ViewData["GenreID"] = new SelectList(_context.Genres.OrderBy(a => a.Name), "ID", "Name", GenreID.GetValueOrDefault());
            ViewData["AlbumID"] = AlbumID.GetValueOrDefault();
            return PartialView("_CreateSong");
        }

        // POST: Songs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Create([Bind("Title,DateRecorded,AlbumID,GenreID")] Song song)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(song);
                    await _context.SaveChangesAsync();
                    return Ok(); // Returns Ok on successful creation
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("Title", "Remember, you cannot have duplicate Song Titles on the same Album");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return BadRequest(BuildMessages.ErrorMessage(ModelState)); // Returns error message if model state is invalid
        }

        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<PartialViewResult> EditSong(int ID)
        {
            // Fetches the song to edit including its genre
            Song song = await _context.Songs.Include(p => p.Genre).Where(p => p.ID == ID).FirstOrDefaultAsync();
            ViewData["GenreID"] = new SelectList(_context.Genres.OrderBy(a => a.Name), "ID", "Name", song.GenreID);
            return PartialView("_EditSong", song);
        }

        // POST: Songs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Edit(int id, Byte[] RowVersion)
        {
            var songToUpdate = await _context.Songs.FirstOrDefaultAsync(m => m.ID == id);

            if (songToUpdate == null)
            {
                return NotFound("Unable to get the data. The Song was deleted by another user.");
            }

            _context.Entry(songToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            if (await TryUpdateModelAsync<Song>(songToUpdate, "", p => p.Title, p => p.DateRecorded, p => p.AlbumID, p => p.GenreID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(); // Returns Ok on successful edit
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Song)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("", "Unable to save changes. The Song was deleted by another user.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you received your values. The edit operation was canceled and the current values in the database are listed below.");
                        var databaseValues = (Song)databaseEntry.ToObject();
                        if (databaseValues.Title != clientValues.Title)
                            ModelState.AddModelError("Title", "Title: Current value: " + databaseValues.Title);
                        if (databaseValues.DateRecorded != clientValues.DateRecorded)
                            ModelState.AddModelError("DateRecorded", "Date Recorded: Current value: " + String.Format("{0:d}", databaseValues.DateRecorded));
                        if (databaseValues.GenreID != clientValues.GenreID)
                        {
                            Genre databaseGenre = await _context.Genres.FirstOrDefaultAsync(i => i.ID == databaseValues.GenreID);
                            ModelState.AddModelError("GenreID", $"Genre: Current value: {databaseGenre?.Name}");
                        }
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("Title", "Remember, you cannot have duplicate Song Titles on the same Album");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }

            return BadRequest(BuildMessages.ErrorMessage(ModelState)); // Returns error message if model state is invalid
        }

        [Authorize(Roles = "Admin")]
        public async Task<PartialViewResult> DeleteSong(int ID)
        {
            // Fetches the song to delete including its genre
            Song song = await _context.Songs.Include(p => p.Genre).Where(p => p.ID == ID).FirstOrDefaultAsync();
            return PartialView("_DeleteSong", song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int ID)
        {
            var song = await _context.Songs.FindAsync(ID);
            if (song == null)
            {
                return NotFound("Unable to get the data. The Song has already been deleted by another user.");
            }
            try
            {
                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();
                return Ok(); // Returns Ok on successful deletion
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return BadRequest(BuildMessages.ErrorMessage(ModelState)); // Returns error message if model state is invalid
        }

        private bool SongExists(int id)
        {
            // Checks if a song exists by ID
            return _context.Songs.Any(e => e.ID == id);
        }
    }
}
