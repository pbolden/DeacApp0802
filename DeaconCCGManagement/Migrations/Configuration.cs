namespace DeaconCCGManagement.Migrations
{
    using DeaconCCGManagement.DAL;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DeaconCCGManagement.DAL.CcgDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CcgDbContext context)
        {

            new DatabaseSeeder().SeedDatabase(context);

            //new DatabaseSeederProduction().SeedDatabase(context);

         
        }
    }
}
