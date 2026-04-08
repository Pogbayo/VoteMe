namespace VoteMe.Domain.Entities
{
    public class Organization : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UniqueKey { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<Election> Elections { get; set; } = new List<Election>();
        public ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
    }
}
