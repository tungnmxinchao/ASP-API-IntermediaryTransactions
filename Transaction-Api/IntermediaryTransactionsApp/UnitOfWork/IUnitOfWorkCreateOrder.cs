namespace IntermediaryTransactionsApp.UnitOfWork
{
	public interface IUnitOfWorkCreateOrder
	{
		Task BeginTransactionAsync();
		Task CommitAsync();
		Task RollbackAsync();
		Task<int> SaveChangesAsync();
	}
}
