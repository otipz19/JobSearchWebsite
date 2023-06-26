using Data.Entities;
using Utility.Interfaces.OrderServices;

namespace Utility.Services.OrderServices
{
    public class VacancieOrderService: BaseOrderService<Vacancie>, IVacancieOrderService
    {
        public override IQueryable<Vacancie> Order(IQueryable<Vacancie> query, VacancieResumeOrder order)
        {
            if(order.OrderType == OrderType.ByResponds && order.IsAscending)
            {
                return query.OrderBy(e => e.VacancieResponds.Count());
            }
            else if(order.OrderType == OrderType.ByResponds)
            {
                return query.OrderByDescending(e => e.VacancieResponds.Count());
            }

            return base.Order(query, order);
        }
    }
}