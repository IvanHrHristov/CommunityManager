namespace CommunityManager.Core.Models.Community
{
    public class AllCommunitiesQueryModel
    {
        public const int CommunitiesPerPage = 3;

        public string? SearchTerm { get; set; }

        public CommunitySorting Sorting { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalCommunitiesCount { get; set; }

        public IEnumerable<CommunityViewModel> Communities { get; set; } = Enumerable.Empty<CommunityViewModel>();
    }
}
