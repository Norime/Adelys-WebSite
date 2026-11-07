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
        private const string ParlementGroup = "parlement";

        private readonly IPlayerBL _playerBL;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(ILogger<PlayersController> logger, IPlayerBL playerBL)
        {
            _logger = logger;
            _playerBL = playerBL;
        }

        public IActionResult Index()
        {
            PlayerViewModel viewModel = new PlayerViewModel();

            foreach (PlayerRank playerRank in _playerBL.GetPlayerRanks())
            {
                LuckpermsPlayer player = playerRank.Player;
                PlayerCardViewModel card = new PlayerCardViewModel
                {
                    Uuid = player.Uuid,
                    Username = player.Username,
                    Rank = playerRank.Rank,
                    SkinUrl = GetSkinUrl(player)
                };

                if (playerRank.IsParlement)
                {
                    viewModel.ParlementPlayers.Add(card);
                }
                else
                {
                    viewModel.OtherPlayers.Add(card);
                }
            }

            viewModel.Groups = viewModel.ParlementPlayers.Concat(viewModel.OtherPlayers)
                .Select(p => p.Rank)
                .Where(g => g != ParlementGroup)
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            return View("PlayersIndex", viewModel);
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