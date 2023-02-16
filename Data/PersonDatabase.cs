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


        public static async Task<Dictionary<long, Person>> LoadAllPersons()
        {
            string json = await File.ReadAllTextAsync(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);
            return people;
        }

        public static async Task AddPerson(Person newPerson)
        {
            string json = await File.ReadAllTextAsync(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);

            people[newPerson.personalId] = newPerson;

            string updatedJson = JsonConvert.SerializeObject(people, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, updatedJson);
        }

        public static async Task AddSpouseToPerson(long personId, long spouseId)
        {
            string json = await File.ReadAllTextAsync(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);

            people[personId].spouseId = spouseId;

            if (spouseId != 0)
            {
                var spouse = people.Values.FirstOrDefault(p => p.personalId == spouseId);
                if (spouse != null)
                {
                    spouse.spouseId = personId;
                }
            }

            string updatedJson = JsonConvert.SerializeObject(people, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }
}
