using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Model;

namespace WebApplication1.DataBase
{
    public class HomeDB : DbContext
    {
        public  HomeDB(DbContextOptions<HomeDB> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<UserAdmin> UsersAdmin { get; set; }
    }

}
