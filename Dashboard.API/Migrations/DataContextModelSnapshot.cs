﻿// <auto-generated />
using System;
using Dashboard.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dashboard.API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("Dashboard.API.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Country");

                    b.Property<float>("Latitude");

                    b.Property<float>("Longitude");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Dashboard.API.Models.KanbanProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Notes");

                    b.Property<int?>("ParentUserId");

                    b.HasKey("Id");

                    b.HasIndex("ParentUserId");

                    b.ToTable("KanbanProjects");
                });

            modelBuilder.Entity("Dashboard.API.Models.KanbanStory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Notes");

                    b.Property<int?>("ParentProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ParentProjectId");

                    b.ToTable("KanbanStories");
                });

            modelBuilder.Entity("Dashboard.API.Models.KanbanTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime>("DueDate");

                    b.Property<string>("Name");

                    b.Property<string>("Notes");

                    b.Property<int?>("ParentStoryId");

                    b.Property<int>("Priority");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("ParentStoryId");

                    b.ToTable("KanbanTasks");
                });

            modelBuilder.Entity("Dashboard.API.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("ParentUserId");

                    b.HasKey("Id");

                    b.HasIndex("ParentUserId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Dashboard.API.Models.SettingField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("ParentSettingId");

                    b.Property<string>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ParentSettingId");

                    b.ToTable("SettingFields");
                });

            modelBuilder.Entity("Dashboard.API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("EmailAddress");

                    b.Property<DateTime>("LastActive");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<int>("UserLevel");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Dashboard.API.Models.KanbanProject", b =>
                {
                    b.HasOne("Dashboard.API.Models.User", "ParentUser")
                        .WithMany()
                        .HasForeignKey("ParentUserId");
                });

            modelBuilder.Entity("Dashboard.API.Models.KanbanStory", b =>
                {
                    b.HasOne("Dashboard.API.Models.KanbanProject", "ParentProject")
                        .WithMany("Stories")
                        .HasForeignKey("ParentProjectId");
                });

            modelBuilder.Entity("Dashboard.API.Models.KanbanTask", b =>
                {
                    b.HasOne("Dashboard.API.Models.KanbanStory", "ParentStory")
                        .WithMany("Tasks")
                        .HasForeignKey("ParentStoryId");
                });

            modelBuilder.Entity("Dashboard.API.Models.Setting", b =>
                {
                    b.HasOne("Dashboard.API.Models.User", "ParentUser")
                        .WithMany("UserSettings")
                        .HasForeignKey("ParentUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Dashboard.API.Models.SettingField", b =>
                {
                    b.HasOne("Dashboard.API.Models.Setting", "ParentSetting")
                        .WithMany("Fields")
                        .HasForeignKey("ParentSettingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
