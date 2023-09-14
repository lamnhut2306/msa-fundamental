using Microsoft.EntityFrameworkCore;

using MSA.BankService.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;

namespace MSA.BankService.Data
{
    public class BankDbContext : AppDbContextBase
    {
        private readonly string _uuidGenerator = "uuid-ossp";
        private readonly string _uuidAlgorithm = "uuid_generate_v4()";
        private readonly IConfiguration configuration;

        public BankDbContext(
            IConfiguration configuration,
            DbContextOptions<BankDbContext> options) : base(configuration, options)
        {
            this.configuration = configuration;
        }

        public DbSet<Payment> Payments { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension(_uuidGenerator);

            //Product
            modelBuilder.Entity<Payment>().ToTable("payments");
            modelBuilder.Entity<Payment>().HasKey(x => x.Id);
            modelBuilder.Entity<Payment>().Property(x => x.OrderId)
                            .HasColumnType("uuid");

            //Relationship

        }
    }
}
