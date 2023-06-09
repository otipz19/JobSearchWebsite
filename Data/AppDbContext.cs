﻿using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Jobseeker> Jobseekers { get; set; }
		public DbSet<Company> Companies { get; set; }

		public DbSet<Resume> Resumes { get; set; }
        public DbSet<Vacancie> Vacancies { get; set; }

        public DbSet<JobOffer> JobOffers { get; set; }
        public DbSet<VacancieRespond> VacancieResponds { get; set; }

        public DbSet<City> Cities { get; set; }
        public DbSet<EnglishLevel> EnglishLevels { get; set; }
        public DbSet<ExperienceLevel> ExperienceLevels { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Remoteness> Remotenesses { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Sphere> Spheres { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
