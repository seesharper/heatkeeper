select (
   select value
     from latestzonemeasurements
    where measurementtype = 5
) as activepowerimport,
       (
          select value
            from latestzonemeasurements
           where measurementtype = 6
       ) as currentphase1,
       (
          select value
            from latestzonemeasurements
           where measurementtype = 7
       ) as currentphase2,
       (
          select value
            from latestzonemeasurements
           where measurementtype = 8
       ) as currentphase3,
       (
          select value
            from latestzonemeasurements
           where measurementtype = 9
       ) as voltagebetweenphase1andphase2,
       (
          select value
            from latestzonemeasurements
           where measurementtype = 10
       ) as voltagebetweenphase1andphase3,
       (
          select value
            from latestzonemeasurements
           where measurementtype = 11
       ) as voltagebetweenphase2andphase3,
       (
          select value
            from latestzonemeasurements
           where measurementtype = 12
       ) as cumulativepowerimport,
       (
          select max(updated)
            from latestzonemeasurements
           where measurementtype in ( 5,
                                      6,
                                      7,
                                      8,
                                      9,
                                      10,
                                      11,
                                      12 )
       ) as timestamp