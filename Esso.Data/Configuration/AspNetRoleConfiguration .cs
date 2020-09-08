using Esso.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Configuration
{
    public class AspNetRoleConfiguration : EntityTypeConfiguration<AspNetRole>
    {
        public AspNetRoleConfiguration()
        {
            ToTable("AspNetRole");
            Property(c => c.Id).IsRequired();
            Property(c => c.Name).HasMaxLength(256);
        }
    }
}
