using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MVC_Music.Utilities
{
    public static class BuildMessages
    {
        // Builds a concatenated error message string from the model state errors
        public static string ErrorMessage(ModelStateDictionary modelState)
        {
            IEnumerable<ModelError> allErrors = modelState.Values.SelectMany(v => v.Errors);
            string errorMessage = "";
            foreach (var e in allErrors)
            {
                errorMessage += e.ErrorMessage + "|";
            }
            return errorMessage;
        }
    }
}
