using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiniAPI.Services
{
    public class IdentityService
    {
       public static async Task<IResult> GetAllTodos(DataContext db)
        {
            return TypedResults.Ok(await db.Users.ToArrayAsync());
        }

    }
}
