using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGenerator
{
    public class Populator
    {
        private readonly string[] enties;

        private readonly int[] maxOccurencies;

        private int[] occurencies;

        private Random rand;

        public Populator(int populationSize, params (string entity, int ammount, bool inPercent)[] EvidenceDefinitons)
        {
            rand = new Random();
            enties = new string[EvidenceDefinitons.Length];
            maxOccurencies = new int[EvidenceDefinitons.Length];
            occurencies = new int[EvidenceDefinitons.Length];
            for (int i = 0; i < EvidenceDefinitons.Length; i++)
            {
                var (entity, amount, inPercent) = EvidenceDefinitons[i];
                enties[i] = entity;
                maxOccurencies[i] = inPercent ? (int)(populationSize * (amount / 100f)) : amount;
                occurencies[i] = 0;
            }
        }

        public void Reset()
        {
            occurencies = Enumerable.Repeat(0, occurencies.Length).ToArray();
        }

        public string GetRandomEntity()
        {
            var tries = 0;
            var allowedIndices = occurencies.Select((e, i) => e < maxOccurencies[i] ? i : -1).Where(e => e >= 0).ToArray();
            while (tries < allowedIndices.Length)
            {
                var ind = allowedIndices[rand.Next(0, allowedIndices.Length - 1)];
                if (occurencies[ind] < maxOccurencies[ind])
                {
                    occurencies[ind]++;
                    return enties[ind];
                }

                tries++;
            }

            return "NO ENTITY!";
            // throw new ArgumentOutOfRangeException("No more allowed evidences...");
        }
    }
}
