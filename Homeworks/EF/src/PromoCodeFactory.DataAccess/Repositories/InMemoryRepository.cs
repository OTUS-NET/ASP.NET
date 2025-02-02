//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using PromoCodeFactory.Core.Abstractions.Repositories;
//using PromoCodeFactory.Core.Domain;

//namespace PromoCodeFactory.DataAccess.Repositories
//{
//    public class InMemoryRepository<T>
//        : IRepository<T>
//        where T : BaseEntity
//    {
//        protected IEnumerable<T> Data { get; set; }

//        public InMemoryRepository(IEnumerable<T> data)
//        {
//            Data = data;
//        }

//        public Task<IEnumerable<T>> GetAllAsync()
//        {
//            return Task.FromResult(Data);
//        }

//        public Task<T> GetByIdAsync(Guid id)
//        {
//            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
//        }

//        public Task AddAsync(T customer)
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateAsync(T customer)
//        {
//            throw new NotImplementedException();
//        }

//        public Task DeleteAsync(Guid id)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}