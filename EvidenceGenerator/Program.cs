
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using MongoDB.Driver;

namespace EvidenceGenerator
{
    [Serializable]
    class Machine
    {
        public string MachineName { get; set; }
        public string ArticleNr { get; set; }
        public string MachineNr { get; set; }

    }
    class Entitiy
    {
        public string name;
        public int ID;
        public string position;
        public DateTime created;
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var machines = new MongoClient("mongodb://localhost:27017").GetDatabase("test").GetCollection<Machine>("machines");




            var machineGenerator = new Generator<Machine>()
                .DefineFieldGenerator(machine => machine.MachineName, (_, _) => "alpha 6.0")
                .DefineFieldGenerator(machine => machine.ArticleNr, (_, _) => "MWLF-00000V")
                .DefineFieldGenerator(machine => machine.MachineNr, (_, count) => $"22-085{60 + count}-00002");


            var res = machineGenerator.Generate(10).ToList();
            machines.InsertMany(res);
            Console.WriteLine("lele");
        }
    }
}