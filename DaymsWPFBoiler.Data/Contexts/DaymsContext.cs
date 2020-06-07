using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Models.DTOs;
using DaymsWPFBoiler.Data.Utilities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DaymsWPFBoiler.Data.Contexts
{
    public class DaymsContext : DbContext
    {
        private SqliteConnection connection;

        public DaymsContext()
        {
            
        }

        public DaymsContext(DbContextOptions<DaymsContext> options)
            : base(options)
        {
        }

        #region DbSet & DbQuery Props

        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //string passStr = LicenseHandler.GenerateUID();
            string passStr = "DCM2500$$";
            string connStr = Path.Combine(
                path1: Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
                path2: "DaymLLC", 
                path3: "WPFBoiler.db"
            );

            connection = DBInitializer.CreateDBConnection(connStr, passStr);

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif

            optionsBuilder.UseSqlite(connection);
            SQLitePCL.Batteries_V2.Init();
        }

        public override void Dispose()
        {
            base.Dispose();

            // TODO: Not Needed?
            // connection/*?*/.Close();

            connection.Dispose();
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                if (entry.Entity.GetType().GetProperty("Id") is PropertyInfo p_id &&
                    (null == p_id.GetValue(entry.Entity) || !Guid.TryParse(p_id.GetValue(entry.Entity).ToString(), out Guid resultGuid) || resultGuid == Guid.Empty))
                {
                    p_id.SetValue(entry.Entity, DataFunctions.NewGuidComb());
                }
            }

            ChangeTracker.AutoDetectChangesEnabled = false;
            int result = base.SaveChanges();
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            BoolToStringConverter boolConverter = new BoolToStringConverter("false", "true");
            DateTimeToStringConverter dateTimeConverter = new DateTimeToStringConverter();


        }

        public SqliteConnection GetConnection()
        {
            return connection;
        }

    }
}
