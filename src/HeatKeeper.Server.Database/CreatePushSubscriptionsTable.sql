CREATE TABLE PushSubscriptions (
    Endpoint text NOT NULL constraint PushSubscriptions_pk primary key,
    P256DH text NOT NULL,
    Auth text NOT NULL,
    UserId INTEGER constraint FK_PUSHSUBSCRIPTIONS_USERS references Users,
    LastSeen DATETIME
)