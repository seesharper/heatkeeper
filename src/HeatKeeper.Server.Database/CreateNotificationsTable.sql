-- auto-generated definition
create table Notifications
(
    Id               INTEGER not null
        constraint NotificationSubscriptions_pk
            primary key,
    UserId           INTEGER not null
        references Users,
    NotificationType INTEGER not null,
    LastSent         datetime,
    Enabled          INTEGER default 1 not null,
    CronExpression   TEXT,
    HoursToSnooze    INTEGER default 0 not null,
    Name             TEXT    default 'dsfd' not null,
    Description      TEXT
);

create unique index Notifications_Id_uindex
    on Notifications (Id);
