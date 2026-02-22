# Energy Cost Calculation

This document explains how HeatKeeper tracks and calculates energy costs per sensor per hour.

## Overview

When power measurements arrive, HeatKeeper automatically calculates the energy cost for each sensor based on hourly electricity market prices or a configurable fixed price. Costs are stored per sensor per hour and support three pricing variants: market price, subsidized market price, and fixed price.

## Prerequisites

For energy cost calculation to work, the following must be configured:

1. A sensor must be assigned to a zone that belongs to a location.
2. The location must have an `EnergyPriceAreaId` set (the market area for electricity pricing).
3. The sensor must report measurements of type `CumulativePowerImport` (cumulative Wh counter).

## Calculation Process

### 1. Trigger

Energy cost calculation is triggered automatically every time measurements are inserted via `WhenMeasurementsAreInserted`. Only measurements of type `CumulativePowerImport` are processed.

### 2. Delta Calculation (kWh Consumed)

The sensor reports a cumulative energy counter (in Wh). HeatKeeper looks up the previous reading for the same sensor and calculates the difference:

```
deltaKwh = (currentValue - previousValue) / 1000
```

- If there is no previous reading, the measurement is skipped (first reading establishes the baseline).
- If the delta is zero or negative (e.g., after a meter reset), the measurement is skipped.

### 3. Hour Distribution

Measurements may span multiple hours. HeatKeeper truncates both the previous and current timestamps to their hour boundaries and distributes the energy evenly across all spanned hours:

```
kwhPerHour = deltaKwh / numberOfHours
```

A cost record is then upserted for each hour.

### 4. Price Lookup

For each hour, HeatKeeper queries the stored electricity price for the location's price area:

```sql
SELECT PriceInLocalCurrency, PriceInLocalCurrencyAfterSubsidy
FROM EnergyPrices
WHERE EnergyPriceAreaId = @EnergyPriceAreaId
  AND TimeStart <= @Hour
  AND TimeEnd > @Hour
```

If no price is found, HeatKeeper attempts to import prices from ENTSO-E on the fly. If that also fails, the cost is recorded as 0.

### 5. Cost Calculation

Three cost values are calculated for each sensor/hour record:

| Column | Formula |
|--------|---------|
| `CostInLocalCurrency` | `kWhForHour × marketPrice` |
| `CostInLocalCurrencyAfterSubsidy` | `kWhForHour × subsidizedPrice` |
| `CostInLocalCurrencyWithFixedPrice` | `kWhForHour × fixedPrice` if fixed pricing is enabled, otherwise same as `CostInLocalCurrency` |

### 6. Upsert

The resulting record is upserted into the `EnergyCosts` table using a unique index on `(SensorId, Hour)`. If a record for that sensor and hour already exists (e.g., from a previous measurement within the same hour), it is overwritten with the latest calculated values.

## Pricing Models

### Market Price (Dynamic)

Electricity prices are imported hourly from [ENTSO-E](https://transparency.entsoe.eu/) for configured price areas. Each price area is identified by an EIC code (e.g., `10YNO-1--------2` for Norway NO1). Prices are stored in the local currency using the EUR exchange rate and include VAT.

### Subsidized Price

For Norwegian price areas, a government subsidy is applied when the price exceeds 0.77 NOK/kWh:

```
subsidy = (priceInLocalCurrency - 0.77) × 0.90
subsidizedPrice = priceInLocalCurrency - subsidy  (when subsidy > 0)
```

### Fixed Price

A location can be configured to use a fixed price per kWh instead of the market price. This is controlled by two location settings:

- `UseFixedEnergyPrice` — enables fixed pricing
- `FixedEnergyPrice` — the price per kWh in local currency

When fixed pricing is enabled, `CostInLocalCurrencyWithFixedPrice` is calculated using the fixed rate. Market price and subsidized price are still calculated and stored regardless.

## Database Schema

```sql
CREATE TABLE EnergyCosts (
    Id                                INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    SensorId                          INTEGER NOT NULL REFERENCES Sensors(Id),
    PowerImport                       REAL NOT NULL,
    CostInLocalCurrency               REAL NOT NULL,
    CostInLocalCurrencyAfterSubsidy   REAL NOT NULL,
    CostInLocalCurrencyWithFixedPrice REAL NOT NULL,
    Hour                              DATETIME NOT NULL
);

CREATE UNIQUE INDEX idx_energy_costs_sensor_hour ON EnergyCosts (SensorId, Hour);
```

## Flow Summary

```
New CumulativePowerImport measurements arrive
        │
        ▼
Filter + group by sensor
        │
        ▼
For each sensor measurement:
  ├─ Look up previous cumulative reading
  ├─ Compute deltaKwh = (current - previous) / 1000
  ├─ Distribute kWh across spanned hours
  │
  └─ For each hour:
       ├─ Query market price (import from ENTSO-E if missing)
       ├─ Calculate market cost, subsidized cost, fixed cost
       └─ Upsert EnergyCosts (SensorId, Hour)
```

## Key Source Files

| Component | Path |
|-----------|------|
| Cost calculator | `src/HeatKeeper.Server/EnergyCosts/CalculateEnergyCosts.cs` |
| Upsert handler | `src/HeatKeeper.Server/EnergyCosts/UpsertEnergyCost.cs` |
| Measurement trigger | `src/HeatKeeper.Server/Measurements/WhenMeasurementsAreInserted.cs` |
| Price importer | `src/HeatKeeper.Server/EnergyPrices/Api/ImportEnergyPrices.cs` |
| Database schema | `src/HeatKeeper.Server.Database/EnergyCosts/` |
| Integration tests | `src/HeatKeeper.Server.WebApi.Tests/EnergyCostsTests.cs` |
