# FamilyTree
This application was built for the ASI Karikas 2023 competition. One of the goals of the competition was to build a family tree creator application. https://asikarikas.ee/

# Features
* Import a .txt file with people as data. "*" separates fields and "#" separates different people
* People have 11 digit long ids
* Second id is their spouse's id
* After the spouse id come children ids
### .txt file
`*JaanM*37306102531*47505112752*50208122711*60401012737*#`
`*MariM*47505112752*37306102531*50208122711*60401012737*#`
`*LauraM*60401012737*0*#`
`*JoonasM*50208122711*0*#`

# Installation

* Clone or download the repository to your local machine.
* Open the solution in Visual Studio.
  * Make sure you have .NET Maui installed
  * Application uses .NET 6.0
* Build the solution and run the app
