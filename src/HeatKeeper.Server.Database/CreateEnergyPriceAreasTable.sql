-- auto-generated definition
create table EnergyPriceAreas
(
    Id          INTEGER not null
        constraint EnergyPriceArea_pk
            primary key autoincrement,
    EIC_Code    TEXT    not null,
    Name        TEXT    not null,
    Description TEXT    not null,
    DisplayOrder INTEGER default 0 not null,
    VATRateId   INTEGER
        constraint EnergyPriceArea_VATRates_Id_fk
            references VATRates
);

create unique index EnergyPriceArea_EIC_Code_uindex
    on EnergyPriceAreas (EIC_Code);

create unique index EnergyPriceArea_Name_uindex
    on EnergyPriceAreas (Name);

