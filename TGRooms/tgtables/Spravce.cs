using System;
using System.Collections.Generic;

namespace TGRooms.tgtables;

public partial class Spravce
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

}
