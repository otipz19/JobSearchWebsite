using Data.Entities;
using Utility.Interfaces.OrderServices;

namespace Utility.Services.OrderServices
{
    public class ResumeOrderService: BaseOrderService<Resume>, IResumeOrderService
    {
        public override IQueryable<Resume> Order(IQueryable<Resume> query, VacancieResumeOrder order)
        {
            if(order.OrderType == OrderType.ByResponds && order.IsAscending)
            {
                return query.OrderBy(e => e.JobOffers.Count());
            }
            else if(order.OrderType == OrderType.ByResponds)
            {
                return query.OrderByDescending(e => e.JobOffers.Count());
            }

            return base.Order(query, order);
        }
    }
}