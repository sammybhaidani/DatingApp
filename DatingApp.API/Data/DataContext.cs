using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    //use DbContext to send a query via entityframwork to my database
    //entity framework will return a result of that query to the controller
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}


     public DbSet<Value> Values { get; set; }

    }
}