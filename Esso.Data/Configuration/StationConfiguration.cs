using Esso.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Configuration
{
    public class StationConfiguration : EntityTypeConfiguration<TBL_STATION>
    {
        public StationConfiguration()
        {
            ToTable("TBL_STATION");
            Property(c => c.NAME).IsRequired().HasMaxLength(250);
        }
    }
}
