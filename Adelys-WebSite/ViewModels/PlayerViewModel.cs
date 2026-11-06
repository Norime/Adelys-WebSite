namespace Adelys_WebSite.ViewModels
{
    public class PlayerViewModel
    {
        public List<PlayerCardViewModel> ParlementPlayers { get; set; } = new();
        public List<PlayerCardViewModel> OtherPlayers { get; set; } = new();
    }

    public class PlayerCardViewModel
    {
        public string Uuid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public string SkinUrl { get; set; } = string.Empty;
    }
}
