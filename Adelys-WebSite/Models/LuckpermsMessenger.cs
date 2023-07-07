using System;
using System.Collections.Generic;

namespace Adelys_WebSite.Models;

public partial class LuckpermsMessenger
{
    public int Id { get; set; }

    public DateTime Time { get; set; }

    public string Msg { get; set; } = null!;
}
