CREATE TABLE EventTriggers (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT
                       NOT NULL,
    Name       TEXT    NOT NULL UNIQUE,
    Definition TEXT    NOT NULL
);