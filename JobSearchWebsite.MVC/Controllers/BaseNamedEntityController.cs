using FluentValidation;
using JobSearchWebsite.Data;
using JobSearchWebsite.Data.Entities.Base;
using Microsoft.AspNetCore.Mvc;
using Utility.Toaster;

namespace JobSearchWebsite.MVC.Controllers
{
	public abstract class BaseNamedEntityController<T> : Controller
		where T : BaseNamedEntity
	{
		private readonly AppDbContext _dbContext;
		private readonly IValidator<BaseNamedEntity> _validator;

		public BaseNamedEntityController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator)
		{
			_dbContext = dbContext;
			_validator = validator;
		}

		[HttpGet]
		public virtual IActionResult Index()
		{
			var entities = _dbContext.Set<T>();
			ViewData["Title"] = typeof(T).Name;
			return View(viewName: "IndexBaseNamedEntity", model: entities);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Create(T entity)
		{
			var validation = await _validator.ValidateAsync(entity);
			if (!validation.IsValid)
			{
				TempData.Toaster().Error(validation.Errors.First().ErrorMessage);
				return RedirectToAction(nameof(Index));
			}
			_dbContext.Set<T>().Add(entity);
			await _dbContext.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Update(T entity)
		{
			var validation = await _validator.ValidateAsync(entity);
			if (!validation.IsValid)
			{
				TempData.Toaster().Error(validation.Errors.First().ErrorMessage);
				return RedirectToAction(nameof(Index));
			}
			T toUpdate = await _dbContext.Set<T>().FindAsync(entity.Id);
			if (toUpdate == null)
			{
				return RedirectToAction(nameof(Index));
			}
			toUpdate.Name = entity.Name;
			_dbContext.Set<T>().Update(toUpdate);
			await _dbContext.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Delete(T entity)
		{
			T toDelete = await _dbContext.Set<T>().FindAsync(entity.Id);
			if (toDelete == null)
			{
				return RedirectToAction(nameof(Index));
			}
			_dbContext.Set<T>().Remove(toDelete);
			await _dbContext.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
