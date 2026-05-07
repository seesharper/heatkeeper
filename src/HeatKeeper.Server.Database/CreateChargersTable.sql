create table Chargers
(
    Id           integer
        constraint Chargers_pk
            primary key autoincrement,
    ZoneId       integer not null
        constraint FK_Chargers_Zones
            references Zones,
    Name         text    not null,
    Description  text,
    MqttTopic    text,
    OnPayload    text,
    OffPayload   text,
    ChargerState integer not null default 0
);
