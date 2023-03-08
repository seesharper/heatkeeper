SELECT 
    z.MqttTopic as Topic,
    'ON' as OnPayLoad,
    'OFF' as OffPayLoad
FROM 
    Zones z    
WHERE 
    Id = @ZoneId