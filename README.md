# SQL Data Generator

[![build status](https://gitlab.com/xlebenny/SQLDataGenerator/badges/master/build.svg)](https://gitlab.com/xlebenny/SQLDataGenerator/commits/master)

Binary file: [Download](https://gitlab.com/xlebenny/SQLDataGenerator/-/jobs/artifacts/master/download?job=Release) (Require .net framework 4.5)

This [repository](https://gitlab.com/xlebenny/SQLDataGenerator) clone from gitlab

<br />
A tool to generate a lot of <b>make sense</b> data for DEMO / performance test.

This tool only generate a SQL statement (plain text), no directly connect to database.

For exmaple:
```
insert into User ([FirstName], [LastName], [EmailAddress]) values
   ('Benny', 'Leung', 'bennyLeung@example.com')

insert into Email ([EmailAddress], [Content]) values
   ('bennyLeung@example.com', 'Dear Benny Leung,\nbla bla bla...')
```
* Insert statement, it can modify what you want before insert 
* Can tailor made data pool
* `EmailAddress` and Dear `xxx` is come from `User` table, it's make sense

<br /><br />
## How to use

Step 0:  You must have a database schema and privilege to query some table information like `information_schema`.

Step 1:  Click "Show SQL", and copy and prase SQL to your query tool like `SSMS`, `phpmyadmin` and parse result to textbox

Step 2:  Configure `Generate Format` and how many rows you need. For detail, reference: [Formatter section](#formatter)

Step 3:  Save config, save config, save config (too important, generate may throw exception)

Step 4:  Click Generate :)

Step 5:  Using your query tool and run it.  Make sure <b>backup</b> Database before run.

<br /><br />
## Formatter

### Sqeuential Formatter

Generate sqeuentual number, usually for auto increment Primary Key

##### Example
````
{seq|start:0, end:999, step:1, padding:'\{0:000\}'}
````

Output
````
000
001
002
...
````

##### Parameters
| Name | Type | Required | Default Value | Remark |
| --- | --- | --- | --- | --- |
| start | string | No | 0 | Start from 0
| end | long | No | long.MaxValue | After `end`, it will restart to `start`
| step | long | No | 1 | i = i + `step`
| padding | string | No | {0} | Same as `string.format` if need to padding, can use this


<br /><br />
### Dictionary Formatter

Random word in dictionary

#### Example
````
{dict|name:'firstNameDict', field:'Eng'}
````

firstNameDict.txt in `Dictionary` folder (using `tab` delimite column)
````
Chi	Eng	Gender
雅婷  	Daisy	F
建成  	Mack	M
姵忠  	Ada	F
....
````

Output
````
Peter
Tom
...
````

##### Parameters
| Name | Type | Required | Default Value | Remark |
| --- | --- | --- | --- | --- |
| name | string | Yes | - | Your dictionary name in Dictionary folder
| field | string | Yes | - | Column name in dictionary (first row)


<br /><br />
### DateTime Formatter

Generate DateTime item, can specify multi range

#### Example
````
{dateTime|[\{ dayRange: '2017-01-01~2017-12-31', timeRangeOfWeekDay:['mon/tue/wed/thur/fri:09:00:00~18:00:00', 'sat:09:00:00~13:00:00']\}, { dayRange: '2016-01-01~2016-12-31', timeRangeOfWeekDay:['mon/tue/wed/thur/fri:09:00:00~18:00:00']\}]}
````

From 2017-01-01 To 2017-12-31 Monday to Firday 09:00 to 18:00 & Satudary 09:00 to 13:00
<br />From 2016-01-01 To 2017-12-31 Monday to Firday 09:00 to 18:00

##### Parameters
| Name | Type | Required | Default Value | Remark |
| --- | --- | --- | --- | --- |
| dayRange | string | Yes | - | YYYY-MM-DD~YYYY-MM-DD (FromDate~ToDate)
| timeRangeOfWeekDay | object[] | Yes | - | ['mon/tue/wed/thur/fri/sat/sun:HH:mm:ss~HH:mm:ss', ....] \(weekday: FromTime~ToTime, ...)

* Becareful: it's `Array` of `Parameters`


<br /><br />
### Reference Formatter

Get value from another field, usually use in foreign key column

#### Example 1
````
{ref|table:'Staff', column:'StaffName'}
````

Get `StaffName` from `Staff` table

#### Example 2
````
{ref|table:'StaffLeave',column:'DateFrom',converterName:'DateTimeConverter',converterParams:['AddDays', 1]}
````

Get `DateFrom` from `StaffLeave` table, and `DateTime.Parse(StaffLeave.DateFrom).AddDays(1)`

##### Parameters
| Name | Type | Required | Default Value | Remark |
| --- | --- | --- | --- | --- |
| table | string | Yes | - | Table name
| column | string | Yes | - | Column name
| converterName | string | No | - | For date column, it can make day range
| converterParams | object[] | No | - | For date column, it can make day range

* If `table` reference current table, it will get current row, that field,
* if not, it will get random row, that field

<br /><br />
## FAQ

### Microsoft SQL Server Management Studio (SSMS) say 'Insufficient memory to continue the execution of the program'

Use `sqlcmd` command, for detail
https://stackoverflow.com/questions/17918770/cannot-execute-script-insufficient-memory-to-continue-the-execution-of-the-prog


### Microsoft SQL Server Management Studio (SSMS) say 'Cannot insert explicit value for identity column in table '&#60;TableName&#62;' when IDENTITY_INSERT is set to OFF.

Check `Identity Insert` before generate

### Need more example?

Find more in [./SQLDataGeneratorApplication/Example](./SQLDataGeneratorApplication/Example)
