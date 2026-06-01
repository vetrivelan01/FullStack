using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FullStack.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Citizens",
                columns: table => new
                {
                    CitizenID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CitizenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citizens", x => x.CitizenID);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    LoginID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.LoginID);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitizenID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HouseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_Addresses_Citizens_CitizenID",
                        column: x => x.CitizenID,
                        principalTable: "Citizens",
                        principalColumn: "CitizenID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitizenID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Documents_Citizens_CitizenID",
                        column: x => x.CitizenID,
                        principalTable: "Citizens",
                        principalColumn: "CitizenID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CitizenRecords",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "40000, 1"),
                    CitizenID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressID = table.Column<int>(type: "int", nullable: false),
                    DocumentID = table.Column<int>(type: "int", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRecords", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_CitizenRecords_Addresses_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Addresses",
                        principalColumn: "AddressID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitizenRecords_Citizens_CitizenID",
                        column: x => x.CitizenID,
                        principalTable: "Citizens",
                        principalColumn: "CitizenID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitizenRecords_Documents_DocumentID",
                        column: x => x.DocumentID,
                        principalTable: "Documents",
                        principalColumn: "DocumentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CitizenID",
                table: "Addresses",
                column: "CitizenID");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRecords_AddressID",
                table: "CitizenRecords",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRecords_CitizenID",
                table: "CitizenRecords",
                column: "CitizenID");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRecords_DocumentID",
                table: "CitizenRecords",
                column: "DocumentID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CitizenID",
                table: "Documents",
                column: "CitizenID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitizenRecords");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Citizens");
        }
    }
}
