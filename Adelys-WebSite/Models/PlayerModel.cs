using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;

namespace Adelys_WebSite.Models
{
    public class PlayerModel
    {
        public string uuid { get; set; }
        public string userName { get; set; }
        public string primary_group { get; set; }
    }
}
