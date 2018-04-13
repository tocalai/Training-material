CREATE TABLE IF NOT EXISTS lai_apiget
(
       id SERIAL PRIMARY KEY,
       city_name varchar(128) NOT NULL,
	   city_id varchar(64) NOT NULL,
       date date NOT NULL,
       description varchar(256) NOT NULL,
       max_temperature_celsius int,
	   min_temperature_celsius int,
       intime timestamp NOT NULL default CURRENT_TIMESTAMP,
       utime timestamp NOT NULL default CURRENT_TIMESTAMP,
	   CONSTRAINT unique_city_id_date UNIQUE(city_id, date)
)