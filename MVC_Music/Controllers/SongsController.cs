using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
    [Authorize] 
    public class SongsController : CustomControllers.ElephantController
    {
        private readonly MusicContext _context;

        public SongsController(MusicContext context)
        {
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index(string SearchTitle, int? GenreID, int? AlbumID,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Title")
        {
            // Clear any previously stored URL in cookies
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            // Populate dropdown lists for filtering
            PopulateDropDownLists();

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Title", "Date Recorded", "Album", "Genre" };

            // Get songs from the database including related Album, Genre, and SongDetail entities
            var songs = from s in _context.Songs
                        .Include(s => s.Album)
                        .Include(s => s.Genre)
                        .Include(s => s.SongDetail)
                        select s;

            // Filtering logic
            if (GenreID.HasValue)
            {
                songs = songs.Where(p => p.GenreID == GenreID);
                ViewData["Filtering"] = " show";
            }
            if (AlbumID.HasValue)
            {
                songs = songs.Where(p => p.AlbumID == AlbumID);
                ViewData["Filtering"] = " show";
            }
            if (!String.IsNullOrEmpty(SearchTitle))
            {
                songs = songs.Where(p => p.Title.ToUpper().Contains(SearchTitle.ToUpper()));
                ViewData["Filtering"] = " show";
            }

            // Sorting logic
            if (!String.IsNullOrEmpty(actionButton))
            {
                page = 1; // Reset page to start

                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField)
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;
                }
            }

            // Apply sorting based on sortField and sortDirection
            if (sortField == "Date Recorded")
            {
                if (sortDirection == "asc")
                {
                    songs = songs.OrderByDescending(p => p.DateRecorded).ThenBy(p => p.Title);
                }
                else
                {
                    songs = songs.OrderBy(p => p.DateRecorded).ThenBy(p => p.Title);
                }
            }
            else if (sortField == "Album")
            {
                if (sortDirection == "asc")
                {
                    songs = songs.OrderBy(p => p.Album.Name).ThenBy(p => p.Title);
                }
                else
                {
                    songs = songs.OrderByDescending(p => p.Album.Name).ThenBy(p => p.Title);
                }
            }
            else if (sortField == "Genre")
            {
                if (sortDirection == "asc")
                {
                    songs = songs.OrderBy(p => p.Genre.Name).ThenBy(p => p.Title);
                }
                else
                {
                    songs = songs.OrderByDescending(p => p.Genre.Name).ThenBy(p => p.Title);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    songs = songs.OrderBy(p => p.Title);
                }
                else
                {
                    songs = songs.OrderByDescending(p => p.Title);
                }
            }
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            // Pagination logic
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Song>.CreateAsync(songs.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Songs/Details/5
        [Authorize(Roles = "Admin,Supervisor,Staff")] // Restrict access to specified roles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Album)
                .Include(s => s.Genre)
                .Include(s => s.SongDetail)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        public PartialViewResult ShowSongDetails(int id)
        {
            // Show song details in a partial view
            ViewBag.SongDetail = _context.SongDetail
                .Where(s => s.SongID == id)
                .FirstOrDefault();
            return PartialView("_SongDetail");
        }

        // GET: Songs/Create
        [Authorize(Roles = "Admin,Supervisor,Staff")] // Restrict access to specified roles
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Songs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor,Staff")] // Restrict access to specified roles
        public async Task<IActionResult> Create([Bind("ID,Title,DateRecorded,AlbumID,GenreID")] Song song)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(song);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    ModelState.AddModelError("Title", "Unable to save changes. Remember, you cannot have duplicate Song Titles on the same Album.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes to the Song. Try again, and if the problem persists see your system administrator.");
                }
            }

            PopulateDropDownLists(song);
            return View(song);
        }

        // GET: Songs/Edit/5
        [Authorize(Roles = "Admin,Supervisor,Staff")] // Restrict access to specified roles
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(song);
            return View(song);
        }

        // POST: Songs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor,Staff")] // Restrict access to specified roles
        public async Task<IActionResult> Edit(int id, Byte[] RowVersion)
        {
            var songToUpdate = await _context.Songs
                .FirstOrDefaultAsync(m => m.ID == id);

            if (songToUpdate == null)
            {
                return NotFound();
            }

            _context.Entry(songToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            if (await TryUpdateModelAsync<Song>(songToUpdate, "",
                p => p.Title, p => p.DateRecorded, p => p.AlbumID, p => p.GenreID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Song)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Song was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Song)databaseEntry.ToObject();
                        if (databaseValues.Title != clientValues.Title)
                            ModelState.AddModelError("Title", "Current value: "
                                + databaseValues.Title);
                        if (databaseValues.DateRecorded != clientValues.DateRecorded)
                            ModelState.AddModelError("DateRecorded", "Current value: "
                                + String.Format("{0:d}", databaseValues.DateRecorded));
                        if (databaseValues.GenreID != clientValues.GenreID)
                        {
                            Genre databaseGenre = await _context.Genres.FirstOrDefaultAsync(i => i.ID == databaseValues.GenreID);
                            ModelState.AddModelError("GenreID", $"Current value: {databaseGenre?.Name}");
                        }
                        if (databaseValues.AlbumID != clientValues.AlbumID)
                        {
                            Album databaseAlbum = await _context.Albums.FirstOrDefaultAsync(i => i.ID == databaseValues.AlbumID);
                            ModelState.AddModelError("AlbumID", $"Current value: {databaseAlbum?.Name}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to Song List' hyperlink.");
                        songToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE"))
                    {
                        ModelState.AddModelError("Title", "Unable to save changes. Remember, you cannot have duplicate Song Titles on the same Album.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes to the Song. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }

            PopulateDropDownLists(songToUpdate);
            return View(songToUpdate);
        }

        // GET: Songs/Delete/5
        [Authorize(Roles = "Admin,Supervisor,Staff")] // Restrict access to specified roles
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Album)
                .Include(s => s.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (song == null)
            {
                return NotFound();
            }

            // Ensure Staff can only delete songs they created
            if (User.IsInRole("Staff"))
            {
                if (song.CreatedBy != User.Identity.Name)
                {
                    ModelState.AddModelError("", "You cannot Delete a Song you did not enter into the system.");
                    ViewData["NoSubmit"] = "disabled=disabled";
                }
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor,Staff")] // Restrict access to specified roles
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Songs == null)
            {
                return Problem("Entity set 'Songs' is null.");
            }
            var song = await _context.Songs
                .Include(s => s.Album)
                .Include(s => s.Genre)
                .FirstOrDefaultAsync(m => m.ID == id);

            // Ensure Staff can only delete songs they created
            if (User.IsInRole("Staff"))
            {
                if (song.CreatedBy != User.Identity.Name)
                {
                    ModelState.AddModelError("", "You cannot Delete a Song you did not enter into the system.");
                    ViewData["NoSubmit"] = "disabled=disabled";
                    return View(song);
                }
            }
            try
            {
                if (song != null)
                {
                    _context.Songs.Remove(song);
                }

                await _context.SaveChangesAsync();
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Song Album. Remember, you cannot delete a Song with Performances in the system.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(song);

        }

        private SelectList GenreList(int? selectedId)
        {
            return new SelectList(_context
                .Genres
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }
        private SelectList AlbumList(int? selectedId)
        {
            return new SelectList(_context
                .Albums
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }
        private void PopulateDropDownLists(Song song = null)
        {
            ViewData["GenreID"] = GenreList(song?.GenreID);
            ViewData["AlbumID"] = AlbumList(song?.AlbumID);
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.ID == id);
        }
    }
}
