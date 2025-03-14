using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoscowWeatherArchives.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "weather_archives",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    upload_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weather_archives", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "weather_dates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    weather_archive_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weather_dates", x => x.id);
                    table.ForeignKey(
                        name: "fk_weather_dates_weather_archives_weather_archive_id",
                        column: x => x.weather_archive_id,
                        principalTable: "weather_archives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weather_measurements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    temperature = table.Column<double>(type: "double precision", nullable: true),
                    relative_humidity = table.Column<byte>(type: "smallint", nullable: true),
                    dew_point = table.Column<double>(type: "double precision", nullable: true),
                    atmospheric_pressure = table.Column<int>(type: "integer", nullable: true),
                    wind_direction = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    wind_speed = table.Column<int>(type: "integer", nullable: true),
                    cloudiness = table.Column<byte>(type: "smallint", nullable: true),
                    cloud_base_height = table.Column<int>(type: "integer", nullable: true),
                    vertical_visibility = table.Column<int>(type: "integer", nullable: true),
                    weather_phenomena = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    weather_date_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weather_measurements", x => x.id);
                    table.ForeignKey(
                        name: "fk_weather_measurements_weather_dates_weather_date_id",
                        column: x => x.weather_date_id,
                        principalTable: "weather_dates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_weather_dates_weather_archive_id",
                table: "weather_dates",
                column: "weather_archive_id");

            migrationBuilder.CreateIndex(
                name: "ix_weather_measurements_weather_date_id",
                table: "weather_measurements",
                column: "weather_date_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weather_measurements");

            migrationBuilder.DropTable(
                name: "weather_dates");

            migrationBuilder.DropTable(
                name: "weather_archives");
        }
    }
}
