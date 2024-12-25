SELECT 
    Id,
    Name,
    EIC_Code,
    Description,
    DisplayOrder,
    VATRateId
FROM EnergyPriceAreas
WHERE Id = @Id