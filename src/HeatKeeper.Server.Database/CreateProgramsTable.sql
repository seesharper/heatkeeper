CREATE TABLE Programs (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT
                             NOT NULL,
    LocationId       INTEGER CONSTRAINT FK_PROGRAMS_LOCATIONS REFERENCES Locations (Id) 
                             NOT NULL,
    ActiveScheduleId INTEGER,
    Name             TEXT    NOT NULL UNIQUE
);