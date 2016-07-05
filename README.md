# RestSql
A tool to automate the middle tier.  This will create a middle tier between a database and a webserver and publish a Rest API.

# Goals
* To eliminate the creation of the middle tier
* Every time we create a website,
we currently have to create the code that connects
from the database to the web server and publish an API.
This is very rinse and repeat code.  There should be a 
way to automate it.  Isolating the development work to
the database and the website UI.  While still allowing
services to directly connect to the database or constraining
services to connect through the API.
