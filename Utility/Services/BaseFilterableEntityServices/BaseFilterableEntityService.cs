using Ardalis.GuardClauses;
using Data;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;
using Utility.ViewModels;

namespace Utility.Services.BaseFilterableEntityServices
{
    public abstract class BaseFilterableEntityService<TEntity>
        where TEntity: BaseFilterableEntity
    {
        protected readonly AppDbContext _dbContext;

        protected BaseFilterableEntityService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity> EagerLoad(int id)
        {
            return await IncludeAllNavProps().FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<TEntity> EagerLoadAsNoTracking(int id)
        {
            return await IncludeAllNavProps().AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<TEntity>> EagerLoadListAsNoTracking()
        {
            return await IncludeAllNavProps().AsNoTracking().ToListAsync();
        }

        protected IQueryable<TEntity> IncludeAllNavProps()
        {
            var navProps = typeof(TEntity).GetProperties()
                .Where(p => p.GetGetMethod().IsVirtual);
            var query = _dbContext.Set<TEntity>().AsQueryable();
            foreach(var navProp in navProps)
            {
                query = query.Include(navProp.Name);
            }
            return query;
        }

        protected string GetShortDescription(string d)
        {
            const int ShortDescriptionLength = 350;
            return $"{d.Substring(0, d.Length < ShortDescriptionLength ? d.Length : ShortDescriptionLength)}...";
        }

        protected string GetCreatedAgo(DateTime createdAt)
        {
            TimeSpan createdAgo = DateTime.Now - createdAt;
            string msg = "Created ";
            if (createdAgo.Days > 0)
                msg += $"{createdAgo.Days} days ago";
            else if (createdAgo.Hours > 0)
				msg += $"{createdAgo.Hours} hours ago";
            else if (createdAgo.Minutes > 0)
				msg += $"{createdAgo.Minutes} minutes ago";
            else
                msg = "Just created";
            return msg;
        }

        protected async Task<int> GetForeignKey<T>(int sourceId)
                where T : BaseFilteringEntity
        {
            var foreignKey = await _dbContext.Set<T>().Select(e => new { e.Id }).FirstOrDefaultAsync(e => e.Id == sourceId);
            Guard.Against.Null(foreignKey.Id);
            return foreignKey.Id;
        }

        protected async Task SetCollectionNavProp<T>(List<T> navProp, IEnumerable<string> ids)
            where T : BaseFilteringEntity
        {
            navProp.AddRange(await _dbContext.Set<T>()
                .Where(e => ids.Select(k => int.Parse(k)).Contains(e.Id))
                .ToListAsync());
        }

        protected async Task<List<CheckboxOption>> MapCheckboxOptions(IQueryable<BaseFilteringEntity> entities)
        {
            return await entities.Select(f => new CheckboxOption()
            {
                IsChecked = false,
                Text = f.Name,
                Value = f.Id.ToString(),
            })
            .ToListAsync();
        }
    }
}
