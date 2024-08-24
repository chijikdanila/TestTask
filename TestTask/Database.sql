USE master;
GO
CREATE DATABASE test_task;
GO
USE test_task;
GO
CREATE TABLE random_strings (
	[date]				VARCHAR(10)		NOT NULL,
	latin_string		VARCHAR(10)		NOT NULL,
	russian_string		NVARCHAR(10)	NOT NULL,
	number				INT				NOT NULL,
	[float]				FLOAT			NOT NULL
);
GO
CREATE TABLE imported_file (
	PRIMARY KEY(id),
	id					VARCHAR(36)		NOT NULL,
	[file_name]			NVARCHAR(255)	NOT NULL,
	file_hash			VARCHAR(64)		NOT NULL
);
GO
CREATE TABLE balance_sheet (
	PRIMARY KEY(id),
	id					INT				NOT NULL	IDENTITY(1,1),
	sheet_number		INT				NOT NULL,	
	incoming_active		FLOAT,
	incoming_passive	FLOAT,
	circuit_debet		FLOAT,
	circuit_credit		FLOAT,
	outcoming_active	FLOAT,
	outcoming_passive	FLOAT,
	[file_id]			VARCHAR(36)		NOT NULL
);
GO
ALTER TABLE balance_sheet
ADD CONSTRAINT FK_balance_sheet_to_imported_file
FOREIGN KEY ([file_id]) REFERENCES imported_file(id);
GO
USE master;