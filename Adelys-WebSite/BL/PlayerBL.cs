using Adelys_WebSite.BL.Interfaces;
using Adelys_WebSite.DAL.Repositories.Interfaces;
using Adelys_WebSite.DAL.UnitOfWork.Interfaces;
using Adelys_WebSite.Models;
using Adelys_WebSite.SAL.Interface;
using System.Threading.Tasks;

namespace Adelys_WebSite.BL
{
    public class PlayerBL : BaseBL<LuckpermsPlayer>, IPlayerBL
    {
        private const string GroupPrefix = "group.";
        private const string DefaultGroup = "default";
        private const string ParlementGroup = "parlement";

        protected override IPlayerRepository Repository => _unitOfWork.PlayerRepositoryDAO;
        private readonly IServiceMojangAccess _mojangApi;
        private readonly ICrafatarAccess _crafatarApi;

        public PlayerBL(IUnitOfWork unitOfWork, IServiceMojangAccess mojangAccess, ICrafatarAccess crafatarApi) : base(unitOfWork)
        {
            _mojangApi = mojangAccess;
            _crafatarApi = crafatarApi;
        }

        /// <summary>
        /// Récupère tout les customers via les repository customer
        /// </summary>
        /// <returns></returns>
        public List<LuckpermsPlayer> GetAllPlayers()
        {
            return Repository.GetAllPlayers();
        }

        /// <summary>
        /// Rang de chaque joueur, partagé par la page des joueurs et le filtre par groupe des commerces
        /// </summary>
        public List<PlayerRank> GetPlayerRanks()
        {
            ILookup<string, LuckpermsUserPermission> permissionsByPlayer = _unitOfWork.UserPermissionRepositoryDAO.GetAllUsersPermissions().ToLookup(p => p.Uuid);
            return GetAllPlayers()
                .OrderBy(p => p.Username)
                .Select(player =>
                {
                    List<string> groups = GetActiveGroups(permissionsByPlayer[player.Uuid]);
                    bool isParlement = groups.Contains(ParlementGroup) || player.PrimaryGroup == ParlementGroup;
                    return new PlayerRank(player, GetRank(player, groups), isParlement);
                })
                .ToList();
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
        /// Le rang affiché est le groupe de ville du joueur : parlement est signalé par le badge P de sa carte et default n'est pas un rang
        /// </summary>
        private static string GetRank(LuckpermsPlayer player, List<string> groups)
        {
            if (IsTownGroup(player.PrimaryGroup))
            {
                return player.PrimaryGroup;
            }
            return groups.FirstOrDefault(IsTownGroup) ?? (groups.Contains(ParlementGroup) ? ParlementGroup : player.PrimaryGroup);
        }

        private static bool IsTownGroup(string group)
        {
            return group != DefaultGroup && group != ParlementGroup;
        }

        /// <summary>
        /// Récupère un customer en fonction de son identifiant à partir du repository générale
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LuckpermsPlayer? GetPlayer(int id)
        {
            return Repository.GetById(id);
        }

        public string GetPlayerSkin(string uuid)
        {
            Task<string> playerSkin = _mojangApi.GetSkinData(uuid);
            playerSkin.Wait();
            return playerSkin.Result;
        }

        public string GetPlayerSkinImg(string uuid)
        {
            Task<string> playerSkin = _crafatarApi.GetPlayerSkin(uuid);
            playerSkin.Wait();
            return playerSkin.Result;
        }
    }
}
