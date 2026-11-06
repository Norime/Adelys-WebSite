using Adelys_WebSite.BL;
using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;

namespace Adelys_WebSite.Controllers
{
    public class PlayersController : Controller
    {
        private const string GroupPrefix = "group.";
        private const string DefaultGroup = "default";
        private const string ParlementGroup = "parlement";

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
            ILookup<string, LuckpermsUserPermission> permissionsByPlayer = _userPermissionBL.GetAllUsersPermissions().ToLookup(p => p.Uuid);
            PlayerViewModel viewModel = new PlayerViewModel();

            foreach (LuckpermsPlayer player in _playerBL.GetAllPlayers().OrderBy(p => p.Username))
            {
                List<string> groups = GetActiveGroups(permissionsByPlayer[player.Uuid]);
                PlayerCardViewModel card = new PlayerCardViewModel
                {
                    Uuid = player.Uuid,
                    Username = player.Username,
                    Rank = GetRank(player, groups),
                    SkinUrl = GetSkinUrl(player)
                };

                if (groups.Contains(ParlementGroup) || player.PrimaryGroup == ParlementGroup)
                {
                    viewModel.ParlementPlayers.Add(card);
                }
                else
                {
                    viewModel.OtherPlayers.Add(card);
                }
            }

            return View("PlayersIndex", viewModel);
        }

        /// <summary>
        /// Récupère les groupes LuckPerms actifs d'un joueur, les autres permissions ne sont pas des rangs
        /// </summary>
        private static List<string> GetActiveGroups(IEnumerable<LuckpermsUserPermission> permissions)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return permissions
                .Where(p => p.Value && p.Permission.StartsWith(GroupPrefix) && (p.Expiry == 0 || p.Expiry > now))
                .Select(p => p.Permission.Substring(GroupPrefix.Length))
                .Distinct()
                .OrderBy(g => g)
                .ToList();
        }

        /// <summary>
        /// Le groupe principal est le rang du joueur, sauf s'il est resté sur default alors qu'il a un autre groupe
        /// </summary>
        private static string GetRank(LuckpermsPlayer player, List<string> groups)
        {
            if (player.PrimaryGroup != DefaultGroup)
            {
                return player.PrimaryGroup;
            }
            return groups.FirstOrDefault(g => g != DefaultGroup) ?? DefaultGroup;
        }

        /// <summary>
        /// Un UUID version 4 est un compte Mojang, sinon le serveur est en offline et seul le pseudo permet de retrouver le skin
        /// </summary>
        private static string GetSkinUrl(LuckpermsPlayer player)
        {
            string identifier = player.Uuid.Length > 14 && player.Uuid[14] == '4' ? player.Uuid : player.Username;
            return $"https://mc-heads.net/body/{Uri.EscapeDataString(identifier)}/200";
        }

        public string GetAllUserSkin()
        {
            return "";
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