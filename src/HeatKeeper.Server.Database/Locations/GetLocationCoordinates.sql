select id,
       name,
       latitude,
       longitude
  from locations
 where longitude is not null
   and latitude is not null