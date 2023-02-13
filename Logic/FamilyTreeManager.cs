using System.Diagnostics;
using FamilyTree.Data;
using FamilyTree.Models;

namespace FamilyTree.Logic
{
    public static class FamilyTreeManager
    {
        // Status
        public static bool isInitialized = false;

        // Database configurations
        public static string dbFile = "people.json";
        public static string solutionFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\"));
        public static string dbFilePath = Path.Combine(solutionFolder, dbFile);

        // Database
        public static Dictionary<long, Person> people = new();

        static FamilyTreeManager()
        {
            Debug.WriteLine("FamilyTree script created");

            if (File.Exists(dbFilePath))
            {
                Debug.WriteLine("Found database at: " + dbFilePath);
            }
            else
            {
                Debug.WriteLine("Failed to find database!");
            }

            // Load database
            PersonDatabase.isInitialized = true;
            PersonDatabase.filePath = dbFilePath;
        }

        public static void UpdatePeopleList()
        {
            people.Clear();
            people = PersonDatabase.LoadAllPersons();
        }

        public static void AddPerson(Person newPerson)
        {
            PersonDatabase.AddPerson(newPerson);
            AddSpouse(newPerson.spouseId, newPerson.personalId);

            UpdatePeopleList();
        }

        public static bool DoesPersonExist(string personalId)
        {
            long id = long.Parse(personalId);

            if (people.ContainsKey(id))
            {
                return true;
            }

            return false;
        }

        private static void AddSpouse(long spouseId, long personalId)
        {
            if (spouseId != 0)
            {
                foreach (var id in people.Keys)
                {
                    if (id == spouseId)
                    {
                        Console.WriteLine("Found spouse with id: " + id + "!");
                        PersonDatabase.AddSpouseToPerson(id, personalId);
                    }
                }
            }
        }
    }
}
