﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using instaProj.Models;

#nullable disable

namespace instaProj.Migrations
{
    [DbContext(typeof(Contexto))]
    partial class ContextoModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("instaProj.Models.Archive", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("DatePub")
                        .HasColumnType("datetime2");

                    b.Property<string>("Desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FK_Comments_User_Id")
                        .HasColumnType("int");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameLocal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Private")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Archives");
                });

            modelBuilder.Entity("instaProj.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Commeting")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ContLike")
                        .HasColumnType("int");

                    b.Property<bool>("Ocult")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("instaProj.Models.Follow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("User_FollowedId")
                        .HasColumnType("int");

                    b.Property<int?>("User_FollowingId")
                        .HasColumnType("int");

                    b.Property<int>("User_Id_Followed")
                        .HasColumnType("int");

                    b.Property<int>("User_Id_Following")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_FollowedId");

                    b.HasIndex("User_FollowingId");

                    b.ToTable("Follow");
                });

            modelBuilder.Entity("instaProj.Models.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Archive_Id")
                        .HasColumnType("int");

                    b.Property<int>("Comment_Id")
                        .HasColumnType("int");

                    b.Property<int>("FK_Comments_User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("instaProj.Models.SubComment", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("Arc_Id")
                        .HasColumnType("int");

                    b.Property<int?>("ArchiveId")
                        .HasColumnType("int");

                    b.Property<int>("Comment_Id")
                        .HasColumnType("int");

                    b.Property<string>("Commeting")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FK_Comments_User_Id")
                        .HasColumnType("int");

                    b.Property<bool>("Ocult")
                        .HasColumnType("bit");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArchiveId");

                    b.HasIndex("UserId");

                    b.ToTable("SubComments");
                });

            modelBuilder.Entity("instaProj.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("instaProj.Models.Archive", b =>
                {
                    b.HasOne("instaProj.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("instaProj.Models.Comment", b =>
                {
                    b.HasOne("instaProj.Models.Archive", "Archive")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("instaProj.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Archive");

                    b.Navigation("User");
                });

            modelBuilder.Entity("instaProj.Models.Follow", b =>
                {
                    b.HasOne("instaProj.Models.User", "User_Followed")
                        .WithMany()
                        .HasForeignKey("User_FollowedId");

                    b.HasOne("instaProj.Models.User", "User_Following")
                        .WithMany()
                        .HasForeignKey("User_FollowingId");

                    b.Navigation("User_Followed");

                    b.Navigation("User_Following");
                });

            modelBuilder.Entity("instaProj.Models.SubComment", b =>
                {
                    b.HasOne("instaProj.Models.Archive", "Archive")
                        .WithMany()
                        .HasForeignKey("ArchiveId");

                    b.HasOne("instaProj.Models.Comment", "Comment")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("instaProj.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Archive");

                    b.Navigation("Comment");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
