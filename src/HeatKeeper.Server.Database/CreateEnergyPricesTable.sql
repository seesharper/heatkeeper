-- auto-generated definition
create table EnergyPrices
(
    Id                   INTEGER  not null
        constraint EnergyPrices_pk
            primary key autoincrement,
    PriceInLocalCurrency NUMERIC  not null,
    PriceInEuro          NUMERIC  not null,
    TimeStart            DATETIME not null,
    TimeEnd              DATETIME not null,
    ExchangeRate         NUMERIC  not null,
    VATRate              numeric,
    EnergyPriceAreaId    INTEGER  not null
        references EnergyPriceAreas
);

