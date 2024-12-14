SELECT 
    Id,
    Name,
    EIC_Code,
    Description,
    VATRateId
FROM EnergyPriceAreas
WHERE Id = @Id