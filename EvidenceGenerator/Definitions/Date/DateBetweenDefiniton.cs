using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGenerator
{
    public class DateBetweenDefiniton : GenerationDefiniton<DateTime>
    {
        private readonly DateTime start; 
        private readonly DateTime end;
        public DateBetweenDefiniton(DateTime from, DateTime to)
        {
            start = from;
            end = to;
        }
        public override DateTime Evaluate(Random rand, int pickCount)
        {
            int range = ((TimeSpan)(end - start)).Days;
            return start.AddDays(rand.Next(range));
        }
    }
}
