namespace CommunityManager.Core.Models.Community
{
    public class CommunityQueryModel
    {
        public int TotalCommunities { get; set; }

        public IEnumerable<CommunityViewModel> Communities { get; set; } = new List<CommunityViewModel>();
    }
}
