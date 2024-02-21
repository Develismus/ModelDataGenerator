using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGenerator
{
    public class OccurenceDefinition
    {
        public OccurenceDefinition(string entity, int maxOccurencies, bool resetable)
        {
            this.entity = entity;
            this.maxOccurencies = maxOccurencies;
        }
        public readonly string entity;
        public readonly int maxOccurencies;
        protected int occurencies;
        private readonly bool isResetable;
        public bool TryPick(int pickCount, out string entity)
        {
            entity = this.entity;
            if(IsAllowed(pickCount))
            {
                occurencies++;
                return true;
            }
            return false;
        }
        public virtual bool IsAllowed(int pickCount) => occurencies < maxOccurencies;

        public void Reset()
        {
            if (isResetable)
                occurencies = 0;
        }
    }

    public class FixedAmountOccurence : OccurenceDefinition
    {
        public FixedAmountOccurence(string entity, int maxOccurencies) : base(entity, maxOccurencies, false)
        {
        }
    }

    public class OneOfAmmountOccurence : OccurenceDefinition
    {
        public OneOfAmmountOccurence(string entity, int howMany) : base(entity, howMany, true)
        {
        }
        public override bool IsAllowed(int pickCount) => (pickCount / (float)maxOccurencies) > occurencies;
    }


    public class StringPopulationDefiniton : GenerationDefiniton<string>
    {
        private List<OccurenceDefinition> definitions = new ();

        public StringPopulationDefiniton() { }
        public StringPopulationDefiniton(params OccurenceDefinition[] entities)
        {
            definitions.AddRange(entities);
        }

        public StringPopulationDefiniton AddEntity(OccurenceDefinition entity)
        {
            definitions.Add(entity);
            return this;
        }

        public override void Reset()
        {
            base.Reset();
            definitions.ForEach(e => e.Reset());
        }

        public override string Evaluate(Random rand, int pickCount)
        {
            var tries = 0;
            var allowedOnes = definitions.Where(e => e.IsAllowed(pickCount)).ToList();
            while (tries < allowedOnes.Count * 2)
            {
                var definition = allowedOnes[rand.Next(0, allowedOnes.Count - 1)];
                if (definition.TryPick(pickCount, out string entitiy))
                {
                    return entitiy;
                }

                tries++;
            }

            return "NO ENTITY!";
        }
    }
}
