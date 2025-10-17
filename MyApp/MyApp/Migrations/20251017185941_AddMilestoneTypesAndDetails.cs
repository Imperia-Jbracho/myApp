using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMilestoneTypesAndDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "TravelMilestones",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "TravelMilestones",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<string>(
                name: "BookingPlatform",
                table: "TravelMilestones",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CheckInDate",
                table: "TravelMilestones",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CheckOutDate",
                table: "TravelMilestones",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classification",
                table: "TravelMilestones",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DurationHours",
                table: "TravelMilestones",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationUrl",
                table: "TravelMilestones",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NightlyRate",
                table: "TravelMilestones",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Nights",
                table: "TravelMilestones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReservationDate",
                table: "TravelMilestones",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TravelMilestones",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingPlatform",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "CheckOutDate",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "Classification",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "LocationUrl",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "NightlyRate",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "Nights",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "ReservationDate",
                table: "TravelMilestones");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TravelMilestones");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "TravelMilestones",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "TravelMilestones",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);
        }
    }
}
