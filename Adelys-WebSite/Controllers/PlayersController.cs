using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;

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
            Dictionary<LuckpermsPlayer, Tuple<LuckpermsUserPermission, string>> PlayerDico = new ();
            Dictionary<LuckpermsPlayer, Tuple<LuckpermsUserPermission, string>> ParlementDico = new ();
            List<LuckpermsPlayer> listPlayer = _playerBL.GetAllPlayers();
            foreach(LuckpermsPlayer player in listPlayer)
            {
                List<LuckpermsUserPermission> playerPermissions = _userPermissionBL.GetUserPermissionByPlayerUuid(player.Uuid);
                foreach(var permission in playerPermissions)
                {
                    string GroupName = permission.Permission.Split('.')[1];
                    if (GroupName == "parlement") {

                        var UserSkinEncoded = _playerBL.GetPlayerSkin(player.Uuid);
                        dynamic SkinEncoded = JObject.Parse(UserSkinEncoded);
                        string skinUrlEncoded = SkinEncoded.properties[0].value;
                        byte[] bytes = Convert.FromBase64String(skinUrlEncoded);
                        string decodedSkinUrl = Encoding.UTF8.GetString(bytes);
                        JObject skinJson = JObject.Parse(decodedSkinUrl);
                        string skinUrl = skinJson["textures"]["SKIN"]["url"].ToString();
                        ParlementDico.Add(player, new Tuple<LuckpermsUserPermission, string>(permission, skinUrl));

                        
                    }
                    else
                    {
                        var UserSkinEncoded = _playerBL.GetPlayerSkin(player.Uuid);
                        dynamic SkinEncoded = JObject.Parse(UserSkinEncoded);
                        string skinUrlEncoded = SkinEncoded.properties[0].value;
                        byte[] bytes = Convert.FromBase64String(skinUrlEncoded);
                        string decodedSkinUrl = Encoding.UTF8.GetString(bytes);
                        JObject skinJson = JObject.Parse(decodedSkinUrl);
                        string skinUrl = skinJson["textures"]["SKIN"]["url"].ToString();
                        PlayerDico.Add(player, new Tuple<LuckpermsUserPermission, string>(permission, skinUrl));
                    }
                }
            }
            PlayerViewModel viewModel= new PlayerViewModel();
            viewModel.PlayerPermissionsDico = PlayerDico.OrderBy(x => x.Key.Username).ToDictionary(x => x.Key, x => x.Value);
            viewModel.ParlementDico = ParlementDico.OrderBy(x => x.Key.Username).ToDictionary(x => x.Key, x => x.Value);
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