using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;

namespace Adelys_WebSite.ViewModels
{
    public class PlayerViewModel
    {
        public Dictionary<LuckpermsPlayer, Tuple<LuckpermsUserPermission, string>> PlayerPermissionsDico { get; set; }
        public Dictionary<LuckpermsPlayer, Tuple<LuckpermsUserPermission, string>> ParlementDico { get; set; }
    }
}
