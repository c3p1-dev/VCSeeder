# VCSeeder

VCSeeder interfaces VisualCarnet database to visualcarnet webAPI on a C3P1 webapp instance.
It first the base url of the WebAPI, then asks for login informations.

If login is successful, it attempts to read the database (usually vcdump.db) locally,
and write line to line every table to the new remote database.

If login fails or source database is not valid, it ends.