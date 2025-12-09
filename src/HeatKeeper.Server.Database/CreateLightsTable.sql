create table Lights
(
    Id          integer
        constraint Lights_pk
            primary key autoincrement,
    ZoneId      integer not null
        constraint FK_Lights_Zones
            references Zones,
    Name        text    not null,
    Description text,
    MqttTopic   text,
    OnPayload   text,
    OffPayload  text,
    Enabled     integer not null default 1
);
