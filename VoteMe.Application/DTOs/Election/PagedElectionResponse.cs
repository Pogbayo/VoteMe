namespace VoteMe.Application.DTOs.Election
{
    public class PagedElectionResponse
    {
        public IEnumerable<ElectionDto> Items { get; set; } = Enumerable.Empty<ElectionDto>();
        public int TotalCount { get; set; }
    }
}
