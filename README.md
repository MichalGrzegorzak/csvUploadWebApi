# CsvUploadWebApi

### Assumptions:
1. Single call can't be longer than 24h
2. call_date in import file, defines start date of a call, which means call end time can be on the next day
3. 'Reference' can't be used as PrimaryKey, I'm generation keys during the upload
4. User will upload just single file to Upload

### What I used:
1. Dapper - haven't used it for a while, and wanted to give it a chance
2. FluentMigration - Dapper lacks migrations functionalities so this was a must
3. CsvHelper - think it's a best library to read/write CSV
4. Did setup DI in Tests

### What I would add:
1. Global error handling
2. Add some better Logging library (NLog/Serilog/..)
3. More tests
4. In Test include LocalDB as a file
5. Use Hangfire instead of own BackgroundService
6. Return ticket for user, then on different endpoint by passing ticket, user could see import results
7. Deploy on AWS Lambda after tweaks
