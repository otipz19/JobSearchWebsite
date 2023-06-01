using Data.Entities.Base;
using FluentValidation;
using Data;
using Microsoft.AspNetCore.Mvc;
using Utility.Toaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Utility.Utilities;

namespace JobSearchWebsite.MVC.Controllers
{
	[Authorize(Policy = Constants.AdminPolicy)]
	public abstract class BaseFilteringEntityController<T> : Controller
		where T : BaseFilteringEntity
	{
        private readonly string _typeName = typeof(T).Name;

        private readonly AppDbContext _dbContext;
		private readonly IValidator<BaseNamedEntity> _validator;

		public BaseFilteringEntityController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator)
		{
			_dbContext = dbContext;
			_validator = validator;
		}

		[HttpGet]
		public virtual async Task<IActionResult> Index()
		{
			var entities = await _dbContext.Set<T>().AsNoTracking().ToListAsync();
			ViewData["Title"] = _typeName;
			return View(viewName: "IndexFilteringEntity", model: entities);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Create(T entity)
		{
			var validation = await _validator.ValidateAsync(entity);
			if (!validation.IsValid)
			{
				TempData.Toaster().Error(validation.Errors.First().ErrorMessage, "Invalid input");
				return RedirectToAction(nameof(Index));
			}
			_dbContext.Set<T>().Add(entity);
			await _dbContext.SaveChangesAsync();
			TempData.Toaster().Success($"{_typeName} created successfully");
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Update(T entity)
		{
			var validation = await _validator.ValidateAsync(entity);
			if (!validation.IsValid)
			{
				TempData.Toaster().Error(validation.Errors.First().ErrorMessage, "Invalid input");
				return RedirectToAction(nameof(Index));
			}
			T toUpdate = await _dbContext.Set<T>().FindAsync(entity.Id);
			if (toUpdate == null)
			{
				TempData.Toaster().Error($"{_typeName} you're trying to update no longer exists", "Not found");
				return RedirectToAction(nameof(Index));
			}
			toUpdate.Name = entity.Name;
			_dbContext.Set<T>().Update(toUpdate);
			await _dbContext.SaveChangesAsync();
			TempData.Toaster().Success($"{_typeName} updated successfully");
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Delete(T entity)
		{
			T toDelete = await _dbContext.Set<T>().FindAsync(entity.Id);
			if (toDelete == null)
			{
				TempData.Toaster().Error($"{_typeName} you're trying to delete no longer exists", "Not found");
				return RedirectToAction(nameof(Index));
			}
			_dbContext.Set<T>().Remove(toDelete);
			await _dbContext.SaveChangesAsync();
			TempData.Toaster().Success($"{_typeName} deleted successfully");
			return RedirectToAction(nameof(Index));
		}
	}
}
