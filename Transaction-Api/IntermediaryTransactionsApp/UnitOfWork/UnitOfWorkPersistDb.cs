
using System;
using IntermediaryTransactionsApp.Db.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace IntermediaryTransactionsApp.UnitOfWork
{
	public class UnitOfWorkPersistDb : IUnitOfWorkPersistDb
	{
		private readonly ApplicationDbContext _context;
		private IDbContextTransaction _transaction;

		public UnitOfWorkPersistDb(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task BeginTransactionAsync()
		{
			_transaction = await _context.Database.BeginTransactionAsync();
		}

		public async Task CommitAsync()
		{
			if (_transaction != null)
			{
				await _transaction.CommitAsync();
				await _transaction.DisposeAsync();
			}
		}

		public async Task RollbackAsync()
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync();
				await _transaction.DisposeAsync();
			}
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
