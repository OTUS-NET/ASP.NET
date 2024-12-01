using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public  class InMemoryRepositoryList<T> : IRepository<T> where T : BaseEntity
    {
        protected List<T> Data {  get; set; }

        public InMemoryRepositoryList(List<T> data) 
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.Where(x => !x.IsDeleted));
        }

        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Data.Find(x => x.Id == id && !x.IsDeleted));
        }

        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
             var empl = Data.Find(x => x.Id == id && !x.IsDeleted);

            if (empl != null)
            {

                empl.IsDeleted = true;
                return Task.FromResult(true);
            }
            else
                return Task.FromResult(false);
        }

        public Task<IEnumerable<T>> CreateAsync(IEnumerable<T> empl, CancellationToken cancellationToken)
        {

             Data.AddRange(empl.ToList());
                
            return Task.FromResult(Data.Where(x => !x.IsDeleted));
        }

        public Task<T?> ReplaceAsync(IEnumerable<T> empl, Guid id, CancellationToken cancellationToken)
        {
   
            if (empl != null)
            {
                int index = Data.FindIndex(x => x.Id == id);
                if (index == -1)
                {
                    Data[index] = empl.FirstOrDefault();
                }
            }

            return Task.FromResult(Data.Find(x => x.Id == id && !x.IsDeleted));
        }



    }
}
