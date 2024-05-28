namespace MVC_Music.Utilities
{
    public static class MaintainURL
    {
        // Returns the appropriate URL based on the current context and controller name
        public static string ReturnURL(HttpContext httpContext, string ControllerName)
        {
            string cookieName = ControllerName + "URL";
            string SearchText = "/" + ControllerName + "?";
            string returnURL = httpContext.Request.Headers["Referer"].ToString();

            // If the Referer URL contains the search text, update and store the return URL in a cookie
            if (returnURL.Contains(SearchText))
            {
                returnURL = returnURL[returnURL.LastIndexOf(SearchText)..];
                CookieHelper.CookieSet(httpContext, cookieName, returnURL, 30);
                return returnURL;
            }
            else
            {
                // If not, retrieve the return URL from the cookie
                returnURL = httpContext.Request.Cookies[cookieName];
                return returnURL ?? "/" + ControllerName;
            }
        }
    }
}
