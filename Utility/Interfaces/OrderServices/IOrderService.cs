using Data.Entities.Base;
using Utility.Services.OrderServices;

namespace Utility.Interfaces.OrderServices
{
    public interface IOrderService<T>
        where T : BaseFilterableEntity
    {
        public IQueryable<T> Order(IQueryable<T> query, VacancieResumeOrder order);
    }
}
