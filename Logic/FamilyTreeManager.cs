using FamilyTree.Data;
using FamilyTree.Models;
using System;
using System.Diagnostics;

namespace FamilyTree.Logic
{
    public static class FamilyTreeManager
    {
        // Status
        public static bool isInitialized = false;
        public static bool usingAppDirectory = false;

        // Database configurations
        public static string dbFile = "people.json";
        public static string solutionFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\"));
        public static string dbFilePathIfUsingSolution = Path.Combine(solutionFolder, dbFile);

        public static string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string applicationDataFolder = Path.Combine(appDirectory, "FamilyTreeManagerData");

        // Database
        public static Dictionary<long, Person> people = new();

        public static event Action PeopleUpdated;
        public static void InitalizePeopleEvent() { }

        static FamilyTreeManager()
        {
            // Txt database file logic
            TxtDataReader.isInitialized = true;

            PeopleUpdated += InitalizePeopleEvent;
            Debug.WriteLine("People updated event initialized: " + PeopleUpdated);


            // Json database file logic
            if (usingAppDirectory == false)
            {
                if (File.Exists(dbFilePathIfUsingSolution))
                {
                    Debug.WriteLine("Found database at: " + dbFilePathIfUsingSolution);
                }
                else
                {
                    Debug.WriteLine("Failed to find database!");
                }

                // Load database
                PersonDatabase.isInitialized = true;
                PersonDatabase.filePath = dbFilePathIfUsingSolution;
                Debug.WriteLine("Database is at: " + dbFilePathIfUsingSolution);
            }
            else
            {
                Debug.WriteLine("AppDirectory folder is at: " + appDirectory);

                if (!Directory.Exists(applicationDataFolder))
                {
                    try
                    {
                        Debug.WriteLine($"Creating a folder in {applicationDataFolder}");
                        Directory.CreateDirectory(applicationDataFolder);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Couldn't create a folder in {applicationDataFolder}, because: {e}");
                        throw;
                    }
                }

                string filePath = Path.Combine(applicationDataFolder, "people.json");
                if (File.Exists(filePath) == false)
                {
                    File.Create(filePath).Dispose();
                }

                PersonDatabase.isInitialized = true;
                PersonDatabase.filePath = filePath;
                Debug.WriteLine("Database is at: " + filePath);
            }
        }

        public static async Task UpdatePeopleList()
        {
            if(people == null) return;

            people.Clear();
            people = await PersonDatabase.LoadAllPersons();
        }

        public static void PeopleUpdatedEventCalled()
        {
            PeopleUpdated.Invoke();
        }



        public static bool DoesPersonExist(string personalId)
        {
            long id = long.Parse(personalId);

            if (people == null)
            {
                people = new Dictionary<long, Person>();
            }

            if (people.ContainsKey(id))
            {
                return true;
            }

            return false;
        }

        // Adders
        public static async Task AddPerson(Person newPerson)
        {

            await PersonDatabase.AddPerson(newPerson);
            //await AddSpouse(newPerson.spouseId, newPerson.personalId);
            await AddParents(newPerson.personalId);

            await UpdatePeopleList();
        }

        public static async Task AddSpouse(long personalId, long spouseId)
        {
            if (spouseId != 0)
            {
                if (people[spouseId] != null)
                {
                    await PersonDatabase.AddSpouseToPerson(personalId, spouseId);
                }
            }
        }

        public static async Task AddParents(long personId)
        {
            List<long> parents = new();
            int parentsFound = 0;

            foreach (var person in people.Values)
            {
                if (parentsFound == 2) break;

                if (person.childrenIds.Length > 0)
                {
                    foreach (var childId in person.childrenIds)
                    {
                        if (childId == personId)
                        {
                            parents.Add(person.personalId);
                            parentsFound += 1;
                            break;
                        }
                    }
                }
            }

            if (parents.Count > 0)
            {
                if (parents.Count == 1)
                {
                    await PersonDatabase.AddParentsToPerson(personId, parents[0]);
                }
                else if (parents.Count == 2)
                {
                    await PersonDatabase.AddParentsToPerson(personId, parents[0], parents[1]);
                }
            }
        }

