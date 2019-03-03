﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ozon.Examination.Service.Persistence;

namespace Ozon.Examination.Service.Migrations
{
    [DbContext(typeof(RateDbContext))]
    partial class RatesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Ozon.Examination.Service.Persistence.Entities.Rate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(3);

                    b.Property<DateTime>("Date");

                    b.Property<decimal>("Value");

                    b.HasKey("Id");

                    b.ToTable("Rate");
                });
#pragma warning restore 612, 618
        }
    }
}
