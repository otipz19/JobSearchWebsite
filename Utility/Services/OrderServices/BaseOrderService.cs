using Data.Entities.Base;

namespace Utility.Services.OrderServices
{
    public abstract class BaseOrderService<T>
        where T : BaseFilterableEntity
    {
        public virtual IQueryable<T> Order(IQueryable<T> query, VacancieResumeOrder order)
        {
            if(order.IsAscending)
            {
                switch (order.OrderType)
                {
                    case OrderType.ByPublishDate:
                        return query.OrderBy(e => e.PublishedAt);
                    case OrderType.ByWatches:
                        return query.OrderBy(e => e.CountWatched);
                }
            }
            else
            {
                switch (order.OrderType)
                {
                    case OrderType.ByPublishDate:
                        return query.OrderByDescending(e => e.PublishedAt);
                    case OrderType.ByWatches:
                        return query.OrderByDescending(e => e.CountWatched);
                }
            }

            return query;
        }
    }
}