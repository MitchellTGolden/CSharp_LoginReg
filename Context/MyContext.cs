using Microsoft.EntityFrameworkCore;
using LoginReg.Models;

namespace LoginReg.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}