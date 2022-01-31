﻿using Microsoft.EntityFrameworkCore.Migrations;
using BC = BCrypt.Net.BCrypt;

namespace Strawberry_Shortcake.Migrations
{
    public partial class user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPw = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activation = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserNo);
                });

            migrationBuilder.InsertData(table: "User", columns:  new[] { 
                "UserNo",
                "UserEmail",
                "UserName",
                "UserPw",
                "Activation",
            }, values: new object[] {
                1,
                "abcd95751@gmail.com",
                string.Empty,
                BC.HashPassword("1q2w3e$R"),
                true,
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
