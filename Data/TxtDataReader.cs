using FamilyTree.Models;
using System.Diagnostics;

namespace FamilyTree.Data
{
    public static class TxtDataReader
    {
        public static bool isInitialized;

        static TxtDataReader()
        {
            Debug.WriteLine("TxtDataReader script created!");
        }

        public static async Task<Dictionary<long, Person>> ReadFile(string txtFilePath)
        {
            Debug.WriteLine("Reading file!");

            Dictionary<long, Person> peopleFromTxtFile = new Dictionary<long, Person>();

            string fileContents = await File.ReadAllTextAsync(txtFilePath);

            fileContents = fileContents.Trim();

            string[] lines = fileContents.Split("#", StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrEmpty(lines.Last()))
            {
                lines = lines.Take(lines.Length - 1).ToArray();
            }

            Debug.WriteLine("Line amount: " + lines.Length);


            foreach (string line in lines)
            {
                Person person = ParsePerson(line);
                peopleFromTxtFile.Add(person.personalId, person);
            }


            return peopleFromTxtFile;
        }

        private static Person ParsePerson(string line)
        {
            string[] parts = line.Split('*');

            if (string.IsNullOrEmpty(parts.Last()))
            {
                parts = parts.Take(parts.Length - 1).ToArray();
            }

            Debug.WriteLine("Parts amount on this line: " + parts.Length);

            Person person = new Person();
            person.name = parts[0].Trim();
            person.personalId = long.Parse(parts[1]);
            person.spouseId = long.Parse(parts[2]);


            List<long> childrenIds = new List<long>();

            if (parts.Length > 2)
            {
                for (int i = 3; i < parts.Length; i++)
                {
                    long id = long.Parse(parts[i]);

                    childrenIds.Add(id);
                }
            }

            if (childrenIds.Count > 0)
            {
                person.childrenIds = childrenIds.ToArray();
            }
            else
            {
                person.childrenIds = null;
            }

            return person;
        }
    }
}
