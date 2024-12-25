insert into EnergyPrices (PriceInLocalCurrency, PriceInLocalCurrencyAfterSubsidy, PriceInEuro, TimeStart, TimeEnd, Currency, ExchangeRate, VATRate,
                          EnergyPriceAreaId)
VALUES (@PriceInLocalCurrency, @PriceInLocalCurrencyAfterSubsidy, @PriceInEuro, @TimeStart, @TimeEnd, @Currency, @ExchangeRate, @VATRate, @EnergyPriceAreaId)                          