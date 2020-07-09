using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApiTest.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static Dictionary<string, string> ToDictionary(this ModelStateDictionary modelState)
        {
            return modelState.ToDictionary(kvp => kvp.Key.Replace("model.", ""), kvp => kvp.Value.Errors[0].ErrorMessage);
        }
    }
}
