using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGenerator
{
    public class RandomNameDefinition : GenerationDefiniton<string>
    {
        private static string[]? _names;

        private static string[] names
        {
            get
            {
                if (_names == null)
                    Init();
                return _names;
            }
        }

        private static void Init()
        {
            if(File.Exists(Path.Join(Environment.CurrentDirectory, "names.txt")))
            {
                _names = File.ReadAllLines(Path.Join(Environment.CurrentDirectory, "names.txt"));
                return;
            }
            _names = DefinedChoices.Names;
        }

        public override string Evaluate(Random rand, int pickCount)
        {
            return names[rand.Next(0, names.Length - 1)];
        }
    }
}
