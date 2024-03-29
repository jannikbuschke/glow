﻿// <auto-generated />

using System;
using Glow.TokenCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Glow.Core.TokenCache.Migrations.SqlServer
{
    [DbContext(typeof(SqlServerTokenDataContext))]
    partial class SqlServerTokenDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Glow.TokenCache.UserToken", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("AccessToken")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("ExpiresIn")
                    .HasColumnType("int");

                b.Property<string>("RefreshToken")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("TokenType")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("GlowTokenCache");
            });
#pragma warning restore 612, 618
        }
    }
}