using System;
using System.Collections.Generic;

namespace TGRooms.tgtables;

public partial class Room
{
    public int Id { get; set; }

    public int Spravce { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Itemroom> Itemrooms { get; set; } = new List<Itemroom>();

    public virtual Spravce SpravceNavigation { get; set; } = null!;
}
