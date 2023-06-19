using Data.Entities;
using Data.Enums;
using System.Security.Claims;

namespace Utility.Utilities
{
    public static class ClaimsPrincipalExtentions
    {
        public static bool IsCompany(this ClaimsPrincipal user)
        {
            return user.IsInRole(AppUserRoleType.Company.ToString());
        }

        public static bool IsJobseeker(this ClaimsPrincipal user)
        {
            return user.IsInRole(AppUserRoleType.Jobseeker.ToString());
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(AppUserRoleType.Jobseeker.ToString());
        }

        public static bool IsOwner(this ClaimsPrincipal user, Vacancie vacancie)
        {
            return vacancie.Company.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static bool IsOwner(this ClaimsPrincipal user, Resume resume)
        {
            return resume.Jobseeker.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
