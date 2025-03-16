using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TGRooms.tgtables;

public partial class TgtablesContext : DbContext
{
    public TgtablesContext()
    {
    }

    public TgtablesContext(DbContextOptions<TgtablesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Itemroom> Itemrooms { get; set; }

    public virtual DbSet<Itemvalue> Itemvalues { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Spravce> Spravces { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=localhost; User ID=root; Password=root; Database=tgtables");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("item");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Acquired)
                .HasColumnType("date")
                .HasColumnName("acquired");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Sold)
                .HasColumnType("date")
                .HasColumnName("sold");
        });

        modelBuilder.Entity<Itemroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("itemroom");

            entity.HasIndex(e => e.Item, "item");

            entity.HasIndex(e => e.Room, "room");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Item).HasColumnName("item");
            entity.Property(e => e.Room).HasColumnName("room");
            entity.Property(e => e.ValidFrom)
                .HasColumnType("date")
                .HasColumnName("valid_from");
            entity.Property(e => e.ValidTo)
                .HasColumnType("date")
                .HasColumnName("valid_to");

            entity.HasOne(d => d.ItemNavigation).WithMany(p => p.Itemrooms)
                .HasForeignKey(d => d.Item)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("itemroom_ibfk_2");

            entity.HasOne(d => d.RoomNavigation).WithMany(p => p.Itemrooms)
                .HasForeignKey(d => d.Room)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("itemroom_ibfk_1");
        });

        modelBuilder.Entity<Itemvalue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("itemvalue");

            entity.HasIndex(e => e.ItemId, "item_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ValidFrom)
                .HasColumnType("date")
                .HasColumnName("valid_from");
            entity.Property(e => e.ValidTo)
                .HasColumnType("date")
                .HasColumnName("valid_to");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Item).WithMany(p => p.Itemvalues)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("itemvalue_ibfk_1");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("room");

            entity.HasIndex(e => e.Spravce, "spravce");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.Spravce).HasColumnName("spravce");

            entity.HasOne(d => d.SpravceNavigation).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.Spravce)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("room_ibfk_1");
        });

        modelBuilder.Entity<Spravce>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("spravce");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
