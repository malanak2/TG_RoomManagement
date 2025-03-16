using System;
using System.Collections.Generic;

namespace TGRooms.tgtables;

public partial class Itemroom : IEquatable<Itemroom>, IComparable<Itemroom>
{
    public int Id { get; set; }

    public int Room { get; set; }

    public int Item { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public virtual Item ItemNavigation { get; set; } = null!;

    public virtual Room RoomNavigation { get; set; } = null!;
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Itemroom objAsPart = obj as Itemroom;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }
    public int SortByNameAscending(string name1, string name2)
    {

        return name1.CompareTo(name2);
    }

    // Default comparer for Part type.
    public int CompareTo(Itemroom? comparePart)
    {
        // A null value means that this object is greater.
        if (comparePart == null)
            return 1;

        else
        {
            return this.ValidFrom.CompareTo(comparePart.ValidFrom);
        }
    }

    public bool Equals(Itemroom? other)
    {
        if (other == null) return false;
        return this.ValidFrom.CompareTo(other.ValidFrom) == 0;
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        return $"Id: {Id} Room: {Room} Item: {Item} ValidFrom: {ValidFrom.ToString()} ValidTo: {ValidTo.ToString()}";
    }
}
