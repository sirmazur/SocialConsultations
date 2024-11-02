﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialConsultations.DbContexts;

#nullable disable

namespace SocialConsultations.Migrations
{
    [DbContext(typeof(ConsultationsContext))]
    [Migration("20241102220603_create_communities")]
    partial class create_communities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CommentUser", b =>
                {
                    b.Property<int>("UpvotedCommentsId")
                        .HasColumnType("int");

                    b.Property<int>("UpvotesId")
                        .HasColumnType("int");

                    b.HasKey("UpvotedCommentsId", "UpvotesId");

                    b.HasIndex("UpvotesId");

                    b.ToTable("CommentUser");
                });

            modelBuilder.Entity("CommunityUser", b =>
                {
                    b.Property<int>("AdminCommunitiesId")
                        .HasColumnType("int");

                    b.Property<int>("AdministratorsId")
                        .HasColumnType("int");

                    b.HasKey("AdminCommunitiesId", "AdministratorsId");

                    b.HasIndex("AdministratorsId");

                    b.ToTable("CommunityUser");
                });

            modelBuilder.Entity("CommunityUser1", b =>
                {
                    b.Property<int>("MemberCommunitiesId")
                        .HasColumnType("int");

                    b.Property<int>("MembersId")
                        .HasColumnType("int");

                    b.HasKey("MemberCommunitiesId", "MembersId");

                    b.HasIndex("MembersId");

                    b.ToTable("CommunityUser1");
                });

            modelBuilder.Entity("SocialConsultations.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SocialConsultations.Entities.Community", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AvatarId")
                        .HasColumnType("int");

                    b.Property<int?>("BackgroundId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("AvatarId");

                    b.HasIndex("BackgroundId");

                    b.ToTable("Communities");
                });

            modelBuilder.Entity("SocialConsultations.Entities.FileData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("FileData");
                });

            modelBuilder.Entity("SocialConsultations.Entities.Issue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.ToTable("Issues");
                });

            modelBuilder.Entity("SocialConsultations.Entities.JoinRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("UserId");

                    b.ToTable("JoinRequest");
                });

            modelBuilder.Entity("SocialConsultations.Entities.Poll", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("SocialConsultations.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AvatarId")
                        .HasColumnType("int");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ConfirmationCode")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastPasswordReminder")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("AvatarId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CommentUser", b =>
                {
                    b.HasOne("SocialConsultations.Entities.Comment", null)
                        .WithMany()
                        .HasForeignKey("UpvotedCommentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialConsultations.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UpvotesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CommunityUser", b =>
                {
                    b.HasOne("SocialConsultations.Entities.Community", null)
                        .WithMany()
                        .HasForeignKey("AdminCommunitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialConsultations.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("AdministratorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CommunityUser1", b =>
                {
                    b.HasOne("SocialConsultations.Entities.Community", null)
                        .WithMany()
                        .HasForeignKey("MemberCommunitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialConsultations.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialConsultations.Entities.Community", b =>
                {
                    b.HasOne("SocialConsultations.Entities.FileData", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarId");

                    b.HasOne("SocialConsultations.Entities.FileData", "Background")
                        .WithMany()
                        .HasForeignKey("BackgroundId");

                    b.Navigation("Avatar");

                    b.Navigation("Background");
                });

            modelBuilder.Entity("SocialConsultations.Entities.Issue", b =>
                {
                    b.HasOne("SocialConsultations.Entities.Community", null)
                        .WithMany("Issues")
                        .HasForeignKey("CommunityId");
                });

            modelBuilder.Entity("SocialConsultations.Entities.JoinRequest", b =>
                {
                    b.HasOne("SocialConsultations.Entities.Community", "Community")
                        .WithMany("JoinRequests")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SocialConsultations.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialConsultations.Entities.User", b =>
                {
                    b.HasOne("SocialConsultations.Entities.FileData", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarId");

                    b.Navigation("Avatar");
                });

            modelBuilder.Entity("SocialConsultations.Entities.Community", b =>
                {
                    b.Navigation("Issues");

                    b.Navigation("JoinRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
