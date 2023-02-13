using System.Diagnostics;
using FamilyTree.Models;
using Newtonsoft.Json;

namespace FamilyTree.Data
{
    public static class PersonDatabase
    {
        public static bool isInitialized;
        public static string filePath;

        static PersonDatabase()
        {
            Debug.WriteLine("PersonDatabase script created!");
        }


        public static Dictionary<long, Person> LoadAllPersons()
        {
            string json = File.ReadAllText(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);
            return people;
        }

        public static void AddPerson(Person newPerson)
        {
            string json = File.ReadAllText(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);

            people[newPerson.personalId] = newPerson;

            string updatedJson = JsonConvert.SerializeObject(people, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);
        }

        public static void AddSpouseToPerson(long personId, long spouseId)
        {
            string json = File.ReadAllText(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);

            foreach (var person in people.Values)
            {
                Console.WriteLine("Person's name is: " + person.name);
                Console.WriteLine("Person's id is: " + person.personalId);
                Console.WriteLine("Person spouse's id is: " + person.spouseId);
            }

            people[personId].spouseId = spouseId;
            Console.WriteLine("Person " + people[personId] + "'s spouse id is: " + people[personId].spouseId);

            string updatedJson = JsonConvert.SerializeObject(people, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);
        }
    }
}
