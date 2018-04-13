## APIGET, Functional summary
Pratice calling 3rd party's API via routine job and implemet viewer to retrieve the result.

Image that you are in project that need provide weather information(as example in Japan area) to client. 
As the scenario you need to implement the below feature:
1. Connect to weather API: livedoor with interval 10 mins, got the feed data and insert into target database
1. Create interface with filter of prefecture, city and forecast day to get back the weather information

## Database schema format and example
|ID|CityName|Description|Temperature|Intime|Utime
|---------|:------|:------|:------|:------|:-----|
|1|Kobe|Rain|20|2017/03/29|2017/03/29|

## Output data format and example
|Date|City Name|Weather|Temperature|
|---------|:------|:------|:------|
|2017/3/29|Kobe|Rain|20℃|
