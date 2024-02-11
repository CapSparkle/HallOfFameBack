using Microsoft.EntityFrameworkCore;
using HallOfFame.Models;
using HallOfFame.Entities;


namespace HallOfFame.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PersonEntity> Persons { get; set; }
        public DbSet<SkillEntity> Skills { get; set; }

        public DbSet<PersonSkill> PersonSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PersonSkill>()
                .HasKey(ps => new { ps.PersonId, ps.SkillId });

            modelBuilder.Entity<PersonSkill>()
                .HasOne(ps => ps.Person)
                .WithMany(p => p.PersonSkills)
                .HasForeignKey(ps => ps.PersonId);

            modelBuilder.Entity<PersonSkill>()
                .HasOne(ps => ps.Skill)
                .WithMany(s => s.PersonSkills)
                .HasForeignKey(ps => ps.SkillId);
        }
    }
}
