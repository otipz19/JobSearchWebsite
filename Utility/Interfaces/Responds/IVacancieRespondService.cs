using Data.Entities;
using Data.Enums;
using System.Security.Claims;

namespace Utility.Interfaces.Responds
{
	public interface IVacancieRespondService
	{
		/// <exception cref="ApplicationException"></exception>
		public Task CreateVacancieRespond(int resumeId, int vacancieId);

		public Task<List<VacancieRespond>> GetVacancieRespondsForJobseeker(ClaimsPrincipal user);

		public Task<List<VacancieRespond>> GetVacancieRespondsForCompany(ClaimsPrincipal user);

		public Task<VacancieRespond> GetVacancieRespondForCompany(ClaimsPrincipal user, int resumeId, int vacancieId);

		public Task ChangeStatus(VacancieRespond respond, VacancieRespondStatus status);
	}
}
