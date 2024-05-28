using Microsoft.AspNetCore.Mvc;

namespace MVC_Music.CustomControllers
{
    // Base controller providing common functionality for other controllers
    public class CognizantController : Controller
    {
        // Retrieves the name of the current controller
        internal string ControllerName()
        {
            return ControllerContext.RouteData.Values["controller"].ToString();
        }

        // Retrieves the name of the current action method
        internal string ActionName()
        {
            return ControllerContext.RouteData.Values["action"].ToString();
        }
    }
}
