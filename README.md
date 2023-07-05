# myQOI
myQOI is a C# application that will install as a Windows Service and run at scheduled times.

## myQOI_API
myQOI_API is an Azure Function. It's purpose is to receive information from the clients and post the data into a SQL DB

## myQOI_Config
myQOI_Config is an Azure Function. It's purpose is to receive scheduled run times from a database and send it to the clients.


## myQoI Creators
Dr. Philip Neufeld, a tech enthusiast and advocate for digital equity, devised a groundbreaking idea to capture and address the issue of digital inequity. Realizing that internet speed disparities were a significant contributing factor, Dr. Neufeld conceptualized a unique approach to quantify and visualize this inequality. Drawing inspiration from speed tests commonly used to assess internet performance, he proposed leveraging this data to shed light on the digital divide. Dr. Neufeld's visionary concept caught the attention of Bryan Alvarado, a skilled software engineer passionate about bridging technological gaps. Collaborating closely, Bryan helped bring Dr. Neufeld's idea to life by developing powerful software capable of collecting speed test results from across various regions. With Bryan's expertise, the software was swiftly deployed, enabling researchers and policymakers to identify and tackle digital inequities with data-backed precision. The joint effort between Dr. Philip Neufeld and Bryan Alvarado marked a significant step forward in the pursuit of a more inclusive and connected digital future.


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
