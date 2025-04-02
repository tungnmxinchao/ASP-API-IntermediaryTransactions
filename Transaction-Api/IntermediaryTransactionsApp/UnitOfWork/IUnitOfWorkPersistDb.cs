namespace IntermediaryTransactionsApp.UnitOfWork
{
	public interface IUnitOfWorkPersistDb
	{
		Task BeginTransactionAsync();
		Task CommitAsync();
		Task RollbackAsync();
		Task<int> SaveChangesAsync();
	}
}
