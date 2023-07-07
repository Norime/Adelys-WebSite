using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adelys_WebSite.Controllers
{
    public class PlayersController : Controller
    {
        private readonly IPlayerBL _playerBL;
        private readonly IUserPermissionBL _userPermissionBL;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(ILogger<PlayersController> logger, IPlayerBL playerBL, IUserPermissionBL userPermissionBL)
        {
            _logger = logger;
            _playerBL = playerBL;
            _userPermissionBL = userPermissionBL;
        }

        public IActionResult Index()
        {
            Dictionary<LuckpermsPlayer, LuckpermsUserPermission> PlayerDico = new Dictionary<LuckpermsPlayer, LuckpermsUserPermission>();
            Dictionary<LuckpermsPlayer, LuckpermsUserPermission> ParlementDico = new Dictionary<LuckpermsPlayer,LuckpermsUserPermission>();
            List<LuckpermsPlayer> listPlayer = _playerBL.GetAllPlayers();
            foreach(LuckpermsPlayer player in listPlayer)
            {
                List<LuckpermsUserPermission> playerPermissions = _userPermissionBL.GetUserPermissionByPlayerUuid(player.Uuid);
                foreach(var permission in playerPermissions)
                {
                    string GroupName = permission.Permission.Split('.')[1];
                    if (GroupName == "parlement") {
                        ParlementDico.Add(player, permission);
                    }
                    else
                    {
                        PlayerDico.Add(player, permission);
                    }
                }
            }
            PlayerViewModel viewModel= new PlayerViewModel();
            viewModel.PlayerPermissionsDico = PlayerDico;
            viewModel.ParlementDico = ParlementDico;
            return View("PlayersIndex", viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}