﻿// <auto-generated />
using System;
using Kcsara.Respond.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

namespace Kcsara.Respond.Data.Migrations.SqliteMigrations
{
    [DbContext(typeof(SqliteRespondDbContext))]
    partial class SqliteRespondDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("Kcsara.Respond.Data.ActivityRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<long>("Created")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("EndTime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Number")
                        .HasColumnType("TEXT");

                    b.Property<long>("StartTime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<long>("Updated")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Updater")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("Kcsara.Respond.Data.RespondingUnitRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<long?>("Activated")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ActivityId")
                        .HasColumnType("TEXT");

                    b.Property<string>("KnownUnitId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long>("Requested")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Updated")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Updater")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.ToTable("RespondingUnits");
                });

            modelBuilder.Entity("Kcsara.Respond.Data.ActivityRow", b =>
                {
                    b.OwnsOne("Kcsara.Respond.Data.ActivityLocation", "Location", b1 =>
                        {
                            b1.Property<Guid>("ActivityRowId")
                                .HasColumnType("TEXT");

                            b1.Property<Geometry>("Geometry")
                                .HasColumnType("GEOMETRY");

                            b1.Property<string>("Name")
                                .HasColumnType("TEXT");

                            b1.Property<string>("PropertiesJson")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Wkid")
                                .HasColumnType("TEXT");

                            b1.HasKey("ActivityRowId");

                            b1.ToTable("Activities");

                            b1.WithOwner()
                                .HasForeignKey("ActivityRowId");
                        });

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Kcsara.Respond.Data.RespondingUnitRow", b =>
                {
                    b.HasOne("Kcsara.Respond.Data.ActivityRow", "Activity")
                        .WithMany("Units")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");
                });

            modelBuilder.Entity("Kcsara.Respond.Data.ActivityRow", b =>
                {
                    b.Navigation("Units");
                });
#pragma warning restore 612, 618
        }
    }
}
