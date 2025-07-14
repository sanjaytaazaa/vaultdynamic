using Microsoft.EntityFrameworkCore;
using VaultDynamicDbDemo.Models;

namespace VaultDynamicDbDemo;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Person> People => Set<Person>(); 
    public DbSet<Products> Products => Set<Products>();
}
