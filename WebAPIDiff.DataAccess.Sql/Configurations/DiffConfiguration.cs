using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIDiff.Domain.Entities;

namespace WebAPIDiff.DataAccess.Sql.Configurations
{
  public class DiffConfiguration : EntityTypeConfiguration<Diff>
  {
    public DiffConfiguration()
    {

      HasKey(p => p.DiffId)
        .Property(p => p.DiffId)
        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

      Property(p => p.LeftData)
        .HasMaxLength(500)
        //.IsFixedLength()
        .IsOptional();

      Property(p => p.RightData)
        .HasMaxLength(500)
        //.IsFixedLength()
        .IsOptional();

      ToTable("Diff");
    }
  }
}
