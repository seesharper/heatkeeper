CREATE TABLE NotificationSubscriptions (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT
                           NOT NULL,
    UserId         INTEGER REFERENCES Users (Id) NOT NULL,
    NotificationId INTEGER REFERENCES Notifications (Id) NOT NULL
);