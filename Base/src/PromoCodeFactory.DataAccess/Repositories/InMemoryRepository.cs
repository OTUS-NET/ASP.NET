using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        #region Homework 1
        public Task<T> CreateAsync(T entity)
        {
            // Приводим Data к типу IList<T> для добавления нового элемента:
            if (Data is IList<T> dataList)
            {
                // Добавляем новый элемент
                dataList.Add(entity);
            }
            else
            {
                // Если что-то пошло не так:
                throw new InvalidOperationException("CreateAsync: Data collection is not modifiable.");
            }
            // Возвращаяем созданный элемент как Task:
            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(T updateEntity)
        {
            // Приводим Data к типу IList<T> для возможности его изменения:
            if (Data is IList<T> dataList)
            {
                // Находим элемент по Id:
                var dataEntity = dataList.FirstOrDefault(x => x.Id == updateEntity.Id);
                if (dataEntity != null)
                {
                    var index = dataList.IndexOf(dataEntity);
                    dataList[index] = updateEntity;
                    // Возвращаем обновленный объект:
                    return Task.FromResult(updateEntity);
                }
            }
            else
            {
                // Если что-то пошло не так:
                throw new InvalidOperationException("UpdateAsync: Data collection is not modifiable.");
            }
            // Если элемент не найден, возвращаем null:
            return Task.FromResult<T>(null);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            // Приводим Data к типу IList<T> для возможности его изменения:
            if (Data is List<T> dataList)
            {
                // Находим элемент по Id:
                var entity = dataList.FirstOrDefault(x => x.Id == id);
                if (entity != null)
                {
                    // Удаляем найденный элемент:
                    dataList.Remove(entity);
                    return Task.FromResult(true);
                }
            }
            else
            {
                // Если что-то пошло не так:
                throw new InvalidOperationException("DeleteAsync: Data collection is not modifiable.");
            }
            // Если элемент не найден, возвращаем false:
            return Task.FromResult(false);
        }
        #endregion
    }
}