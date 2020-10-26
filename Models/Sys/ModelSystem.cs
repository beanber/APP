namespace APP.Models.Sys
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelSystem : DbContext
    {
        public ModelSystem()
            : base("name=ModelSystem")
        {
        }

        public virtual DbSet<sysCountry> sysCountries { get; set; }
        public virtual DbSet<sysDistrict> sysDistricts { get; set; }
        public virtual DbSet<sysModule> sysModules { get; set; }
        public virtual DbSet<sysPage> sysPages { get; set; }
        public virtual DbSet<sysPerson> sysPersons { get; set; }
        public virtual DbSet<sysPosition> sysPositions { get; set; }
        public virtual DbSet<sysProvince> sysProvinces { get; set; }
        public virtual DbSet<sysSex> sysSexes { get; set; }
        public virtual DbSet<sysTournament> sysTournaments { get; set; }
        public virtual DbSet<sysTournamentLevel> sysTournamentLevels { get; set; }
        public virtual DbSet<sysWard> sysWards { get; set; }
        public virtual DbSet<v_sysModule> v_sysModule { get; set; }
        public virtual DbSet<v_sysPage> v_sysPage { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<sysCountry>()
                .HasMany(e => e.sysPersons)
                .WithOptional(e => e.sysCountry)
                .HasForeignKey(e => e.CountryId);

            modelBuilder.Entity<sysCountry>()
                .HasMany(e => e.sysProvinces)
                .WithOptional(e => e.sysCountry)
                .HasForeignKey(e => e.CountryId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<sysDistrict>()
                .HasMany(e => e.sysWards)
                .WithOptional(e => e.sysDistrict)
                .HasForeignKey(e => e.DistrictId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<sysModule>()
                .HasMany(e => e.sysPages)
                .WithOptional(e => e.sysModule)
                .HasForeignKey(e => e.ModuleId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<sysProvince>()
                .HasMany(e => e.sysDistricts)
                .WithOptional(e => e.sysProvince)
                .HasForeignKey(e => e.ProvinceId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<sysProvince>()
                .HasMany(e => e.sysPersons)
                .WithOptional(e => e.sysProvince)
                .HasForeignKey(e => e.ProvinceId);

            modelBuilder.Entity<sysSex>()
                .HasMany(e => e.sysPersons)
                .WithOptional(e => e.sysSex)
                .HasForeignKey(e => e.SexId);

            modelBuilder.Entity<sysTournament>()
                .Property(e => e.PercentForStaff)
                .HasPrecision(9, 2);

            modelBuilder.Entity<sysTournament>()
                .Property(e => e.PercentForPrize)
                .HasPrecision(9, 2);

            modelBuilder.Entity<sysTournament>()
                .HasMany(e => e.sysTournamentLevels)
                .WithOptional(e => e.sysTournament)
                .HasForeignKey(e => e.TournamentId)
                .WillCascadeOnDelete();
        }
    }
}
