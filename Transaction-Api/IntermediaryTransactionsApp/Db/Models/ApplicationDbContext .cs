using Microsoft.EntityFrameworkCore;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<OrderStatus> OrderStatuses { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<LoginHistory> LoginHistories { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<TransactionHistory> TransactionHistories { get; set; }
		public DbSet<Role> Roles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
		
			modelBuilder.Entity<OrderStatus>(entity =>
			{
				entity.ToTable("OrderStatus");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.StatusName)
					.IsRequired()
					.HasMaxLength(100);
				entity.Property(e => e.Description)
					.HasMaxLength(255);
			});

		
			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("Users");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Username)
					.IsRequired()
					.HasMaxLength(50);
				entity.Property(e => e.PasswordHash)
					.IsRequired()
					.HasMaxLength(255);
				entity.Property(e => e.Email)
					.IsRequired()
					.HasMaxLength(255);
				entity.Property(e => e.CreatedAt)
					.HasDefaultValueSql("GETDATE()");
				entity.HasOne(u => u.Role)
					.WithMany(r => r.Users)
					.HasForeignKey(u => u.RoleId)
					.OnDelete(DeleteBehavior.Restrict);
			});

	
			modelBuilder.Entity<LoginHistory>(entity =>
			{
				entity.ToTable("LoginHistory");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.IPAddress)
					.IsRequired()
					.HasMaxLength(45);
				entity.Property(e => e.LoginTime)
					.HasDefaultValueSql("GETDATE()");
				entity.HasOne(lh => lh.User)
					.WithMany(u => u.LoginHistories)
					.HasForeignKey(lh => lh.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

	
			modelBuilder.Entity<Order>(entity =>
			{
				entity.ToTable("Orders");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Contact)
					.IsRequired();
				entity.Property(e => e.Title)
					.IsRequired();
				entity.Property(e => e.MoneyValue)
					.IsRequired()
					.HasColumnType("decimal(18,2)");
				entity.Property(e => e.CreatedAt)
					.HasDefaultValueSql("GETDATE()");
				entity.HasOne(o => o.Status)
					.WithMany()
					.HasForeignKey(o => o.StatusId)
					.OnDelete(DeleteBehavior.Restrict);
				entity.HasOne(o => o.CustomerUser)
					.WithMany(u => u.CustomerOrders)
					.HasForeignKey(o => o.Customer)
					.OnDelete(DeleteBehavior.Restrict);
			});


			modelBuilder.Entity<Message>(entity =>
			{
				entity.ToTable("Messages");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Subject)
					.IsRequired()
					.HasMaxLength(255);
				entity.Property(e => e.Content)
					.IsRequired();
				entity.Property(e => e.CreatedAt)
					.HasDefaultValueSql("GETDATE()");
				entity.Property(e => e.Level)
					.IsRequired()
					.HasMaxLength(50);
				entity.HasOne(m => m.User)
					.WithMany(u => u.Messages)
					.HasForeignKey(m => m.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

		
			modelBuilder.Entity<TransactionHistory>(entity =>
			{
				entity.ToTable("TransactionHistory");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Amount)
					.IsRequired()
					.HasColumnType("decimal(18,2)");
				entity.Property(e => e.CreatedAt)
					.HasDefaultValueSql("GETDATE()");
				entity.HasOne(th => th.User)
					.WithMany(u => u.TransactionHistories)
					.HasForeignKey(th => th.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

	
			modelBuilder.Entity<Role>(entity =>
			{
				entity.ToTable("Roles");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.RoleName)
					.IsRequired()
					.HasMaxLength(50);
				entity.HasIndex(e => e.RoleName).IsUnique();
			});
		}

	}
}
