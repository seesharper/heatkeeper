SELECT NOT EXISTS(
SELECT 1 FROM sqlite_master
WHERE type='table' and name = 'VersionInfo')