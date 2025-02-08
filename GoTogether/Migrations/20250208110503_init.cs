using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoTogether.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizationTokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_token = table.Column<string>(type: "text", nullable: false),
                    c_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationTokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_country_id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    n_latitude = table.Column<double>(type: "double precision", nullable: false),
                    n_longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationCodes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_email = table.Column<string>(type: "text", nullable: false),
                    n_code = table.Column<int>(type: "integer", nullable: false),
                    d_expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationCodes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    n_latitude = table.Column<double>(type: "double precision", nullable: false),
                    n_longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Landmarks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_country_id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_city_id = table.Column<Guid>(type: "uuid", nullable: true),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: false),
                    c_address = table.Column<string>(type: "text", nullable: false),
                    n_latitude = table.Column<double>(type: "double precision", nullable: false),
                    n_longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Landmarks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TripCities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_city_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripCities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TripCountries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_country_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripCountries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TripLandmarks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_landmark_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripLandmarks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TripRoles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_dev_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: true),
                    b_is_can_edit = table.Column<bool>(type: "boolean", nullable: false),
                    b_is_can_delete = table.Column<bool>(type: "boolean", nullable: false),
                    b_is_can_invite = table.Column<bool>(type: "boolean", nullable: false),
                    b_is_can_banish = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripRoles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_dev_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_nickname = table.Column<string>(type: "text", nullable: false),
                    c_email = table.Column<string>(type: "text", nullable: false),
                    c_password = table.Column<string>(type: "text", nullable: false),
                    d_registration_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    f_authorization_token = table.Column<Guid>(type: "uuid", nullable: false),
                    f_role = table.Column<Guid>(type: "uuid", nullable: false),
                    UserRoleid = table.Column<Guid>(type: "uuid", nullable: false),
                    b_is_mail_confirmed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_AuthorizationTokens_f_authorization_token",
                        column: x => x.f_authorization_token,
                        principalTable: "AuthorizationTokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_UserRoleid",
                        column: x => x.UserRoleid,
                        principalTable: "UserRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_f_role",
                        column: x => x.f_role,
                        principalTable: "UserRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: true),
                    d_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    d_end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    f_author = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.id);
                    table.ForeignKey(
                        name: "FK_Trips_Users_f_author",
                        column: x => x.f_author,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TripInvites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripInvites", x => x.id);
                    table.ForeignKey(
                        name: "FK_TripInvites_Trips_f_trip_id",
                        column: x => x.f_trip_id,
                        principalTable: "Trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTrips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Userid = table.Column<Guid>(type: "uuid", nullable: false),
                    f_trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tripid = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_trip_role = table.Column<Guid>(type: "uuid", nullable: false),
                    TripRoleid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTrips", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserTrips_TripRoles_TripRoleid",
                        column: x => x.TripRoleid,
                        principalTable: "TripRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTrips_TripRoles_f_user_trip_role",
                        column: x => x.f_user_trip_role,
                        principalTable: "TripRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTrips_Trips_Tripid",
                        column: x => x.Tripid,
                        principalTable: "Trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTrips_Trips_f_trip_id",
                        column: x => x.f_trip_id,
                        principalTable: "Trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTrips_Users_Userid",
                        column: x => x.Userid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTrips_Users_f_user_id",
                        column: x => x.f_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripInvites_f_trip_id",
                table: "TripInvites",
                column: "f_trip_id");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_f_author",
                table: "Trips",
                column: "f_author");

            migrationBuilder.CreateIndex(
                name: "IX_Users_f_authorization_token",
                table: "Users",
                column: "f_authorization_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_f_role",
                table: "Users",
                column: "f_role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserRoleid",
                table: "Users",
                column: "UserRoleid");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_f_trip_id",
                table: "UserTrips",
                column: "f_trip_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_f_user_id",
                table: "UserTrips",
                column: "f_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_f_user_trip_role",
                table: "UserTrips",
                column: "f_user_trip_role");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_Tripid",
                table: "UserTrips",
                column: "Tripid");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_TripRoleid",
                table: "UserTrips",
                column: "TripRoleid");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_Userid",
                table: "UserTrips",
                column: "Userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ConfirmationCodes");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Landmarks");

            migrationBuilder.DropTable(
                name: "TripCities");

            migrationBuilder.DropTable(
                name: "TripCountries");

            migrationBuilder.DropTable(
                name: "TripInvites");

            migrationBuilder.DropTable(
                name: "TripLandmarks");

            migrationBuilder.DropTable(
                name: "UserTrips");

            migrationBuilder.DropTable(
                name: "TripRoles");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AuthorizationTokens");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
