## DBAccess, Functional summary
Pratice use console application to operate target database.

Extend from practice of CSVStream, you need  to insert the created CSV file into database for further operation:
1. Peform insert record into database
1. Perform update record into database
1. Read data from database, filter by person's birthday before/after 1 weeks of application start date

## Data schema format and example
| ID | Name | Sex | Birthday | Intime | Utime |
| ------ | ------ | ------- | ------- | ----- | ---- |
| 1 | John | Male | 2017/03/28 | 2017/03/29 | 2017/03/30 |

## Export format and example
```
|ID|Name|Sex|Birthday|
|1 |John|Male|2017/03/28|
```
