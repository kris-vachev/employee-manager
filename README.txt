SETUP:
	- Refer to Requirements.txt in the main directory and make sure you have the required dependencies installed.
	- Create a local server named 'Local'. (Could be created using 'sqllocaldb create Local' in the CMD, after installing the 'SqlLocalDB.msi' from /_Redist.)
	- Access '(localdb)\Local' using windows authentication on your SQL server (SSMS).
	- Execute all of the scripts located in /Scripts/Sql Scripts on your SQL server in their respective order.
RUN:
	- Run RUN.BAT located in the main directory.
	- Wait for .NET and NG to finish building their respective projects.
	- The instance is now accessible at http://localhost:4200/.
	- Login credentials are available on the first page of the API documentation.