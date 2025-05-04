using System;
using System.Collections.Generic;

namespace carnetDb.Models;

public partial class WrkEcrLig
{
    public string CodCpt { get; set; } = null!;

    public int Nol { get; set; }

    public DateOnly? Jma { get; set; }

    public DateOnly? JmaVal { get; set; }

    public string? NoChq { get; set; }

    public string? Lib1 { get; set; }

    public string? Lib2 { get; set; }

    public double? Deb { get; set; }

    public double? Cre { get; set; }

    public string? CodSfa { get; set; }

    public double? SldProgressif { get; set; }
}
public partial class WrkEcrLigTransfertModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CodCpt { get; set; } = null!;

    public int Nol { get; set; }

    public DateOnly? Jma { get; set; }

    public DateOnly? JmaVal { get; set; }

    public string? NoChq { get; set; }

    public string? Lib1 { get; set; }

    public string? Lib2 { get; set; }

    public double? Deb { get; set; }

    public double? Cre { get; set; }

    public string? CodSfa { get; set; }

    public double? SldProgressif { get; set; }
}
