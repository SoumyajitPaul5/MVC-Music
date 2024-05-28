using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_Music.Utilities
{
    public static class PageSizeHelper
    {
        // Sets the page size based on the provided value or cookies
        public static int SetPageSize(HttpContext httpContext, int? pageSizeID, string ControllerName)
        {
            int pageSize = 0;

            if (pageSizeID.HasValue)
            {
                // Use provided pageSizeID and store it in cookies
                pageSize = pageSizeID.GetValueOrDefault();
                CookieHelper.CookieSet(httpContext, ControllerName + "pageSizeValue", pageSize.ToString(), 480);
                CookieHelper.CookieSet(httpContext, "DefaultpageSizeValue", pageSize.ToString(), 480);
            }
            else
            {
                // Retrieve pageSize from cookies
                pageSize = Convert.ToInt32(httpContext.Request.Cookies[ControllerName + "pageSizeValue"]);
            }

            // If pageSize is still 0, check the default pageSize cookie
            if (pageSize == 0)
            {
                pageSize = Convert.ToInt32(httpContext.Request.Cookies["DefaultpageSizeValue"]);
            }

            // Return default value of 5 if pageSize is still 0
            return (pageSize == 0) ? 5 : pageSize;
        }

        // Provides a SelectList of page size options
        public static SelectList PageSizeList(int? pageSize)
        {
            return new SelectList(new[] { "3", "5", "10", "20", "30", "40", "50", "100", "500" }, pageSize.ToString());
        }
    }
}
