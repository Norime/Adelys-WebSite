using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.ViewModels
{
    public class PlayerViewModel
    {
        public Dictionary<LuckpermsPlayer, LuckpermsUserPermission> PlayerPermissionsDico { get; set; }
        public Dictionary<LuckpermsPlayer, LuckpermsUserPermission> ParlementDico { get; set; }
    }
}
