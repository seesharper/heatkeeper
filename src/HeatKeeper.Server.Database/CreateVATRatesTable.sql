create table VATRates
(
    Id   INTEGER not null
        constraint VATRates_pk
            primary key autoincrement,
    Name TEXT    not null,
    Rate NUMERIC not null
);

create unique index VATRates_Id_uindex
    on VATRates (Id);