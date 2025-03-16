using System;
using System.Collections.Generic;

namespace TGRooms.tgtables;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Acquired { get; set; }

    public DateTime? Sold { get; set; }

    public virtual ICollection<Itemroom> Itemrooms { get; set; } = new List<Itemroom>();

    public virtual ICollection<Itemvalue> Itemvalues { get; set; } = new List<Itemvalue>();

}
