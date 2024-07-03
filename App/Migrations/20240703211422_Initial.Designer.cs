﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SimplifyCondoApi.Entity;

#nullable disable

namespace simplify_condo_api.Migrations
{
    [DbContext(typeof(SimplifyCondoApiDbContext))]
    [Migration("20240703211422_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SimplifyCondoApi.Entity.Role", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Name = "Tenant"
                        },
                        new
                        {
                            Name = "Manager"
                        });
                });

            modelBuilder.Entity("SimplifyCondoApi.Entity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SimplifyCondoApi.Entity.User", b =>
                {
                    b.HasOne("SimplifyCondoApi.Entity.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SimplifyCondoApi.Entity.Role", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
