namespace Drupal.ParentPortal.Repository.Context
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class SqlContext : DbContext
    {
        public DbSet<Models.AppManagement> AppManagement { get; set; }
        public DbSet<Models.SchoolEvent> SchoolEvent { get; set; }
        public DbSet<Models.EventVolunteer> EventVolunteer { get; set; }
        public DbSet<Models.Document> Document { get; set; }
        public DbSet<Models.DocumentLibrary> DocumentLibrary { get; set; }
        //public DbSet<Models.Alert> Alert { get; set; }
        //public DbSet<Models.AlertFeed> AlertFeed { get; set; }
        public DbSet<Models.Audience> Audience { get; set; }
        public DbSet<Models.Student> Student { get; set; }
        public DbSet<Models.ConfigurationItem> ConfigurationItem { get; set; }
        

        public SqlContext() : base("DefaultConnection")
        {            
            Database.SetInitializer(new CreateDatabaseIfNotExists<SqlContext>());
            
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #region AppManagement Definition
            modelBuilder.Entity<Models.AppManagement>().HasKey(t => t.AppManagementId);
            #endregion

            #region Event Volunteer Definition
            modelBuilder.Entity<Models.EventVolunteer>().HasKey(t => t.EventVolunteerId);
            #endregion

            #region School Event Definition
            modelBuilder.Entity<Models.SchoolEvent>().HasKey(t => t.SchoolEventId);
            //modelBuilder.Entity<SchoolEvent>().HasOptional<Models.Audience>(u => u.).WithOptionalPrincipal();
            #endregion

            #region Document Definition
            modelBuilder.Entity<Models.Document>().HasKey(t => t.DocumentId);
            #endregion

            #region ConfigurationItem Definition
            modelBuilder.Entity<Models.ConfigurationItem>().HasKey(t => t.ConfigurationItemId);
            #endregion
        }
    }
}