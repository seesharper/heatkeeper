create table Heaters
(
    Id          integer
        constraint Heaters_pk
            primary key autoincrement,
    ZoneId      integer not null
        constraint FK_Heaters_Zones
            references Zones,
    Name        text    not null,
    Description text,
    MqttTopic   text,
    OnPayload   text,
    OffPayload  text
);