        public static async Task AddChildren(long personId, List<long> childrenIdsToAdd)
        {
            List<long> newChildren = new();

            foreach (var childId in childrenIdsToAdd)
            {
                if (people.ContainsKey(childId))
                {
                    newChildren.Add(childId);
                }
            }

            await PersonDatabase.AddChildrenToPerson(personId, newChildren);
        }

        // Getters
        public static List<Person> GetParents(Person personWithParents)
        {
            List<Person> parents = new();
            ;

            if (personWithParents.parentIds.Length > 0)
            {
                foreach (var parentId in personWithParents.parentIds)
                {
                    Person parent = people[parentId];
                    parents.Add(parent);
                }
            }

            return parents;
        }

        public static List<Person> GetSiblings(Person personWithSiblings, List<Person> parents)
        {
            if (parents.Count == 0) return new List<Person>();

            List<Person> siblings = new();

            //Debug.WriteLine("Parents count: " + parents.Count);
            Person parent1 = parents.First();
            Person parent2 = parents.Last();
            //Debug.WriteLine("Parent 1 name: " + parent1.name);
            //Debug.WriteLine("Parent 2 name: " + parent2.name);


            // Parent 1, also taking in half-siblings
            if (parent1 != null)
            {
                foreach (var childId in parent1.childrenIds)
                {
                    if (childId != personWithSiblings.personalId)
                    {
                        Person sibling = people[childId];
                        siblings.Add(sibling);
                    }
                }
            }

            // Parent 2, also taking in half-siblings and ensuring no duplicates
            if (parent2 != null)
            {
                List<Person> siblingsToAddFromOtherParent = new List<Person>();


                foreach (var childId in parent2.childrenIds)
                {
                    bool siblingExistsAlready = false;

                    foreach (var sib in siblings)
                    {
                        if (sib.personalId == childId)
                        {
                            siblingExistsAlready = true;
                            break;
                        }
                    }

                    if (!siblingExistsAlready && childId != personWithSiblings.personalId)
                    {
                        Person sibling = people[childId];
                        siblingsToAddFromOtherParent.Add(sibling);
                    }
                }

                siblings.AddRange(siblingsToAddFromOtherParent);
            }

            return siblings;
        }

        public static async Task<List<Person>> GetChildren(Person personWithChildren)
        {
            List<Person> children = new();

            if (personWithChildren.childrenIds.Length > 0)
            {
                foreach (var childId in personWithChildren.childrenIds)
                {
                    if (people.ContainsKey(childId) == false)
                    {
                        Person child = await AddMissingChild(childId);
                        children.Add(child);
                    }
                    else
                    {
                        Person child = people[childId];
                        children.Add(child);
                    }
                }
            }

            return children;
        }

        public static async Task<Person> AddMissingChild(long childId)
        {
            Person missingChild = new Person();
            missingChild.personalId = childId;
            missingChild.name = "Unknown";
            await PersonDatabase.AddPerson(missingChild);
            return missingChild;
        }

        public static async Task<List<Person>> GetAncestors(Person personWithAncestors)
        {
            List<Person> ancestors = new List<Person>();

            foreach (var parentId in personWithAncestors.parentIds)
            {
                if (people.ContainsKey(parentId))
                {
                    Person parent = people[parentId];
                    ancestors.Add(parent);
                    ancestors.AddRange(await GetAncestors(parent));
                }
            }

            return ancestors;
        }

        public static async Task<List<Person>> GetDescendants(Person personWithDescendants)
        {
            List<Person> descendants = new List<Person>();

            foreach (var childId in personWithDescendants.childrenIds)
            {
                if (people.ContainsKey(childId))
                {
                    Person descendant = people[childId];
                    descendants.Add(descendant);
                    descendants.AddRange(await GetDescendants(descendant));
                }
            }

            return descendants;
        }
    }
}
