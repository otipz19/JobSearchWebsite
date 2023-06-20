using Data.Entities;
using Data.Enums;
using System.Security.Claims;
using Utility.Exceptions;
using Utility.ViewModels;

namespace Utility.Interfaces.Responds
{
	public interface IVacancieRespondService
	{
		/// <exception cref="ApplicationException"></exception>
		public Task CreateVacancieRespond(int resumeId, int vacancieId);

		public Task<List<VacancieRespond>> GetVacancieRespondsForJobseeker(ClaimsPrincipal user);

		public Task<List<VacancieRespond>> GetVacancieRespondsForCompany(ClaimsPrincipal user);

		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="NoAccessException"></exception>
		public Task<VacancieRespond> GetVacancieRespondForCompany(ClaimsPrincipal user, int resumeId, int vacancieId);

		/// <exception cref="ArgumentException"></exception>
		public Task ChangeStatus(VacancieRespond respond, RespondStatus status);

		public List<VacancieRespondIndexVm> GetIndexVmList(IEnumerable<VacancieRespond> responds);

		public VacancieRespondIndexVm GetIndexVm(VacancieRespond respond);
    }
}
