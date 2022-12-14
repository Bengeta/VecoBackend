// <auto-generated />
using System;
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

            modelBuilder.Entity("VecoBackend.Models.ImageStorageModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("imagePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("imageType")
                        .HasColumnType("integer");

                    b.Property<bool>("isUsed")
                        .HasColumnType("boolean");

                    b.Property<int>("userId")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("userId");

                    b.ToTable("ImageStorage");
                });

            modelBuilder.Entity("VecoBackend.Models.MaterialImageModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ImageId")
                        .HasColumnType("integer");

                    b.Property<int>("MaterialId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.HasIndex("MaterialId");

                    b.ToTable("MaterialImageModels");
                });

            modelBuilder.Entity("VecoBackend.Models.MaterialModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Category")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsSeen")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Materials");
                });

            modelBuilder.Entity("VecoBackend.Models.NotificationTokensModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("UserId");

                    b.ToTable("NotificationTokens");
                });

            modelBuilder.Entity("VecoBackend.Models.TaskImageModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<int>("UserTaskId")
                        .HasColumnType("integer");

                    b.Property<int>("imageId")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("UserTaskId");

                    b.HasIndex("imageId");

                    b.ToTable("TaskImage");
                });

            modelBuilder.Entity("VecoBackend.Models.TaskModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsSeen")
                        .HasColumnType("boolean");

                    b.Property<int>("Points")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("VecoBackend.Models.UserModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("points")
                        .HasColumnType("integer");

                    b.Property<string>("salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.HasIndex("email")
                        .IsUnique();

                    b.HasIndex("token")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VecoBackend.Models.UserTaskModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DeleteTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("TaskId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("taskStatus")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.HasIndex("UserId");

                    b.ToTable("UserTasks");
                });

            modelBuilder.Entity("VecoBackend.Models.ImageStorageModel", b =>
                {
                    b.HasOne("VecoBackend.Models.UserModel", "UserModel")
                        .WithMany("images")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserModel");
                });

            modelBuilder.Entity("VecoBackend.Models.MaterialImageModel", b =>
                {
                    b.HasOne("VecoBackend.Models.ImageStorageModel", "ImageStorageModel")
                        .WithMany("MaterialImageModel")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VecoBackend.Models.MaterialModel", "MaterialModel")
                        .WithMany("Images")
                        .HasForeignKey("MaterialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ImageStorageModel");

                    b.Navigation("MaterialModel");
                });

            modelBuilder.Entity("VecoBackend.Models.NotificationTokensModel", b =>
                {
                    b.HasOne("VecoBackend.Models.UserModel", "user")
                        .WithMany("notificationTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("VecoBackend.Models.TaskImageModel", b =>
                {
                    b.HasOne("VecoBackend.Models.UserTaskModel", "UserTask")
                        .WithMany("task_images")
                        .HasForeignKey("UserTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VecoBackend.Models.ImageStorageModel", "ImageStorage")
                        .WithMany("TaskImageModel")
                        .HasForeignKey("imageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ImageStorage");

                    b.Navigation("UserTask");
                });

            modelBuilder.Entity("VecoBackend.Models.UserTaskModel", b =>
                {
                    b.HasOne("VecoBackend.Models.TaskModel", "task")
                        .WithMany("userTasks")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VecoBackend.Models.UserModel", "user")
                        .WithMany("userTasks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("task");

                    b.Navigation("user");
                });

            modelBuilder.Entity("VecoBackend.Models.ImageStorageModel", b =>
                {
                    b.Navigation("MaterialImageModel");

                    b.Navigation("TaskImageModel");
                });

            modelBuilder.Entity("VecoBackend.Models.MaterialModel", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("VecoBackend.Models.TaskModel", b =>
                {
                    b.Navigation("userTasks");
                });

            modelBuilder.Entity("VecoBackend.Models.UserModel", b =>
                {
                    b.Navigation("images");

                    b.Navigation("notificationTokens");

                    b.Navigation("userTasks");
                });

            modelBuilder.Entity("VecoBackend.Models.UserTaskModel", b =>
                {
                    b.Navigation("task_images");
                });
#pragma warning restore 612, 618
        }
    }
}
