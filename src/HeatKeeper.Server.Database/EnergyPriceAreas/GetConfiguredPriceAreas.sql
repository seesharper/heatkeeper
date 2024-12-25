SELECT
    epa.Id as EnergyPriceAreaId,
    epa.EIC_Code,
    vr.Rate as VATRate
FROM
    EnergyPriceAreas epa
INNER JOIN
    VatRates vr ON epa.VatRateId = vr.Id