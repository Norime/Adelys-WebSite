using System;
using System.Collections.Generic;

namespace Adelys_WebSite.Models;

public partial class LuckpermsPlayer
{
    public string Uuid { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PrimaryGroup { get; set; } = null!;
}
