-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               11.7.2-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             12.10.0.7000
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for air_monitor_db
CREATE DATABASE IF NOT EXISTS `air_monitor_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_uca1400_ai_ci */;
USE `air_monitor_db`;

-- Dumping structure for table air_monitor_db.data
CREATE TABLE IF NOT EXISTS `data` (
  `ID` int(64) NOT NULL AUTO_INCREMENT,
  `RoomName` varchar(32) NOT NULL DEFAULT '0',
  `Temperature` float NOT NULL DEFAULT 0,
  `Humidity` float NOT NULL DEFAULT 0,
  `Radon` float NOT NULL DEFAULT 0,
  `PPM` float NOT NULL DEFAULT 0,
  `AirQuality` float NOT NULL DEFAULT 0,
  `DeviceID` int(11) NOT NULL DEFAULT 0,
  `TimeStamp` timestamp NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `FK1_DeviceID` (`DeviceID`),
  CONSTRAINT `FK1_DeviceID` FOREIGN KEY (`DeviceID`) REFERENCES `devices` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table air_monitor_db.devices
CREATE TABLE IF NOT EXISTS `devices` (
  `ID` int(128) NOT NULL AUTO_INCREMENT,
  `IsActive` tinyint(1) NOT NULL DEFAULT 0,
  `Firmware` varchar(32) NOT NULL DEFAULT '0',
  `MAC` varchar(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table air_monitor_db.user
CREATE TABLE IF NOT EXISTS `user` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Email` text NOT NULL,
  `Password` varchar(256) NOT NULL DEFAULT '',
  `IsAdmin` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table air_monitor_db.userdevices
CREATE TABLE IF NOT EXISTS `userdevices` (
  `ID` int(16) NOT NULL AUTO_INCREMENT,
  `UserID` int(16) NOT NULL DEFAULT 0,
  `DeviceID` int(16) NOT NULL DEFAULT 0,
  PRIMARY KEY (`ID`),
  KEY `UserID` (`UserID`),
  KEY `fk_DeviceID` (`DeviceID`),
  CONSTRAINT `fk_DeviceID` FOREIGN KEY (`DeviceID`) REFERENCES `devices` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_UserID` FOREIGN KEY (`UserID`) REFERENCES `user` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
