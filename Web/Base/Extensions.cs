using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Web.Base
{
    public static class Extensions
    {
        public static void AddErrors(this ModelStateDictionary modelState, DbEntityValidationException exception)
        {
            foreach(DbValidationError error in exception.EntityValidationErrors.SelectMany(evr => evr.ValidationErrors))
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        public static string ToDisplayString(this string value)
        {
            StringBuilder builder = new StringBuilder();

            foreach(char bit in value)
            {
                if (char.IsUpper(bit))
                {
                    builder.Append(" ");
                }

                builder.Append(bit);
            }

            return builder.ToString().Trim();
        }
    }
}