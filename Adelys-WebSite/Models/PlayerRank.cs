namespace Adelys_WebSite.Models
{
    /// <summary>
    /// Rang affiché d'un joueur : son groupe de ville, parlement étant signalé à part
    /// </summary>
    public record PlayerRank(LuckpermsPlayer Player, string Rank, bool IsParlement);
}
