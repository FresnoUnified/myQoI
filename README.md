# myQOI
myQOI is a C# application that will install as a Windows Service and run at scheduled times.

## myQOI_API
myQOI_API is an Azure Function. It's purpose is to receive information from the clients and post the data into a SQL DB

## myQOI_Config
myQOI_Config is an Azure Function. It's purpose is to receive scheduled run times from a database and send it to the clients.





Credit to James Ioppolo for writing the C# version of speedtest-cli by Matt Martz
https://github.com/jamesioppolo/speedtest-net-cli


MIT License

Copyright (c) 2021 Fresno Unified School District

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
