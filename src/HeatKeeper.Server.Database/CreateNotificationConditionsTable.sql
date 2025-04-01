-- auto-generated definition
create table NotificationConditions
(
    Id             INTEGER
        constraint NotificationConditions_pk
            primary key,
    NotificationId INTEGER not null
        references Notifications,
    Value          double  not null,
    ConditionType  INTEGER not null,
    OperatorType   INTEGER not null   
);

create unique index NotificationConditions_Id_uindex
    on NotificationConditions (Id);

