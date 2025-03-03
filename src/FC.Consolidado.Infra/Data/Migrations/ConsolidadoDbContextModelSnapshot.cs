﻿// <auto-generated />
using System;
using FC.Consolidado.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FC.Consolidado.Infra.Data.Migrations
{
    [DbContext(typeof(ConsolidadoDbContext))]
    partial class ConsolidadoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FC.Consolidado.Domain.Entities.Transacao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DataHora")
                        .HasColumnType("timestamptz");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<decimal>("Valor")
                        .HasColumnType("numeric(10,2)");

                    b.HasKey("Id");

                    b.ToTable("Transacoes", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
