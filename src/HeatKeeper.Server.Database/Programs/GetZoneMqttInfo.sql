SELECT 
    z.MqttTopic as Topic,
    'on' as OnPayLoad,
    'off' as OffPayLoad
FROM 
    Zones z    
WHERE 
    Id = @ZoneId