using System;
using System.Collections.Generic;

namespace carnetDb.Models;

public partial class FicFam
{
    public string CodFam { get; set; } = null!;

    public string? Nom { get; set; }
}

public partial class FicFamTransfertModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CodFam { get; set; } = null!;

    public string? Nom { get; set; }
}
