    CREATE TABLE IF NOT EXISTS lai_apicreate
    (
       id SERIAL PRIMARY KEY
	   ,line_name varchar(64) NOT NULL 
       ,date date NOT NULL
       ,description text NOT NULL
    );