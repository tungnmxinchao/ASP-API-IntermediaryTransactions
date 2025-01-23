using IntermediaryTransactionsApp.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace IntermediaryTransactionsApp.Db
{
	public static class DbServiceExtension
	{
		public static void AddDatabaseService(this IServiceCollection services,
			string connectionString) => services.AddDbContext<ApplicationDbContext>(d => d.UseSqlServer(connectionString));
	}
}
