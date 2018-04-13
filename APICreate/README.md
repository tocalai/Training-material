## APICreate, Functional summary
Pratice to create web API.

Image that your are a rail way company management company, you need provide the API to client side, 
let them log route event into database and also create the other API to query the route event by filter.
1. Add new route record API, input with below information
  - Information event of date
  - Line name
  - Description of the event
1. Add get route record API, input wit query criteria
  - Date range (from-to format)
  - Line of name 
  - Order by event date, ASC or DESC
  - Return type, xml or json format 

## Database schema format and example
|ID|Line Name|Date|Description|
|---------|:------|:------|:------|
|1|JR EAST LINE|2017/03/11|Delay 5 mins|


## Ouput format and example
|Line Name|Date|Description|
|---------|:------|:------|
|JR EAST LINE|2017/01/11|Delay 5 mins|
|JR EAST LINE|2017/01/14|Stop operation due to heavy snow|
