using System;
using System.Collections.Generic;

namespace Shared.Models;

public partial class ApplicationDTO
{
    public int Applicationid { get; set; }

    public string Userid { get; set; } = null!;

    public string Roleid { get; set; } = null!;

    public string Applicationname { get; set; } = null!;
}
