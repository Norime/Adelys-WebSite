using System;
using System.Collections.Generic;

namespace Adelys_WebSite.Models;

public partial class LuckpermsUserPermission
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Permission { get; set; } = null!;

    public bool Value { get; set; }

    public string Server { get; set; } = null!;

    public string World { get; set; } = null!;

    public long Expiry { get; set; }

    public string Contexts { get; set; } = null!;
}
