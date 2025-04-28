# Kolejarz

Tworzenie Bazy danych kolejarz
```sql
CREATE DATABASE kolejarz;
USE kolejarz;
```
Tworzenie tabeli stacje
```sql
CREATE TABLE stations (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,
    posX INT NOT NULL,
    posY INT NOT NULL,
    station_order INT NOT NULL
);
```
Tworzenie tabeli trasy
```sql
CREATE TABLE routes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    from_station_id INT,
    to_station_id INT,
    departure_time TIME,
    arrival_time TIME,
    FOREIGN KEY (from_station_id) REFERENCES stations(id),
    FOREIGN KEY (to_station_id) REFERENCES stations(id)
);
```
Insert danych do tabel
```sql
INSERT INTO stations (name, posX, posY, station_order) VALUES 
('Warszawa Centralna', 158, 67, 1),
('Warszawa Wschodnia', 309, 67, 2),
('Mińsk Mazowiecki', 420, 394, 3),
('Siedlce', 520, 189, 4),
('Łuków', 616, 73, 5),
('Międzyrzec Podlaski', 645, 316, 6),
('Biała Podlaska', 672, 394, 7),
('Terespol', 146, 390, 8),
('Brześć', 129, 272, 9);

INSERT INTO routes (from_station_id, to_station_id, departure_time, arrival_time) VALUES
(1, 2, '08:00:00', '08:15:00'),
(2, 3, '08:20:00', '09:00:00'),
(3, 4, '09:05:00', '09:45:00'),
(4, 5, '09:50:00', '10:30:00'),
(5, 6, '10:35:00', '11:15:00'),
(6, 7, '11:20:00', '12:00:00'),
(7, 8, '12:05:00', '12:45:00'),
(8, 9, '12:50:00', '13:30:00');
```
