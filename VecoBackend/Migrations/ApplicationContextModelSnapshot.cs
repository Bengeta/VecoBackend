﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VecoBackend.Data;

#nullable disable

namespace VecoBackend.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VecoBackend.Models.TaskModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("type")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("VecoBackend.Models.UserModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<bool>("isAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VecoBackend.Models.UserTaskModel", b =>
                {
                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.Property<int>("task_id")
                        .HasColumnType("integer");

                    b.Property<string>("photos")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("task_status")
                        .HasColumnType("integer");

                    b.Property<int>("taskid")
                        .HasColumnType("integer");

                    b.Property<int>("userid")
                        .HasColumnType("integer");

                    b.HasKey("user_id", "task_id");

                    b.HasIndex("taskid");

                    b.HasIndex("userid");

                    b.ToTable("UserTasks");
                });

            modelBuilder.Entity("VecoBackend.Models.UserTaskModel", b =>
                {
                    b.HasOne("VecoBackend.Models.TaskModel", "task")
                        .WithMany()
                        .HasForeignKey("taskid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VecoBackend.Models.UserModel", "user")
                        .WithMany()
                        .HasForeignKey("userid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("task");

                    b.Navigation("user");
                });
#pragma warning restore 612, 618
        }
    }
}
