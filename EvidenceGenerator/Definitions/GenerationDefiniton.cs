using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvidenceGenerator
{

    public delegate T GenerationDelegate<T>(Random rand, int pickCound);
    public interface IGenerationDefinition
    {
        public void Reset();
        public object GenerateValue(Random rand, int pickCound);
    }



    public abstract class GenerationDefiniton<T> : IGenerationDefinition where T : notnull
    {
        public abstract T Evaluate(Random rand, int pickCount);
        public object GenerateValue(Random rand, int pickCound)
        {
            return Evaluate(rand, pickCound);
        }

        public virtual void Reset() { }
    }

    public class InlineGenerationDefiniton<T> : GenerationDefiniton<T> where T: notnull
    {
        private GenerationDelegate<T> generationCallback;
        public InlineGenerationDefiniton(GenerationDelegate<T> generationFunction)
        {
            generationCallback = generationFunction;
        }
        public override T Evaluate(Random rand, int pickCount)
        {
            return generationCallback(rand, pickCount);
        }
    }
}
