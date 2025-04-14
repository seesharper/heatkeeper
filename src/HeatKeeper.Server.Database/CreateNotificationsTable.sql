-- auto-generated definition
create table Notifications
(
    Id               INTEGER not null
        constraint NotificationSubscriptions_pk
            primary key,    
    NotificationType INTEGER not null,
    LastSent         datetime,    
    CronExpression   TEXT,
    HoursToSnooze    INTEGER default 0 not null,
    Name             TEXT not null,
    Description      TEXT
);

create unique index Notifications_Id_uindex
    on Notifications (Id);
