﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFMdotnet.Database;

#nullable disable

namespace PFMdotnet.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class TransactionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PFMdotnet.Database.Entities.CategoryEntity", b =>
                {
                    b.Property<string>("Code")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ParentCode")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Code");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("PFMdotnet.Database.Entities.TransactionEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<string>("BeneficiaryName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("CatCode")
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Direction")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Kind")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("Mcc")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CatCode");

                    b.ToTable("transactions", (string)null);
                });

            modelBuilder.Entity("PFMdotnet.Database.Entities.TransactionEntity", b =>
                {
                    b.HasOne("PFMdotnet.Database.Entities.CategoryEntity", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CatCode");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("PFMdotnet.Database.Entities.CategoryEntity", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
