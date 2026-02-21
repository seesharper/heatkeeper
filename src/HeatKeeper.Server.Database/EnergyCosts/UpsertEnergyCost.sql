INSERT INTO EnergyCosts (SensorId, PowerImport, CostInLocalCurrency, CostInLocalCurrencyAfterSubsidy, CostInLocalCurrencyWithFixedPrice, Hour)
VALUES (@SensorId, @PowerImport, @CostInLocalCurrency, @CostInLocalCurrencyAfterSubsidy, @CostInLocalCurrencyWithFixedPrice, @Hour)
ON CONFLICT(SensorId, Hour) DO UPDATE SET
    PowerImport = @PowerImport,
    CostInLocalCurrency = @CostInLocalCurrency,
    CostInLocalCurrencyAfterSubsidy = @CostInLocalCurrencyAfterSubsidy,
    CostInLocalCurrencyWithFixedPrice = @CostInLocalCurrencyWithFixedPrice;
