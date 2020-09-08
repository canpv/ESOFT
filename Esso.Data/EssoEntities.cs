using Esso.Data.Configuration;
using Esso.Model;
using Esso.Model.Models;
using Esso.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace Esso.Data
{
    public class EssoEntities : DbContext
    {
        public EssoEntities() : base("DefaultConnection") {
            Database.SetInitializer<EssoEntities>(null);
        }
        //public DbSet<TBL_MODBUS_TAG> ModbusTag { get; set; }
        public DbSet<AspNetUsers> Users { get; set; }
        public DbSet<TBL_PUSH_NTF> pushInfs { get; set; }
        public DbSet<TBL_COMPANY> Companies { get; set; }
        public DbSet<TBL_COMPANY_USER> CompanyUsers { get; set; }
        public DbSet<TBL_STATION_USER> StationUsers { get; set; }
        public DbSet<TBL_STATION> Stations { get; set; }
        public DbSet<TBL_INVERTER> Inverters { get; set; }
        public DbSet<TBL_INV_ADDRESS> InvAddresses { get; set; }
        public DbSet<TBL_MODBUS_CMD_LOG> ModbusLog { get; set; }

        public DbSet<TBL_MODBUS_TAG> ModbusTag { get; set; }

        public DbSet<TBL_MODBUS_DATA> ModbusData { get; set; }
        public DbSet<TBL_MODBUS_DATA_LIVE> ModbusDataLive { get; set; }
        public DbSet<TBL_INV_OZET> InvSums { get; set; }
        public DbSet<TBL_OZET> Summaries  { get; set; }
        public DbSet<TBL_OZET_LIVE> SummariesLive { get; set; }
        public DbSet<TBL_PR_OZET> PRSum  { get; set; }

        public DbSet<TBL_STR_OZET> StringOzet { get; set; }
        public DbSet<TBL_STR_OZET_LIVE> StringOzetLive { get; set; }
        public DbSet<TBL_STR_OZET_AVG> StringOzetAVG { get; set; }
        public DbSet<TBL_STR_OZET_QUARTERHOUR_AVG> StringOzetQuarterHourAVG { get; set; }
        public DbSet<TBL_STR_OZET_DAILY> StringOzetDaily { get; set; }
        public DbSet<VW_STR_OZET_DAILY> VWStringOzetDaily { get; set; }
        public DbSet<TBL_TAG_TEMP> TagTemplates { get; set; }
        public DbSet<TBL_TAG_TEMP_DET> TagTemplateDets { get; set; }

        public DbSet<TBL_STATION_GROUP> StationGroups { get; set; }

        public DbSet<TBL_PROVIENCE> Proviences { get; set; }
        public DbSet<TBL_TOWN> Towns { get; set; }

        public DbSet<TBL_TAG> Tags { get; set; }
        public DbSet<TBL_DATA_MAIN> DataMains { get; set; }
        public DbSet<TBL_MONTHLY_TARGET> MonthlyTargets { get; set; }

        public DbSet<TBL_ALARM_DEF> AlarmDefinitions { get; set; }
        public DbSet<TBL_ALARM_TEMP> AlarmTemplate { get; set; }
        public DbSet<TBL_ALARM_LOG> AlarmLogs { get; set; }
        public DbSet<TBL_STATUS_LOG> StatusLogs { get; set; }
        public DbSet<TBL_ALARM_TEMP_DET> AlarmTemplatedet { get; set; }
        public DbSet<TBL_STATION_STRING> stationString { get; set; }
        public DbSet<TBL_DIGITAL_LOG> DigitalLogs { get; set; }
        public DbSet<TBL_DIGITAL_LOG_LIVE> DigitalLogsLive { get; set; }
        public DbSet<TBL_TARGET> targets { get; set; }
        public DbSet<TBL_USER_CHART> UserCharts { get; set; }
		public DbSet<TBL_USER_ENTITY> UserEntity { get; set; }
		public DbSet<TBL_USER_CHART_DETAIL> ChartDetails { get; set; }
		public DbSet<TBL_FTP> FtpList { get; set; }
		public DbSet<TBL_ALARM_STATUS> AlarmStatus { get; set; }
        public DbSet<TBL_ALARM_DESC> AlarmDesc { get; set; }
        public DbSet<TBL_EXCHANGE> exchange { get; set; }
        public DbSet<TBL_STATION_SUMMARY> stationSummary { get; set; }
        public DbSet<TBL_DISTRIBUTION_DATA> Osos { get; set; }
        public DbSet<TBL_UNIT_OZET> UnitSums { get; set; }
        public DbSet<TBL_PANEL_LOCATIONS> PanelLocations { get; set; }
        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.HasDefaultSchema("LOG724DB");

            //modelBuilder.Configurations.Add(new CompanyConfiguration());
            //modelBuilder.Ignore<TBL_STATION>();
            //modelBuilder.Ignore<TBL_STATION_GROUP>();
            //modelBuilder.Ignore<TBL_COMPANY_USER>();
            //modelBuilder.Ignore<TBL_STATION_USER>();

            //modelBuilder.Entity<TBL_COMPANY>().ToTable("TBL_COMPANY");
            //modelBuilder.Entity<TBL_COMPANY_USER>().ToTable("TBL_COMPANY_USER");
            //modelBuilder.Entity<TBL_STATION_USER>().ToTable("TBL_STATION_USER");
            //modelBuilder.Entity<TBL_STATION>().ToTable("TBL_STATION");
            //modelBuilder.Entity<TBL_TAG>().ToTable("TBL_TAG");
            //modelBuilder.Entity<TBL_TAG_TEMP>().ToTable("TBL_TAG_TEMP");
            //modelBuilder.Entity<TBL_TAG_TEMP_DET>().ToTable("TBL_TAG_TEMP_DET");
            //modelBuilder.Entity<TBL_INVERTER>().ToTable("TBL_INVERTER");
            //modelBuilder.Entity<TBL_INV_ADDRESS>().ToTable("TBL_INV_ADDRESS");
            //modelBuilder.Entity<TBL_MONTHLY_TARGET>().ToTable("TBL_MONTHLY_TARGET");


            //modelBuilder.Entity<TBL_INV_OZET>().ToTable("TBL_INV_OZET");
            //modelBuilder.Entity<TBL_OZET>().ToTable("TBL_OZET");
            //modelBuilder.Entity<TBL_PR_OZET>().ToTable("TBL_PR_OZET");

            //modelBuilder.Entity<TBL_STATION_GROUP>().ToTable("TBL_STATION_GROUP");
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = fullErrorMessage;

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

    }
}
