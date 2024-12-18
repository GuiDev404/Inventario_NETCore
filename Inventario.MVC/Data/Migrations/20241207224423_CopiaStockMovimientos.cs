﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class CopiaStockMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Movimientos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Movimientos");
        }
    }
}
