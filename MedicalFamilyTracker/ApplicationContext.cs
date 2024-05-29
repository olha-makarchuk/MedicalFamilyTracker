using MedicalFamilyTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalFamilyTracker
{
	public class ApplicationContext : DbContext
	{
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

		public DbSet<FamilyMember> FamilyMembers { get; set; } = null!;
        public DbSet<SheduleEntity> SheduleEntities { get; set; } = null!;
    }
}
