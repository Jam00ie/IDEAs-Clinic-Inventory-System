using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IdeasClinicInventory.Web.Data;

/// <summary>
/// Translates provider-specific SQL errors into conditions the UI can explain.
/// </summary>
public static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueConstraintViolation(this DbUpdateException exception)
    {
        // SQL Server can wrap SqlException more than once, so walk the complete
        // exception chain instead of assuming a fixed InnerException shape.
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException { Number: 2601 or 2627 })
            {
                return true;
            }
        }

        return false;
    }
}
