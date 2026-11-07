using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Adelys_WebSite.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_event_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Summary = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "mediumtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverFileName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_event_categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_gazettes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(170)", maxLength: 170, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublishedOn = table.Column<DateTime>(type: "date", nullable: false),
                    PdfFileName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PdfSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    CoverFileName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedBy = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_gazettes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_home_slides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Caption = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AltText = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_home_slides", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_laws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_laws", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_parliament_meetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HeldOn = table.Column<DateTime>(type: "date", nullable: false),
                    Content = table.Column<string>(type: "mediumtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_parliament_meetings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_settings", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_shop_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_shop_categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Owner = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Z = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_shops", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MustChangePassword = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FailedLoginCount = table.Column<int>(type: "int", nullable: false),
                    LockoutEndUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastLoginUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(170)", maxLength: 170, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Location = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Summary = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "mediumtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverFileName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFeatured = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_site_events_site_event_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "site_event_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_law_amendments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LawId = table.Column<int>(type: "int", nullable: false),
                    AmendedOn = table.Column<DateTime>(type: "date", nullable: false),
                    Note = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_law_amendments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_site_law_amendments_site_laws_LawId",
                        column: x => x.LawId,
                        principalTable: "site_laws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_parliament_images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeetingId = table.Column<int>(type: "int", nullable: true),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_parliament_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_site_parliament_images_site_parliament_meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "site_parliament_meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_shop_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_shop_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_site_shop_items_site_shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "site_shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_shop_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_shop_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_site_shop_types_site_shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "site_shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "site_event_images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_event_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_site_event_images_site_event_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "site_event_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_site_event_images_site_events_EventId",
                        column: x => x.EventId,
                        principalTable: "site_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "site_event_categories",
                columns: new[] { "Id", "CoverFileName", "Description", "Name", "Slug", "SortOrder", "Summary" },
                values: new object[,]
                {
                    { 1, null, null, "Les jeux de la mort", "jeux-de-la-mort", 2, "Le principe des jeux de la mort, leurs règles et toutes les éditions passées." },
                    { 2, null, null, "Les events saisonniers", "saisonniers", 1, "Les events organisés au fil des saisons et toutes leurs éditions." },
                    { 3, null, null, "Les events spéciaux", "speciaux", 3, "Les events uniques, organisés pour une occasion particulière." }
                });

            migrationBuilder.InsertData(
                table: "site_home_slides",
                columns: new[] { "Id", "AltText", "Caption", "ImageUrl", "SortOrder", "Title" },
                values: new object[,]
                {
                    { 1, "Le palais de Pristo, en cuivre oxydé et en bois", "Quoi de mieux que rejoindre l'empire le plus puissant ?", "/content/Images/Home/palais-pristo.jpg", 1, "Pristo, un empire de confiance !" },
                    { 2, "Le Centre Adelys et son parking", "Un problème ? Une erreur ? Dénoncez PiloteWho !", "/content/Images/Home/centre-adelys.jpg", 2, "Le Centre Adelys" }
                });

            migrationBuilder.InsertData(
                table: "site_laws",
                columns: new[] { "Id", "Content", "Number" },
                values: new object[,]
                {
                    { 1, "Pour accéder à la citoyenneté d'Adélys, il faut déposer une demande, qui sera examinée par le parlement.", "1" },
                    { 2, "L'assassinat, l'attaque, la destruction et le vol de bien sont interdit.", "2" },
                    { 3, "Pour chaque demandes (plainte, parcelle, construction, devenir membre du parlement...), il faut la déposer sur le discord. Elle seront examinées par le parlement.", "3" },
                    { 4, "Les sanctions possibles lors d'un jugement sont :\n\n- l'avertissement\n- la mort\n- dommage et intérêt\n- le bannissement", "4" },
                    { 5, "La monnaie utilisée est le Cuivre Brut, le Fer Brut et l'Or Brut (1 Or Brut = 9 Fer Brut / 1 Fer Brut = 9 Cuivre Brut)", "5" },
                    { 6, "Il peut être demandé aux citoyens une participation exceptionnelle sous forme de main d'œuvre ou de matériaux lors de la mise en place de grands projets communs (ex: la banque, le parlement).", "6" },
                    { 7, "Pour des raison de performance, il est interdit de posséder un cheptel d'animaux trop nombreux (20 individus pour chaque espèces), hormis exceptions validées par le parlement.", "7" },
                    { 8, "Il est interdit de construire de manière non esthétique, de défigurer le paysage, de créer des mines à ciel ouvert sans accord du parlement.", "8" },
                    { 9, "L'abus de pouvoir est interdit", "9" },
                    { 10, "- Les portails du nether sont limités à 1 par colonie\n- Le portail d'Adélys se trouve sous l'hôtel de ville\n- La construction d'un portail doit obligatoirement faire l'objet d'une demande", "10" },
                    { 11, "Pour le commerce, certain prix sont fixes :\n\n- 16 lingots de fer = 5 lingots d'or\n- 1 diamant = 3 lingots d'or\n- 1 lingot de netherite = 40 lingots d'or", "11" },
                    { 12, "L'exploration ne peux se faire que sous certaines conditions :\n\n- L'exploration en ligne droite dans une direction est prohibé (exploration anarchique)\n- L'exploration dois être mappée\n- L'exploration doit se faire à côté d'une zone explorée", "12" },
                    { 13, "En cas d'abandon du serveur par un membre du parlement, ce dernier sera destituer de sa place passé un délai de 6 mois et sans indications contraires de sa part. En cas de retour sur le serveur la personne sera à nouveau citoyen et non membre du parlement.", "13" },
                    { 14, "En cas d'un départ du serveur, les commerces et monopoles sont \"gelés\" et chacun peut revendiquer le monopole passé les 6 mois. La ville reprend le commerce durant les 6 mois et engage un citoyen pour la reprise éventuelle du commerce.\n\nLes stocks précédents le départ des propriétaires seront stockés à la banque ou à l'hôtel de ville en cas d'un éventuel retour. La ville repartira avec un stock neuf. L'argent de ces 6 mois reviens à la ville mais la personne partenaire touchera un pourcentage.", "13bis" },
                    { 15, "Tout citoyen ayant quitté le serveur peux y revenir à tout moment hors bannissement. Les lois 13 et 13bis seront à appliquer pour la répartition des biens selon la durée du départ.", "14" },
                    { 16, "Tout event organisé par un citoyen d'Adélys est fortement encouragé mais doit être approuvé par au minimum un membre du parlement.", "15" }
                });

            migrationBuilder.InsertData(
                table: "site_shop_categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Matériaux de construction" },
                    { 2, "Nourriture" },
                    { 3, "Utilitaires" },
                    { 4, "Redstone" }
                });

            migrationBuilder.InsertData(
                table: "site_law_amendments",
                columns: new[] { "Id", "AmendedOn", "LawId", "Note" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "changement de monnaie" },
                    { 2, new DateTime(2020, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "nouvelle formulation" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_site_event_categories_Slug",
                table: "site_event_categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_event_images_CategoryId",
                table: "site_event_images",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_site_event_images_EventId",
                table: "site_event_images",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_site_event_images_FileName",
                table: "site_event_images",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_events_CategoryId",
                table: "site_events",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_site_events_Slug",
                table: "site_events",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_events_StartsAt",
                table: "site_events",
                column: "StartsAt");

            migrationBuilder.CreateIndex(
                name: "IX_site_gazettes_Slug",
                table: "site_gazettes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_law_amendments_LawId",
                table: "site_law_amendments",
                column: "LawId");

            migrationBuilder.CreateIndex(
                name: "IX_site_laws_Number",
                table: "site_laws",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_parliament_images_FileName",
                table: "site_parliament_images",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_parliament_images_MeetingId",
                table: "site_parliament_images",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_site_shop_categories_Name",
                table: "site_shop_categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_shop_items_ShopId",
                table: "site_shop_items",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_site_shop_types_ShopId",
                table: "site_shop_types",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_site_users_Username",
                table: "site_users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_event_images");

            migrationBuilder.DropTable(
                name: "site_gazettes");

            migrationBuilder.DropTable(
                name: "site_home_slides");

            migrationBuilder.DropTable(
                name: "site_law_amendments");

            migrationBuilder.DropTable(
                name: "site_parliament_images");

            migrationBuilder.DropTable(
                name: "site_settings");

            migrationBuilder.DropTable(
                name: "site_shop_categories");

            migrationBuilder.DropTable(
                name: "site_shop_items");

            migrationBuilder.DropTable(
                name: "site_shop_types");

            migrationBuilder.DropTable(
                name: "site_users");

            migrationBuilder.DropTable(
                name: "site_events");

            migrationBuilder.DropTable(
                name: "site_laws");

            migrationBuilder.DropTable(
                name: "site_parliament_meetings");

            migrationBuilder.DropTable(
                name: "site_shops");

            migrationBuilder.DropTable(
                name: "site_event_categories");
        }
    }
}
