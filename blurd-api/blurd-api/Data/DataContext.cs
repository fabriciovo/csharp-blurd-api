using blurd_api.Models;
using Microsoft.EntityFrameworkCore;

namespace blurd_api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base (options) { }
        public DbSet<User> users => Set<User>();
    }
