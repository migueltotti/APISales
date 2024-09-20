﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sales.Infrastructure.Context;

#nullable disable

namespace Sales.Infrastructure.Migrations
{
    [DbContext(typeof(SalesDbContext))]
    [Migration("20240920163423_Affiliate and UserPoints")]
    partial class AffiliateandUserPoints
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("OrderProduct", b =>
                {
                    b.Property<int>("OrdersOrderId")
                        .HasColumnType("int");

                    b.Property<int>("ProductsProductId")
                        .HasColumnType("int");

                    b.HasKey("OrdersOrderId", "ProductsProductId");

                    b.HasIndex("ProductsProductId");

                    b.ToTable("OrderProduct");
                });

            modelBuilder.Entity("Sales.Domain.Models.Affiliate", b =>
                {
                    b.Property<int>("AffiliateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("AffiliateId"));

                    b.Property<decimal>("Discount")
                        .HasPrecision(4, 2)
                        .HasColumnType("decimal(4,2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("AffiliateId");

                    b.ToTable("Affiliate", (string)null);

                    b.HasData(
                        new
                        {
                            AffiliateId = 1,
                            Discount = 0.00m,
                            Name = "Nenhuma Afiliacao"
                        });
                });

            modelBuilder.Entity("Sales.Domain.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("CategoryId");

                    b.ToTable("Category", (string)null);

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            ImageUrl = "carnes-bovinas.jpg",
                            Name = "Carnes Bovinas"
                        },
                        new
                        {
                            CategoryId = 2,
                            ImageUrl = "produtos-diversos.jpg",
                            Name = "Produtos Diversos"
                        },
                        new
                        {
                            CategoryId = 3,
                            ImageUrl = "aves.jpg",
                            Name = "Aves"
                        },
                        new
                        {
                            CategoryId = 4,
                            ImageUrl = "carnes-suinas.jpg",
                            Name = "Carnes Suinas"
                        });
                });

            modelBuilder.Entity("Sales.Domain.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalValue")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("Order", (string)null);

                    b.HasData(
                        new
                        {
                            OrderId = 1,
                            OrderDate = new DateTime(2024, 9, 19, 15, 50, 45, 0, DateTimeKind.Unspecified),
                            OrderStatus = 2,
                            TotalValue = 10.00m,
                            UserId = 1
                        },
                        new
                        {
                            OrderId = 2,
                            OrderDate = new DateTime(2024, 9, 20, 15, 50, 45, 0, DateTimeKind.Unspecified),
                            OrderStatus = 1,
                            TotalValue = 20.00m,
                            UserId = 2
                        },
                        new
                        {
                            OrderId = 3,
                            OrderDate = new DateTime(2024, 9, 19, 15, 51, 39, 0, DateTimeKind.Unspecified),
                            OrderStatus = 1,
                            TotalValue = 30.00m,
                            UserId = 1
                        },
                        new
                        {
                            OrderId = 4,
                            OrderDate = new DateTime(2024, 9, 19, 15, 53, 36, 0, DateTimeKind.Unspecified),
                            OrderStatus = 2,
                            TotalValue = 40.00m,
                            UserId = 2
                        });
                });

            modelBuilder.Entity("Sales.Domain.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(175)
                        .HasColumnType("varchar(175)");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("StockQuantity")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(80)
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int>("TypeValue")
                        .HasColumnType("int");

                    b.Property<decimal>("Value")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product", (string)null);

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CategoryId = 2,
                            Description = "Coca Cola 250ml garrafinha",
                            ImageUrl = "coca-cola-250.jpg",
                            Name = "Coca-Cola 250",
                            StockQuantity = 10,
                            TypeValue = 2,
                            Value = 3.5m
                        },
                        new
                        {
                            ProductId = 2,
                            CategoryId = 2,
                            Description = "Pão Caseiro feito no dia",
                            ImageUrl = "pao-caseiro.jpg",
                            Name = "Pão Caseiro",
                            StockQuantity = 3,
                            TypeValue = 2,
                            Value = 9.9m
                        },
                        new
                        {
                            ProductId = 3,
                            CategoryId = 1,
                            Description = "Picanha",
                            ImageUrl = "picanha.jpg",
                            Name = "Picanha",
                            StockQuantity = 5,
                            TypeValue = 1,
                            Value = 69.99m
                        },
                        new
                        {
                            ProductId = 4,
                            CategoryId = 2,
                            Description = "TesteProduto",
                            ImageUrl = "TesteProduto.jpg",
                            Name = "Teste Produto",
                            StockQuantity = 10,
                            TypeValue = 1,
                            Value = 10.00m
                        },
                        new
                        {
                            ProductId = 5,
                            CategoryId = 3,
                            Description = "Teste2Produto",
                            ImageUrl = "Teste2.jpg",
                            Name = "Teste2",
                            StockQuantity = 10,
                            TypeValue = 2,
                            Value = 69.99m
                        });
                });

            modelBuilder.Entity("Sales.Domain.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserId"));

                    b.Property<int>("AffiliateId")
                        .HasColumnType("int");

                    b.Property<string>("Cpf")
                        .HasMaxLength(14)
                        .HasColumnType("varchar(14)");

                    b.Property<DateTime>("DateBirth")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<decimal>("Points")
                        .HasPrecision(6, 2)
                        .HasColumnType("decimal(6,2)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("AffiliateId");

                    b.ToTable("User", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            AffiliateId = 1,
                            Cpf = "111.111.111-11",
                            DateBirth = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "migueltotti2005@gmail.com",
                            Name = "Miguel Totti de Oliveira",
                            Password = "testemiguel",
                            Points = 0.00m,
                            Role = 2
                        },
                        new
                        {
                            UserId = 2,
                            AffiliateId = 1,
                            Cpf = "222.222.222-22",
                            DateBirth = new DateTime(2, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "isadorapaludeto15@gmail.com",
                            Name = "Isadora Leao Paludeto",
                            Password = "testeisadora",
                            Points = 0.00m,
                            Role = 2
                        },
                        new
                        {
                            UserId = 31,
                            AffiliateId = 1,
                            Cpf = "331.331.331-31",
                            DateBirth = new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "testeadmin@gmail.com",
                            Name = "TesteAdmin",
                            Password = "testeadmin",
                            Points = 0.00m,
                            Role = 2
                        },
                        new
                        {
                            UserId = 32,
                            AffiliateId = 1,
                            Cpf = "332.332.332-32",
                            DateBirth = new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "testeemployee@gmail.com",
                            Name = "TesteEmployee",
                            Password = "testeemployee",
                            Points = 0.00m,
                            Role = 1
                        },
                        new
                        {
                            UserId = 33,
                            AffiliateId = 1,
                            Cpf = "333.333.333-33",
                            DateBirth = new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "testecustomer@gmail.com",
                            Name = "TesteCustomer",
                            Password = "testecustomer",
                            Points = 0.00m,
                            Role = 0
                        });
                });

            modelBuilder.Entity("OrderProduct", b =>
                {
                    b.HasOne("Sales.Domain.Models.Order", null)
                        .WithMany()
                        .HasForeignKey("OrdersOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sales.Domain.Models.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Sales.Domain.Models.Order", b =>
                {
                    b.HasOne("Sales.Domain.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sales.Domain.Models.Product", b =>
                {
                    b.HasOne("Sales.Domain.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Sales.Domain.Models.User", b =>
                {
                    b.HasOne("Sales.Domain.Models.Affiliate", "Affiliate")
                        .WithMany("Users")
                        .HasForeignKey("AffiliateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Affiliate");
                });

            modelBuilder.Entity("Sales.Domain.Models.Affiliate", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Sales.Domain.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Sales.Domain.Models.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
