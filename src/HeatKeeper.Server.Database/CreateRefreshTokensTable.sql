CREATE TABLE RefreshTokens (
    Id            INTEGER  PRIMARY KEY AUTOINCREMENT
                           NOT NULL,
    UserId        INTEGER  CONSTRAINT FK_REFRESHTOKENS_USERS REFERENCES Users (Id) 
                           NOT NULL,
    Token         TEXT     NOT NULL,
    Created       DATETIME NOT NULL
                           DEFAULT (CURRENT_TIMESTAMP),
    ExpiresInDays INTEGER  NOT NULL
);
