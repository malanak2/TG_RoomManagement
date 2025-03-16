using System;
using System.Collections.Generic;

namespace TGRooms.tgtables;

public partial class Itemvalue
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int Value { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public virtual Item Item { get; set; } = null!;
}
