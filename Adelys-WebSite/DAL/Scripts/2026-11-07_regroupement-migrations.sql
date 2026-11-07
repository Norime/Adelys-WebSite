-- =====================================================================================
-- Regroupement des migrations du site (07/11/2026)
--
-- Les dix migrations de la refonte (InitSite ... AddSiteSettings) ont été remplacées par une
-- seule migration, 20261107201915_InitialCreate. Ce script met une base existante en accord :
--   1. il crée les tables qui manquent encore, selon les anciennes migrations déjà appliquées ;
--   2. il remplace l'historique __site_migrations par la seule ligne InitialCreate.
-- Les données (comptes, gazettes, commerces...) ne sont pas touchées.
--
-- À lancer UNE FOIS sur la base du site, AVANT de démarrer la nouvelle version du site :
--   mysql -h ADRESSE -u UTILISATEUR -p NOM_BASE_SITE < 2026-11-07_regroupement-migrations.sql
-- Il peut être relancé sans risque. Une base neuve n'en a pas besoin : le site la crée seul.
-- =====================================================================================

-- 1. Tables manquantes, d'après les anciennes migrations
CREATE TABLE IF NOT EXISTS `__site_migrations` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___site_migrations` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261106215452_InitSite') THEN

    CREATE TABLE `site_users` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Username` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `PasswordHash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Role` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `MustChangePassword` tinyint(1) NOT NULL,
        `SecurityStamp` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `FailedLoginCount` int NOT NULL,
        `LockoutEndUtc` datetime(6) NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `LastLoginUtc` datetime(6) NULL,
        CONSTRAINT `PK_site_users` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261106215452_InitSite') THEN

    CREATE UNIQUE INDEX `IX_site_users_Username` ON `site_users` (`Username`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261106215452_InitSite') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261106215452_InitSite', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261106233041_AddGazettes') THEN

    CREATE TABLE `site_gazettes` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Title` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `Slug` varchar(170) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(500) CHARACTER SET utf8mb4 NULL,
        `PublishedOn` date NOT NULL,
        `PdfFileName` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `PdfSizeBytes` bigint NOT NULL,
        `CoverFileName` varchar(64) CHARACTER SET utf8mb4 NULL,
        `UploadedBy` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        CONSTRAINT `PK_site_gazettes` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261106233041_AddGazettes') THEN

    CREATE UNIQUE INDEX `IX_site_gazettes_Slug` ON `site_gazettes` (`Slug`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261106233041_AddGazettes') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261106233041_AddGazettes', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107091320_AddShops') THEN

    CREATE TABLE `site_shops` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `City` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `Description` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `Owner` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `X` int NOT NULL,
        `Y` int NOT NULL,
        `Z` int NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        CONSTRAINT `PK_site_shops` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107091320_AddShops') THEN

    CREATE TABLE `site_shop_items` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ShopId` int NOT NULL,
        `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Price` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_site_shop_items` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_site_shop_items_site_shops_ShopId` FOREIGN KEY (`ShopId`) REFERENCES `site_shops` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107091320_AddShops') THEN

    CREATE TABLE `site_shop_types` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ShopId` int NOT NULL,
        `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_site_shop_types` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_site_shop_types_site_shops_ShopId` FOREIGN KEY (`ShopId`) REFERENCES `site_shops` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107091320_AddShops') THEN

    CREATE INDEX `IX_site_shop_items_ShopId` ON `site_shop_items` (`ShopId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107091320_AddShops') THEN

    CREATE INDEX `IX_site_shop_types_ShopId` ON `site_shop_types` (`ShopId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107091320_AddShops') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107091320_AddShops', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107095331_AddShopCategories') THEN

    CREATE TABLE `site_shop_categories` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_site_shop_categories` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107095331_AddShopCategories') THEN

    INSERT INTO `site_shop_categories` (`Id`, `Name`)
    VALUES (1, 'Matériaux de construction'),
    (2, 'Nourriture'),
    (3, 'Utilitaires'),
    (4, 'Redstone');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107095331_AddShopCategories') THEN

    CREATE UNIQUE INDEX `IX_site_shop_categories_Name` ON `site_shop_categories` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107095331_AddShopCategories') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107095331_AddShopCategories', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107101247_AddShopItemCatalog') THEN

    UPDATE `site_shop_items` SET `Price` = '1' WHERE `Price` NOT REGEXP '^[0-9]{1,9}$';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107101247_AddShopItemCatalog') THEN

    ALTER TABLE `site_shop_items` MODIFY COLUMN `Price` int NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107101247_AddShopItemCatalog') THEN

    ALTER TABLE `site_shop_items` ADD `Currency` varchar(10) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107101247_AddShopItemCatalog') THEN

    ALTER TABLE `site_shop_items` ADD `ItemId` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107101247_AddShopItemCatalog') THEN

    ALTER TABLE `site_shop_items` ADD `Quantity` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107101247_AddShopItemCatalog') THEN

    UPDATE `site_shop_items` SET `Quantity` = 1, `Currency` = 'cuivre';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107101247_AddShopItemCatalog') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107101247_AddShopItemCatalog', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE TABLE `site_laws` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Number` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `Content` varchar(5000) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_site_laws` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE TABLE `site_parliament_meetings` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Title` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `HeldOn` date NOT NULL,
        `Content` mediumtext CHARACTER SET utf8mb4 NOT NULL,
        `CreatedBy` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        CONSTRAINT `PK_site_parliament_meetings` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE TABLE `site_law_amendments` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `LawId` int NOT NULL,
        `AmendedOn` date NOT NULL,
        `Note` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_site_law_amendments` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_site_law_amendments_site_laws_LawId` FOREIGN KEY (`LawId`) REFERENCES `site_laws` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE TABLE `site_parliament_images` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `FileName` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `MeetingId` int NULL,
        `UploadedAtUtc` datetime(6) NOT NULL,
        CONSTRAINT `PK_site_parliament_images` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_site_parliament_images_site_parliament_meetings_MeetingId` FOREIGN KEY (`MeetingId`) REFERENCES `site_parliament_meetings` (`Id`) ON DELETE SET NULL
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    INSERT INTO `site_laws` (`Id`, `Content`, `Number`)
    VALUES (1, 'Pour accéder à la citoyenneté d''Adélys, il faut déposer une demande, qui sera examinée par le parlement.', '1'),
    (2, 'L''assassinat, l''attaque, la destruction et le vol de bien sont interdit.', '2'),
    (3, 'Pour chaque demandes (plainte, parcelle, construction, devenir membre du parlement...), il faut la déposer sur le discord. Elle seront examinées par le parlement.', '3'),
    (4, CONCAT('Les sanctions possibles lors d''un jugement sont :', CHAR(10), '', CHAR(10), '- l''avertissement', CHAR(10), '- la mort', CHAR(10), '- dommage et intérêt', CHAR(10), '- le bannissement'), '4'),
    (5, 'La monnaie utilisée est le Cuivre Brut, le Fer Brut et l''Or Brut (1 Or Brut = 9 Fer Brut / 1 Fer Brut = 9 Cuivre Brut)', '5'),
    (6, 'Il peut être demandé aux citoyens une participation exceptionnelle sous forme de main d''œuvre ou de matériaux lors de la mise en place de grands projets communs (ex: la banque, le parlement).', '6'),
    (7, 'Pour des raison de performance, il est interdit de posséder un cheptel d''animaux trop nombreux (20 individus pour chaque espèces), hormis exceptions validées par le parlement.', '7'),
    (8, 'Il est interdit de construire de manière non esthétique, de défigurer le paysage, de créer des mines à ciel ouvert sans accord du parlement.', '8'),
    (9, 'L''abus de pouvoir est interdit', '9'),
    (10, CONCAT('- Les portails du nether sont limités à 1 par colonie', CHAR(10), '- Le portail d''Adélys se trouve sous l''hôtel de ville', CHAR(10), '- La construction d''un portail doit obligatoirement faire l''objet d''une demande'), '10'),
    (11, CONCAT('Pour le commerce, certain prix sont fixes :', CHAR(10), '', CHAR(10), '- 16 lingots de fer = 5 lingots d''or', CHAR(10), '- 1 diamant = 3 lingots d''or', CHAR(10), '- 1 lingot de netherite = 40 lingots d''or'), '11'),
    (12, CONCAT('L''exploration ne peux se faire que sous certaines conditions :', CHAR(10), '', CHAR(10), '- L''exploration en ligne droite dans une direction est prohibé (exploration anarchique)', CHAR(10), '- L''exploration dois être mappée', CHAR(10), '- L''exploration doit se faire à côté d''une zone explorée'), '12'),
    (13, 'En cas d''abandon du serveur par un membre du parlement, ce dernier sera destituer de sa place passé un délai de 6 mois et sans indications contraires de sa part. En cas de retour sur le serveur la personne sera à nouveau citoyen et non membre du parlement.', '13'),
    (14, CONCAT('En cas d''un départ du serveur, les commerces et monopoles sont "gelés" et chacun peut revendiquer le monopole passé les 6 mois. La ville reprend le commerce durant les 6 mois et engage un citoyen pour la reprise éventuelle du commerce.', CHAR(10), '', CHAR(10), 'Les stocks précédents le départ des propriétaires seront stockés à la banque ou à l''hôtel de ville en cas d''un éventuel retour. La ville repartira avec un stock neuf. L''argent de ces 6 mois reviens à la ville mais la personne partenaire touchera un pourcentage.'), '13bis'),
    (15, 'Tout citoyen ayant quitté le serveur peux y revenir à tout moment hors bannissement. Les lois 13 et 13bis seront à appliquer pour la répartition des biens selon la durée du départ.', '14'),
    (16, 'Tout event organisé par un citoyen d''Adélys est fortement encouragé mais doit être approuvé par au minimum un membre du parlement.', '15');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    INSERT INTO `site_law_amendments` (`Id`, `AmendedOn`, `LawId`, `Note`)
    VALUES (1, DATE '2021-12-04', 5, 'changement de monnaie'),
    (2, DATE '2020-12-17', 6, 'nouvelle formulation');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE INDEX `IX_site_law_amendments_LawId` ON `site_law_amendments` (`LawId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE UNIQUE INDEX `IX_site_laws_Number` ON `site_laws` (`Number`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE UNIQUE INDEX `IX_site_parliament_images_FileName` ON `site_parliament_images` (`FileName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    CREATE INDEX `IX_site_parliament_images_MeetingId` ON `site_parliament_images` (`MeetingId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107155751_AddRules') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107155751_AddRules', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE TABLE `site_event_categories` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
        `Slug` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Description` mediumtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_site_event_categories` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE TABLE `site_events` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Title` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `Slug` varchar(170) CHARACTER SET utf8mb4 NOT NULL,
        `CategoryId` int NOT NULL,
        `StartsAt` datetime(6) NOT NULL,
        `EndsAt` datetime(6) NULL,
        `Location` varchar(150) CHARACTER SET utf8mb4 NULL,
        `Summary` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
        `Content` mediumtext CHARACTER SET utf8mb4 NULL,
        `CoverFileName` varchar(64) CHARACTER SET utf8mb4 NULL,
        `IsFeatured` tinyint(1) NOT NULL,
        `CreatedBy` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAtUtc` datetime(6) NOT NULL,
        `UpdatedAtUtc` datetime(6) NULL,
        CONSTRAINT `PK_site_events` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_site_events_site_event_categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `site_event_categories` (`Id`) ON DELETE RESTRICT
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE TABLE `site_event_images` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `FileName` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `EventId` int NULL,
        `CategoryId` int NULL,
        `UploadedAtUtc` datetime(6) NOT NULL,
        CONSTRAINT `PK_site_event_images` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_site_event_images_site_event_categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `site_event_categories` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_site_event_images_site_events_EventId` FOREIGN KEY (`EventId`) REFERENCES `site_events` (`Id`) ON DELETE SET NULL
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    INSERT INTO `site_event_categories` (`Id`, `Description`, `Name`, `Slug`)
    VALUES (1, NULL, 'Les jeux de la mort', 'jeux-de-la-mort'),
    (2, NULL, 'Les events saisonniers', 'saisonniers');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE UNIQUE INDEX `IX_site_event_categories_Slug` ON `site_event_categories` (`Slug`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE INDEX `IX_site_event_images_CategoryId` ON `site_event_images` (`CategoryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE INDEX `IX_site_event_images_EventId` ON `site_event_images` (`EventId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE UNIQUE INDEX `IX_site_event_images_FileName` ON `site_event_images` (`FileName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE INDEX `IX_site_events_CategoryId` ON `site_events` (`CategoryId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE UNIQUE INDEX `IX_site_events_Slug` ON `site_events` (`Slug`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    CREATE INDEX `IX_site_events_StartsAt` ON `site_events` (`StartsAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107171519_AddEvents') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107171519_AddEvents', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107175536_EventCategoryShowcase') THEN

    ALTER TABLE `site_event_categories` ADD `CoverFileName` varchar(64) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107175536_EventCategoryShowcase') THEN

    ALTER TABLE `site_event_categories` ADD `SortOrder` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107175536_EventCategoryShowcase') THEN

    ALTER TABLE `site_event_categories` ADD `Summary` varchar(500) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107175536_EventCategoryShowcase') THEN

    UPDATE `site_event_categories` SET `CoverFileName` = NULL, `SortOrder` = 2, `Summary` = 'Le principe des jeux de la mort, leurs règles et toutes les éditions passées.'
    WHERE `Id` = 1;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107175536_EventCategoryShowcase') THEN

    UPDATE `site_event_categories` SET `CoverFileName` = NULL, `SortOrder` = 1, `Summary` = 'Les events organisés au fil des saisons et toutes leurs éditions.'
    WHERE `Id` = 2;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107175536_EventCategoryShowcase') THEN

    INSERT INTO `site_event_categories` (`Id`, `CoverFileName`, `Description`, `Name`, `Slug`, `SortOrder`, `Summary`)
    VALUES (3, NULL, NULL, 'Les events spéciaux', 'speciaux', 3, 'Les events uniques, organisés pour une occasion particulière.');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107175536_EventCategoryShowcase') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107175536_EventCategoryShowcase', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107190204_AddHomeSlides') THEN

    CREATE TABLE `site_home_slides` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Title` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
        `Caption` varchar(300) CHARACTER SET utf8mb4 NULL,
        `ImageUrl` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `AltText` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `SortOrder` int NOT NULL,
        CONSTRAINT `PK_site_home_slides` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107190204_AddHomeSlides') THEN

    INSERT INTO `site_home_slides` (`Id`, `AltText`, `Caption`, `ImageUrl`, `SortOrder`, `Title`)
    VALUES (1, 'Le palais de Pristo, en cuivre oxydé et en bois', 'Quoi de mieux que rejoindre l''empire le plus puissant ?', '/content/Images/Home/palais-pristo.jpg', 1, 'Pristo, un empire de confiance !'),
    (2, 'Le Centre Adelys et son parking', 'Un problème ? Une erreur ? Dénoncez PiloteWho !', '/content/Images/Home/centre-adelys.jpg', 2, 'Le Centre Adelys');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107190204_AddHomeSlides') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107190204_AddHomeSlides', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107191714_AddSiteSettings') THEN

    CREATE TABLE `site_settings` (
        `Key` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `Value` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_site_settings` PRIMARY KEY (`Key`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__site_migrations` WHERE `MigrationId` = '20261107191714_AddSiteSettings') THEN

    INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107191714_AddSiteSettings', '9.0.22');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;


-- 2. Historique : les dix anciennes migrations deviennent InitialCreate
DELETE FROM `__site_migrations`;
INSERT INTO `__site_migrations` (`MigrationId`, `ProductVersion`)
VALUES ('20261107201915_InitialCreate', '9.0.22');
