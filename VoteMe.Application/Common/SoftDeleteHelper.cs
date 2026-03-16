using VoteMe.Application.Interface.IRepositories;

namespace VoteMe.Application.Common
{
    public static class SoftDeleteHelper
    {
        public static async Task CascadeSoftDeleteForOrganizationAsync(
            this IUnitOfWork unitOfWork,
            Guid organizationId,
            CancellationToken ct = default)
        {
            var org = await unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (org != null)
            {
                org.MarkAsDeleted();
                unitOfWork.Organizations.Update(org);
            }

            var members = await unitOfWork.OrganizationMembers
                .FindAsync(m => m.OrganizationId == organizationId);
            foreach (var m in members)
            {
                m.MarkAsDeleted();
                unitOfWork.OrganizationMembers.Update(m);
            }

            var elections = await unitOfWork.Elections
                .FindAsync(e => e.OrganizationId == organizationId);
            foreach (var e in elections)
            {
                e.MarkAsDeleted();
                unitOfWork.Elections.Update(e);
            }

            var categories = await unitOfWork.ElectionCategories
                .FindAsync(ec => ec.Election.OrganizationId == organizationId);
            foreach (var c in categories)
            {
                c.MarkAsDeleted();
                unitOfWork.ElectionCategories.Update(c);
            }

            var candidates = await unitOfWork.Candidates
                .FindAsync(c => c.ElectionCategory.Election.OrganizationId == organizationId);
            foreach (var c in candidates)
            {
                c.MarkAsDeleted();
                unitOfWork.Candidates.Update(c);
            }
        }
    }
}
