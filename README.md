# FamilyTree
This application was built for the ASI Karikas 2023 competition. One of the goals of the competition was to build a family tree creator application. https://asikarikas.ee/

# Installation

* Clone or download the repository to your local machine.
* Make sure that you have WINDOWS 10
* Open the solution in Visual Studio.
  * Make sure you have .NET Maui installed
  * Application uses .NET 6.0
* Open Visual Studio and the .sln from the repository
* !Wait a bit before starting as VS needs to download Nuget packages!
* Build the solution and run the app

# Features
* Import people to the application from a .txt file
* Uses a .json file as a database for people
* Add new people to the database
* Add a spouse and children to a person
  * Spouse cannot be a person's parent, grandparent or any other person of direct ancestry
  * A child cannot be the person's parents or granderparents. Also some other person who already has parents can't be added as a child as well.
* See person's parents, siblings, children and grandchildren in a family tree

# Import people
* Import a .txt file with people as data. '*' separates fields and '#' separates different people
* People have 11 digit long ids
* Second id is their spouse's id
* After the spouse id come children ids
### Example .txt file
`*JaanM*37306102531*47505112752*50208122711*60401012737*#*MariM*47505112752*37306102531*50208122711*60401012737*#*LauraM*60401012737*0*#*JoonasM*50208122711*0*#`
