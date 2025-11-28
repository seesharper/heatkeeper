select (
   select value
     from measurements
    where measurementtype = 11
) as activepowerimport,
       (
          select value
            from measurements
           where measurementtype = 6
       ) as currentphase1,
       (
          select value
            from measurements
           where measurementtype = 7
       ) as currentphase2,
       (
          select value
            from measurements
           where measurementtype = 8
       ) as currentphase3,
       (
          select value
            from measurements
           where measurementtype = 9
       ) as voltagebetweenphase1andphase2,
       (
          select value
            from measurements
           where measurementtype = 10
       ) as voltagebetweenphase1andphase3,
       (
          select value
            from measurements
           where measurementtype = 11
       ) as voltagebetweenphase2andphase3,
       (
          select value
            from measurements
           where measurementtype = 12
       ) as cumulativepowerimport