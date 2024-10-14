using Microsoft.EntityFrameworkCore;

namespace MaxDDDDemo.Core.Data;

public class ApplicationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}