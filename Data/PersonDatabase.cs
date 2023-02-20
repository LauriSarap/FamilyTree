using FamilyTree.Models;
using Newtonsoft.Json;
using System.Diagnostics;

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

            if (newPerson.childrenIds == null)
            {
                newPerson.childrenIds = new long[] { };
            }

            if (newPerson.parentIds == null)
            {
                newPerson.parentIds = new long[] { };
            }

            if (people == null)
            {
                people = new Dictionary<long, Person>();
                people[newPerson.personalId] = newPerson;
            }
            else
            {
                people[newPerson.personalId] = newPerson;
            }

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

        public static async Task RemoveSpouse(long person)
        {
            string json = await File.ReadAllTextAsync(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);

            people[person].spouseId = 0;

            string updatedJson = JsonConvert.SerializeObject(people, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, updatedJson);
        }

        public static async Task AddParentsToPerson(long personId, long parent1 = 0, long parent2 = 0)
        {
            string json = await File.ReadAllTextAsync(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);

            // Add parent 1 to person
            if (parent1 != 0)
            {
                people[personId].parentIds = new long[] { parent1 };
            }

            // Add parent 2 to person
            if (parent2 != 0)
            {
                if (people[personId].parentIds.Length == 0)
                {
                    people[personId].parentIds = new long[] { parent2 };
                }
                else
                {
                    people[personId].parentIds = new long[] { parent1, parent2 };
                }
            }

            /*// Add person to parent 1's children
            if (parent1 != 0)
            {
                if (people[parent1].childrenIds.Length == 0)
                {
                    people[parent1].childrenIds = new long[] { personId };
                }
                else
                {
                    people[parent1].childrenIds = people[parent1].childrenIds.Append(personId).ToArray();
                }
            }

            // Add person to parent 2's children
            if (parent2 != 0)
            {
                if (people[parent2].childrenIds.Length == 0)
                {
                    people[parent2].childrenIds = new long[] { personId };
                }
                else
                {
                    people[parent2].childrenIds = people[parent2].childrenIds.Append(personId).ToArray();
                }
            }*/

            string updatedJson = JsonConvert.SerializeObject(people, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, updatedJson);
        }

        public static async Task AddChildrenToPerson(long personId, List<long> childrenIds)
        {
            string json = await File.ReadAllTextAsync(filePath);
            Dictionary<long, Person> people = JsonConvert.DeserializeObject<Dictionary<long, Person>>(json);

            // Get current children if any
            List<long> currentChildren = people[personId].childrenIds.ToList();

            foreach (long childId in childrenIds)
            {
                if (currentChildren.Contains(childId) == false)
                {
                    currentChildren.Add(childId);
                }
            }

            people[personId].childrenIds = currentChildren.ToArray();

            string updatedJson = JsonConvert.SerializeObject(people, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, updatedJson);
        }
    }
}
