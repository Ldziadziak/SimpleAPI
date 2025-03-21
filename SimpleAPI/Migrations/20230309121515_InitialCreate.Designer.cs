﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleAPI.DbContexts;

#nullable disable

namespace SimpleAPI.Migrations
{
    [DbContext(typeof(CustomerContext))]
    [Migration("20230309121515_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.2");

            modelBuilder.Entity("SimpleAPI.Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("customer");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Karsa",
                            Surname = "Orlong"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Anomander",
                            Surname = "Rake"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Onos",
                            Surname = "T'oolan"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
