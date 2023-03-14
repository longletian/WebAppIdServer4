using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppIdServer4.Model.Entity;

namespace WebAppIdServer4.Data
{
    public class IdentityDataContext : IdentityDbContext<UserEntity>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options):base(options)
        { 
        

        }
    }
}
