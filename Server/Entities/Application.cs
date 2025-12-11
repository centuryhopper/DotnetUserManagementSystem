using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Application
{
    public int Applicationid { get; set; }

    public string Userid { get; set; } = null!;

    public string Roleid { get; set; } = null!;

    public string Applicationname { get; set; } = null!;

    public bool? Requirestwofactor { get; set; }
}
