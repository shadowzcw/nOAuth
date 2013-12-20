CREATE DATABASE  IF NOT EXISTS `opcenter` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `opcenter`;
-- MySQL dump 10.13  Distrib 5.5.16, for Win32 (x86)
--
-- Host: test001    Database: opcenter
-- ------------------------------------------------------
-- Server version	5.5.30

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `oauth_client`
--

DROP TABLE IF EXISTS `oauth_client`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oauth_client` (
  `ClientId` int(11) NOT NULL AUTO_INCREMENT,
  `ClientIdentifier` varchar(100) NOT NULL,
  `ClientSecret` varchar(100) DEFAULT NULL,
  `Callback` varchar(8000) DEFAULT NULL,
  `Name` varchar(8000) NOT NULL,
  `ClientType` int(11) NOT NULL,
  PRIMARY KEY (`ClientId`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

INSERT INTO `oauth_client` VALUES (1,'samplewebapiconsumer','samplesecret',NULL,'Some sample client',0),
(2,'nios_0b0c8368c3054d1c898c23a20c10c7f6','4a69a9a677fb4bf9bed57c443a9c8e3e','','Some sample client used for implicit grants (no secret)',1),
(3,'nandroid_fb3ec459bef54359b34cc66cedd4ffc0','a3fc7b361f95458391344e8517a3518d',NULL,'Some sample client used for implicit grants (no secret)',1),
(4,'ntest_5cdf2af64a9e460aa96e4ac631ed16d9','cd3ebe9fd4ca4f7b866c3f9b205ae338',NULL,'Some sample client used for implicit grants (no secret)',1);

--
-- Table structure for table `oauth_nonce`
--

DROP TABLE IF EXISTS `oauth_nonce`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oauth_nonce` (
  `Context` varchar(8000) NOT NULL,
  `Code` varchar(8000) NOT NULL,
  `Timestamp` datetime NOT NULL,
  PRIMARY KEY (`Context`(255),`Code`(255),`Timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `oauth_clientauthorization`
--

DROP TABLE IF EXISTS `oauth_clientauthorization`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oauth_clientauthorization` (
  `AuthorizationId` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedOnUtc` datetime NOT NULL,
  `ClientId` int(11) NOT NULL,
  `UserId` int(11) DEFAULT NULL,
  `Scope` longtext,
  `ExpirationDateUtc` datetime DEFAULT NULL,
  PRIMARY KEY (`AuthorizationId`),
  KEY `Client_ClientAuthorization` (`ClientId`),
  KEY `User_ClientAuthorization` (`UserId`),
  CONSTRAINT `Client_ClientAuthorization` FOREIGN KEY (`ClientId`) REFERENCES `oauth_client` (`ClientId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `User_ClientAuthorization` FOREIGN KEY (`UserId`) REFERENCES `sys_user` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `oauth_symmetriccryptokey`
--

DROP TABLE IF EXISTS `oauth_symmetriccryptokey`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oauth_symmetriccryptokey` (
  `Bucket` varchar(8000) NOT NULL,
  `Handle` varchar(8000) NOT NULL,
  `ExpiresUtc` datetime NOT NULL,
  `Secret` mediumblob NOT NULL,
  PRIMARY KEY (`Bucket`(255),`Handle`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_user`
--

DROP TABLE IF EXISTS `sys_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_user` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(100) NOT NULL,
  `Password` varchar(200) NOT NULL,
  `FullName` varchar(200) NOT NULL,
  `HeadUrl` text,
  `Email` varchar(100) NOT NULL,
  `GroupID` int(11) DEFAULT NULL,
  `RoleID` int(11) NOT NULL,
  `Status` bit(1) NOT NULL DEFAULT b'0',
  `CreateUser` int(11) NOT NULL,
  `CreateTime` datetime NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;

INSERT INTO `sys_user` VALUES (1,'zhangsan','96e79218965eb72c92a549dd5a330112','ÕÅÈý','2-20130716105801.jpg','zhangsan@qq.com',1,0,'',1,'2013-06-19 16:55:30');
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-12-12 15:52:40
