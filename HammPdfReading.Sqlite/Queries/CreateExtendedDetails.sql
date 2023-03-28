CREATE TABLE details (
	item REAL NOT NULL,
	part_no INTEGER PRIMARY KEY,
	valid_for_1 INTEGER NOT NULL,
	valid_for_2 INTEGER NOT NULL,
	quantity REAL NOT NULL,
	unit INTEGER NOT NULL,
	designation TEXT NOT NULL,
	"assembly" INTEGER NOT NULL
)