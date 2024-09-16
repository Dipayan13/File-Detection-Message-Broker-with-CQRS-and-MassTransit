using DocumentListenerService.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentListenerService.Data
{
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) { }

        public DbSet<DocumentRecord> documents { get; set; }

        // Set the default schema to 'public' or another schema name
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public"); // This is the default schema in PostgreSQL
            base.OnModelCreating(modelBuilder);
        }
    }
}
