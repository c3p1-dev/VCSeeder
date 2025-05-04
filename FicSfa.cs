using System;
using System.Collections.Generic;

namespace carnetDb.Models;

public partial class FicSfa
{
    public string CodSfa { get; set; } = null!;

    public string? Nom { get; set; }

    public string? CodFam { get; set; }
}

public partial class FicSfaTransfertModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CodSfa { get; set; } = null!;

    public string? Nom { get; set; }

    public string? CodFam { get; set; }
}