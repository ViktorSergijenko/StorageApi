using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StorageAPI.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminLogTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Where = table.Column<string>(nullable: true),
                    What = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminLogTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    WhoCreated = table.Column<string>(nullable: true),
                    HasAbilityToLoad = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogNameDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogNameDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    HasProblems = table.Column<bool>(nullable: false),
                    HasMinCatalogs = table.Column<bool>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    QrCodeBase64 = table.Column<string>(nullable: true),
                    ImageBase64 = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baskets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSettingsDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CanDeleteUsers = table.Column<bool>(nullable: false),
                    CanEditUserPassword = table.Column<bool>(nullable: false),
                    CanAddProductsManually = table.Column<bool>(nullable: false),
                    CanEditUserBaskets = table.Column<bool>(nullable: false),
                    CanEditUserInformation = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettingsDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettingsDB_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    ShortDescription = table.Column<string>(nullable: false),
                    WarehouseId = table.Column<Guid>(nullable: false),
                    FixedProblem = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    AcceptedFix = table.Column<bool>(nullable: false),
                    AuthorAcceptedFix = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FixedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsDB_WarehouseDB_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "WarehouseDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimpleLogTableDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    Where = table.Column<string>(nullable: true),
                    What = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    WarehouseId = table.Column<Guid>(nullable: false),
                    Manually = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleLogTableDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimpleLogTableDB_WarehouseDB_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "WarehouseDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWarehouseDB",
                columns: table => new
                {
                    WarehouseId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWarehouseDB", x => new { x.UserId, x.WarehouseId });
                    table.ForeignKey(
                        name: "FK_UserWarehouseDB_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWarehouseDB_WarehouseDB_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "WarehouseDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CatalogNameId = table.Column<Guid>(nullable: false),
                    CurrentAmount = table.Column<decimal>(nullable: false),
                    MaximumAmount = table.Column<int>(nullable: false),
                    MinimumAmount = table.Column<int>(nullable: false),
                    ProductPrice = table.Column<decimal>(nullable: false),
                    WarehouseId = table.Column<Guid>(nullable: true),
                    BasketId = table.Column<Guid>(nullable: true),
                    Type = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogDB_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogDB_CatalogNameDB_CatalogNameId",
                        column: x => x.CatalogNameId,
                        principalTable: "CatalogNameDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogDB_WarehouseDB_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "WarehouseDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsCommentDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    NewsId = table.Column<Guid>(nullable: false),
                    Comment = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCommentDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsCommentDB_NewsDB_NewsId",
                        column: x => x.NewsId,
                        principalTable: "NewsDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PricePerOne = table.Column<decimal>(nullable: false),
                    VendorCode = table.Column<string>(nullable: true),
                    CatalogId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDB_CatalogDB_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "CatalogDB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FullName",
                table: "AspNetUsers",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_UserId",
                table: "Baskets",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogDB_BasketId",
                table: "CatalogDB",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogDB_WarehouseId",
                table: "CatalogDB",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogDB_CatalogNameId_WarehouseId",
                table: "CatalogDB",
                columns: new[] { "CatalogNameId", "WarehouseId" },
                unique: true,
                filter: "[WarehouseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogNameDB_Name",
                table: "CatalogNameDB",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsCommentDB_NewsId",
                table: "NewsCommentDB",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsDB_WarehouseId",
                table: "NewsDB",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDB_CatalogId",
                table: "ProductDB",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleLogTableDB_WarehouseId",
                table: "SimpleLogTableDB",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettingsDB_UserId",
                table: "UserSettingsDB",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserWarehouseDB_WarehouseId",
                table: "UserWarehouseDB",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDB_Name_Address",
                table: "WarehouseDB",
                columns: new[] { "Name", "Address" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminLogTable");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "NewsCommentDB");

            migrationBuilder.DropTable(
                name: "ProductDB");

            migrationBuilder.DropTable(
                name: "SimpleLogTableDB");

            migrationBuilder.DropTable(
                name: "UserSettingsDB");

            migrationBuilder.DropTable(
                name: "UserWarehouseDB");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "NewsDB");

            migrationBuilder.DropTable(
                name: "CatalogDB");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "CatalogNameDB");

            migrationBuilder.DropTable(
                name: "WarehouseDB");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
