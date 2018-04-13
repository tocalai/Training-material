CREATE TABLE IF NOT EXISTS lai_data 
(
    id SERIAL PRIMARY KEY
	, name varchar(512) NOT NULL
	, sex varchar(5) NOT NULL
	, birthday date NOT NULL
	, intime date NOT NULL DEFAULT CURRENT_DATE
	, utime date NOT NULL DEFAULT CURRENT_DATE
)