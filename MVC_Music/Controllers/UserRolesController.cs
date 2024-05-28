using MVC_Music.Data;
using MVC_Music.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_Music.Controllers
{
    [Authorize(Roles = "Security")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRolesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context; // Injects the application context into the controller
            _userManager = userManager; // Injects the user manager into the controller
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            // Retrieves all users and their roles, and passes them to the view
            var users = await (from u in _context.Users
                               .OrderBy(u => u.UserName)
                               select new UserVM
                               {
                                   Id = u.Id,
                                   UserName = u.UserName
                               }).ToListAsync();
            foreach (var u in users)
            {
                var user = await _userManager.FindByIdAsync(u.Id);
                u.UserRoles = (List<string>)await _userManager.GetRolesAsync(user); // Fetches user roles
            };
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult(); // Returns a bad request if id is null
            }
            var _user = await _userManager.FindByIdAsync(id);
            if (_user == null)
            {
                return NotFound(); // Returns not found if the user is not found
            }
            UserVM user = new UserVM
            {
                Id = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user) // Fetches user roles
            };
            PopulateAssignedRoleData(user); // Populates role data for the view
            if (user.UserName == User.Identity.Name)
            {
                ModelState.AddModelError("", "You cannot change your own role assignments."); // Prevents user from editing their own roles
                ViewData["NoSubmit"] = "disabled=disabled";
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, string[] selectedRoles)
        {
            var _user = await _userManager.FindByIdAsync(Id);
            UserVM user = new UserVM
            {
                Id = _user.Id,
                UserName = _user.UserName,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(_user) // Fetches user roles
            };
            if (user.UserName == User.Identity.Name)
            {
                ModelState.AddModelError("", "You cannot change your own role assignments."); // Prevents user from editing their own roles
                ViewData["NoSubmit"] = "disabled=disabled";
            }
            else
            {
                try
                {
                    await UpdateUserRoles(selectedRoles, user); // Updates user roles
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes."); // Handles exceptions
                }
            }

            PopulateAssignedRoleData(user); // Populates role data for the view
            return View(user);
        }

        private void PopulateAssignedRoleData(UserVM user)
        {
            var allRoles = _context.Roles;
            var currentRoles = user.UserRoles;
            var viewModel = new List<RoleVM>();
            foreach (var r in allRoles)
            {
                viewModel.Add(new RoleVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Assigned = currentRoles.Contains(r.Name) // Checks if the role is assigned to the user
                });
            }
            ViewBag.Roles = viewModel;
        }

        private async Task UpdateUserRoles(string[] selectedRoles, UserVM userToUpdate)
        {
            var UserRoles = userToUpdate.UserRoles;
            var _user = await _userManager.FindByIdAsync(userToUpdate.Id);

            if (selectedRoles == null)
            {
                foreach (var r in UserRoles)
                {
                    await _userManager.RemoveFromRoleAsync(_user, r); // Removes all roles if none are selected
                }
            }
            else
            {
                IList<IdentityRole> allRoles = _context.Roles.ToList<IdentityRole>();

                foreach (var r in allRoles)
                {
                    if (selectedRoles.Contains(r.Name))
                    {
                        if (!UserRoles.Contains(r.Name))
                        {
                            await _userManager.AddToRoleAsync(_user, r.Name); // Adds the role if it is selected and not already assigned
                        }
                    }
                    else
                    {
                        if (UserRoles.Contains(r.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(_user, r.Name); // Removes the role if it is not selected but currently assigned
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose(); // Disposes the context when done
                _userManager.Dispose(); // Disposes the user manager when done
            }
            base.Dispose(disposing);
        }
    }
}
