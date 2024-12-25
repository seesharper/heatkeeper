UPDATE EnergyPriceAreas
SET EIC_Code = @EIC_Code,
    Name = @Name,
    Description = @Description,
    DisplayOrder = @DisplayOrder,
    VATRateId = @VATRateId
WHERE Id = @Id