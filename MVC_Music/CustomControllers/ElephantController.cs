using Microsoft.AspNetCore.Mvc.Filters;
using MVC_Music.Utilities;
using System.Reflection.Metadata;

namespace MVC_Music.CustomControllers
{
    // Derived controller that extends CognizantController and adds URL maintenance functionality
    public class ElephantController : CognizantController
    {
        // List of actions that should maintain a return URL
        internal string[] ActionWithURL = new string[] { "Details", "Create", "Edit", "Delete", "Add", "Update", "Remove" };

        // Method executed before the action method is called
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // If the current action is in ActionWithURL, maintain the return URL
            if (ActionWithURL.Contains(ActionName()))
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
            }
            base.OnActionExecuting(context);
        }

        // Asynchronous method executed before the action method is called
        public override Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // If the current action is in ActionWithURL, maintain the return URL
            if (ActionWithURL.Contains(ActionName()))
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
