
USE tgtables;
CREATE TABLE `room` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `spravce` int NOT NULL,
  `name` varchar(30) NOT NULL
);

CREATE TABLE `spravce` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `surname` varchar(255) NOT NULL
);

CREATE TABLE `item` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `name` varchar(255) UNIQUE NOT NULL,
  `acquired` date NOT NULL,
  `sold` date DEFAULT null
);

CREATE TABLE `itemValue` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `item_id` int NOT NULL,
  `value` int NOT NULL,
  `valid_from` date NOT NULL,
  `valid_to` date DEFAULT null
);

CREATE TABLE `ItemRoom` (
  `id` int PRIMARY KEY AUTO_INCREMENT,
  `room` int NOT NULL,
  `item` int NOT NULL,
  `valid_from` date NOT NULL,
  `valid_to` date DEFAULT null
);

ALTER TABLE `room` ADD FOREIGN KEY (`spravce`) REFERENCES `spravce` (`id`);

ALTER TABLE `itemValue` ADD FOREIGN KEY (`item_id`) REFERENCES `item` (`id`);

ALTER TABLE `ItemRoom` ADD FOREIGN KEY (`room`) REFERENCES `room` (`id`);

ALTER TABLE `ItemRoom` ADD FOREIGN KEY (`item`) REFERENCES `item` (`id`);

USE tgtables;

INSERT INTO spravce (id, name, surname) VALUES
(1, 'John', 'Doe'),
(2, 'Jane', 'Smith'),
(3, 'Alice', 'Brown');

INSERT INTO room (id, spravce, name) VALUES
(1, 1, 'Conference Room A'),
(2, 2, 'Lecture Hall B'),
(3, 3, 'Meeting Room C');

INSERT INTO item (id, name, acquired, sold) VALUES
(1, 'Laptop', '2023-01-10', NULL),
(2, 'Projector', '2022-11-05', NULL),
(3, 'Whiteboard', '2021-06-20', '2023-12-15');

INSERT INTO itemValue (id, item_id, value, valid_from, valid_to) VALUES
(1, 1, 1500, '2023-01-10', '2023-06-01'),
(2, 1, 1200, '2023-06-02', NULL),
(3, 2, 800, '2022-11-05', '2023-03-01'),
(4, 2, 750, '2023-03-02', NULL),
(5, 3, 300, '2021-06-20', '2022-12-01'),
(6, 3, 250, '2022-12-02', '2023-12-15');

INSERT INTO ItemRoom (id, room, item, valid_from, valid_to) VALUES
(1, 1, 1, '2023-01-10', NULL),
(2, 2, 2, '2022-11-05', '2023-09-01'),
(3, 3, 2, '2023-09-02', NULL),
(4, 3, 3, '2021-06-20', '2023-12-15');
