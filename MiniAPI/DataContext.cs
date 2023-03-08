using Microsoft.EntityFrameworkCore;

namespace MiniAPI
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { 
          
        }
        public DbSet<UserEntity> Users { get; set; }
    }

    public class UserEntity 
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string? PassWord { get; set; }

        public string? NickName { get; set; }

    }
}
