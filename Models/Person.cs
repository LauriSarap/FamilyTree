
namespace FamilyTree.Models
{
    public class Person
    {
        public string name { get; set; }
        public long personalId { get; set; }
        public long spouseId { get; set; }
        public long[] parentIds { get; set; }
        public long[] childrenIds { get; set; }
    }
}
