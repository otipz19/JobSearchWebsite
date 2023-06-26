using Ardalis.GuardClauses;
using Data;
using Data.Entities.Base;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using Utility.Utilities;
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

        protected string GetShortDescription(string source)
		{
			const int ShortDescriptionLength = 350;
			string shortDesc = $"{source.Substring(0, source.Length < ShortDescriptionLength ? source.Length : ShortDescriptionLength)}...";

			return CloseHtmlTags(shortDesc);
		}

		private string CloseHtmlTags(string shortDesc)
		{
			Dictionary<string, int> openedTags = new();

			int? curOpenBracket = null;
			int? curSlash = null;
			for (int i = 0; i < shortDesc.Length; i++)
			{
				if (shortDesc[i] == '<')
				{
					curOpenBracket = i;
				}
				if (shortDesc[i] == '/')
				{
					curSlash = i;
				}
				else if (shortDesc[i] == '>')
				{
					if (curOpenBracket.HasValue && curSlash.HasValue)
					{
						//Get only content of
						int indexOfContentStart = curOpenBracket.Value + 2;
						int contentLength = i - indexOfContentStart;
						if (contentLength > 0)
						{
							string tag = shortDesc.Substring(indexOfContentStart, contentLength);
							//Not empty tag
							if (tag.Length > 0)
							{
								if (openedTags.ContainsKey(tag) && openedTags[tag] > 0)
									openedTags[tag]--;
							}
						}
						curOpenBracket = null;
						curSlash = null;
					}
					if (curOpenBracket.HasValue)
					{
						//Get only content of tag
						int indexOfContentStart = curOpenBracket.Value + 1;
						int contentLength = i - indexOfContentStart;
						if (contentLength > 0)
						{
							string tag = shortDesc.Substring(indexOfContentStart, contentLength);
							//Not empty tag
							if (tag.Length > 0)
							{
								if (openedTags.ContainsKey(tag))
									openedTags[tag]++;
								else
									openedTags[tag] = 1;
							}
						}
						curOpenBracket = null;
					}
				}
			}

			StringBuilder builder = new StringBuilder(shortDesc);
			foreach (var tag in openedTags.Keys.Where(k => openedTags[k] > 0))
			{
				builder.Append("</");
				builder.Append(tag);
				builder.Append(">");
			}
			return builder.ToString();
		}

		protected string GetPublishedAgo(DateTime createdAt)
        {
            string str = createdAt.GetTimePassedString();
            if (str == "")
                return "Just published";
            return "Published" + str;
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
    }
}
