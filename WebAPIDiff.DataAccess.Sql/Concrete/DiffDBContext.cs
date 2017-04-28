using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIDiff.DataAccess.Sql.Configurations;
using WebAPIDiff.Domain.Entities;

namespace WebAPIDiff.DataAccess.Sql.Concrete
{
  public class DiffDbContext : DbContext
  {
    public DiffDbContext() : base("name=DiffDbContext")
    {
      Configuration.LazyLoadingEnabled = false;
      Configuration.ProxyCreationEnabled = false;
    }

    public DbSet<Diff> Diffs { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      // Entity Type Configuration
      modelBuilder.Configurations.Add(new DiffConfiguration());
    }
  }
}
