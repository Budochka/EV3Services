CREATE DATABASE `ev3` /*!40100 DEFAULT CHARACTER SET utf8 */;

-- ev3.Events definition

CREATE TABLE `Events` (
  `Time` datetime NOT NULL,
  `Topic` varchar(100) NOT NULL,
  `Data` longblob DEFAULT NULL,
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;