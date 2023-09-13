using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RtChat.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class lastseen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "lastSeen",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastSeen",
                table: "AspNetUsers");
        }
    }
}
