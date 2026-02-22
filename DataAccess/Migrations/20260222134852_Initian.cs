using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    TimeDeltaSeconds = table.Column<double>(type: "double precision", nullable: false),
                    FirstOperationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AverageExecutionTime = table.Column<double>(type: "double precision", nullable: false),
                    AverageValue = table.Column<double>(type: "double precision", nullable: false),
                    MedianValue = table.Column<double>(type: "double precision", nullable: false),
                    MaxValue = table.Column<double>(type: "double precision", nullable: false),
                    MinValue = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.UniqueConstraint("AK_Results_FileName", x => x.FileName);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionTime = table.Column<double>(type: "double precision", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Values_Results_FileName",
                        column: x => x.FileName,
                        principalTable: "Results",
                        principalColumn: "FileName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_AverageExecutionTime",
                table: "Results",
                column: "AverageExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_Results_AverageValue",
                table: "Results",
                column: "AverageValue");

            migrationBuilder.CreateIndex(
                name: "IX_Results_FileName",
                table: "Results",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_FirstOperationDate",
                table: "Results",
                column: "FirstOperationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Values_FileName",
                table: "Values",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_Values_FileName_Date",
                table: "Values",
                columns: new[] { "FileName", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
