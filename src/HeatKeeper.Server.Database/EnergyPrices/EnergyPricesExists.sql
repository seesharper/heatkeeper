select exists(
    select 1
    from EnergyPrices eg
    where EnergyPriceAreaId = 1 AND TimeStart >= @Date AND TimeStart < date(@Date, '+1 day')) as EnergyPricesExists