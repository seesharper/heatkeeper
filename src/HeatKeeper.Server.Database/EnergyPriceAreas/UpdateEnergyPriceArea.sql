UPDATE EnergyPriceAreas
SET EIC_Code = @EIC_Code,
    Name = @Name,
    Description = @Description,
    VATRateId = @VATRateId
WHERE Id = @Id