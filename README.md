# myQOI
myQOI is a C# application that will install as a Windows Service and run at scheduled times.

## myQOI_API
myQOI_API is an Azure Function. It's purpose is to receive information from the clients and post the data into a SQL DB

## myQOI_Config
myQOI_Config is an Azure Function. It's purpose is to receive scheduled run times from a database and send it to the clients.





Credit to James Ioppolo for writing the C# version of speedtest-cli by Matt Martz
https://github.com/jamesioppolo/speedtest-net-cli
