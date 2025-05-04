using System;
using System.Collections.Generic;

namespace carnetDb.Models;

public partial class FicCpt
{
    public string CodCpt { get; set; } = null!;

    public string? Nom { get; set; }

    public string? Visible { get; set; }
}

public partial class FicCptTransfertModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CodCpt { get; set; } = null!;

    public string? Nom { get; set; }

    public string? Visible { get; set; }
}
