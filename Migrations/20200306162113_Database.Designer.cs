﻿// <auto-generated />
using System;
using APICasadeshow.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace APICasadeshow.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200306162113_Database")]
    partial class Database
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("APICasadeshow.Models.CasaDeShow", b =>
                {
                    b.Property<int>("CasaDeShowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Endereco")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("CasaDeShowId");

                    b.ToTable("CasaDeShow");
                });

            modelBuilder.Entity("APICasadeshow.Models.Compra", b =>
                {
                    b.Property<int>("CompraId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("EventoId")
                        .HasColumnType("int");

                    b.Property<int>("QtdIngressos")
                        .HasColumnType("int");

                    b.Property<float>("Total")
                        .HasColumnType("float");

                    b.Property<int?>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("CompraId");

                    b.HasIndex("EventoId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Compra");
                });

            modelBuilder.Entity("APICasadeshow.Models.Evento", b =>
                {
                    b.Property<int>("EventoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Capacidade")
                        .HasColumnType("int");

                    b.Property<int?>("CasaDeShowId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime(6)");

                    b.Property<byte[]>("Foto")
                        .HasColumnType("longblob");

                    b.Property<int?>("GeneroId")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<float>("PrecoIngresso")
                        .HasColumnType("float");

                    b.Property<int>("QtdIngresso")
                        .HasColumnType("int");

                    b.HasKey("EventoId");

                    b.HasIndex("CasaDeShowId");

                    b.HasIndex("GeneroId");

                    b.ToTable("Evento");
                });

            modelBuilder.Entity("APICasadeshow.Models.Genero", b =>
                {
                    b.Property<int>("GeneroId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("GeneroId");

                    b.ToTable("Genero");
                });

            modelBuilder.Entity("APICasadeshow.Models.Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Role")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Senha")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UsuarioId");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("APICasadeshow.Models.Compra", b =>
                {
                    b.HasOne("APICasadeshow.Models.Evento", "Evento")
                        .WithMany()
                        .HasForeignKey("EventoId");

                    b.HasOne("APICasadeshow.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");
                });

            modelBuilder.Entity("APICasadeshow.Models.Evento", b =>
                {
                    b.HasOne("APICasadeshow.Models.CasaDeShow", "CasaDeShow")
                        .WithMany()
                        .HasForeignKey("CasaDeShowId");

                    b.HasOne("APICasadeshow.Models.Genero", "Genero")
                        .WithMany()
                        .HasForeignKey("GeneroId");
                });
#pragma warning restore 612, 618
        }
    }
}
