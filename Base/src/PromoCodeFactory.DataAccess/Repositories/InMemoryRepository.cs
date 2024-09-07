using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected IList<T> Data { get; set; }
        private readonly ILogger<T> _logger;

        public InMemoryRepository(IList<T> data, ILogger<T> logger)
        {
            Data = data;
            _logger = logger;
        }

        public Task<IList<T>> GetAllAsync(CancellationToken cts)
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id, CancellationToken cts)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(T entity, CancellationToken cts)
        {
            Data.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateByIdAsync(Guid id, T entity, CancellationToken cts)
        {
            Data[Data.IndexOf(Data.FirstOrDefault(x => x.Id == id))] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteByIdAsync(Guid id, CancellationToken cts)
        {
            Data.Remove(Data.FirstOrDefault(e => e?.Id == id));
            return Task.CompletedTask;
        }

        public Task<bool> Exists(Guid id)
        {
            return Task.FromResult(Data.Any(x => x.Id == id)); 
        }
    }
}