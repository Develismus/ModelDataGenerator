using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Amazon.Auth.AccessControlPolicy;

using MongoDB.Driver;

namespace EvidenceGenerator
{
    public class FieldGenerationCondition<TModel> 
    {
    }
    public class FieldGenerationOptions<TModel>
    {
        public Dictionary<string, Func<TModel, bool>>? conditions;

        public FieldGenerationOptions(params Expression<Func<TModel, bool>>[] conditions)
        {
            this.conditions = new Dictionary<string, Func<TModel, bool>>();
            foreach (var expression in conditions)
            {
                //this.conditions.Add();
            }
        }
    }

    class FieldGenerationDefiniton<TModel>
    {
        public  FieldGenerationOptions<TModel>? options;
        public  IGenerationDefinition definition;
        public  string name;
    }

    class FieldGerationDefinitionSet<TModel> : List<FieldGenerationDefiniton<TModel>> {}

    public class Generator<TModel> : IEnumerable<TModel> where TModel : notnull
    {
        private FieldGerationDefinitionSet<TModel> definitions = new FieldGerationDefinitionSet<TModel>();
        private readonly int _seed;
        public Generator(int seed)
        {
            _seed = seed;
        }
        public Generator() : this(new Random().Next(-99999, 99999))
        {
        }

        

        public Generator<TModel> DefineFieldGenerator<T>(Expression<Func<TModel, T>> field, GenerationDelegate<T> generationFunction, FieldGenerationOptions<TModel>? options = null) where T : notnull
            => DefineFieldGenerator(field, new InlineGenerationDefiniton<T>(generationFunction),options);
        public Generator<TModel> DefineFieldGenerator<T>(Expression<Func<TModel, T>> field, GenerationDefiniton<T> definition, FieldGenerationOptions<TModel>? options = null) where T : notnull
            => DefineFieldGenerator(field.GetMemberName(), definition, options);

        public Generator<TModel> DefineFieldGenerator<T>(string field, GenerationDefiniton<T> definition, FieldGenerationOptions<TModel>? options = null) where T : notnull
        {
            definitions.Add( new FieldGenerationDefiniton<TModel>
            {
                options = options,
                definition = definition,
                name = field,
            });
            return this;
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return new GeneratorEnumerator(ref definitions, _seed);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<TModel> Generate(int amount)
        {
            var i = 0;
            foreach (var model in this)
            {
                if (i++ >= amount) yield break;
                yield return model;
            }
        }

        class GeneratorEnumerator : IEnumerator<TModel>
        {
            private FieldGerationDefinitionSet<TModel> definitions;
            private Random rand;
            private int _currentIndex = -1;
            private readonly int _seed;
            private readonly bool finite;

            private readonly int _maxIndex;

            public GeneratorEnumerator(ref FieldGerationDefinitionSet<TModel> defs, int seed , int ammount = 1000, bool finite = true)
            {
                this.finite = finite;
                _maxIndex = ammount;
                _seed = seed;
                rand = new Random(seed);
                definitions = defs;
            }

            private TModel? _current;

            public TModel? Current => _current;

            object? IEnumerator.Current => _current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (finite && _currentIndex >= _maxIndex) return false;

                var ind = _currentIndex;
                try
                {

                    var gen = typeof(TModel);
                    TModel inst = (TModel)Activator.CreateInstance(gen);

                    definitions.Sort(
                        (a, b) =>
                            a.options?.conditions != null && b.options?.conditions != null
                                ? a.options.conditions.ContainsKey(b.name) 
                                    ? 1 
                                    : -1
                                : 0
                        );

                    _currentIndex++;
                    foreach (var definition in definitions)
                    {
                        
                        var field = gen.GetField(definition.name);
                        if (field != null)
                        {
                            field.SetValue(inst, definition.definition.GenerateValue(rand, _currentIndex));
                            continue;
                        }
                        var prop = gen.GetProperty(definition.name);
                        if (prop != null)
                        {
                            prop.SetValue(inst, definition.definition.GenerateValue(rand, _currentIndex));
                        }
                    }

                    _current = inst;
                    return true;
                }
                catch (Exception e)
                {
                    _currentIndex = ind;
                    _current = default;
                    Console.WriteLine(e);
                    return false;
                }
            }

            public void Reset()
            {
                rand = new Random(_seed);
                definitions.ForEach(e => e.definition.Reset());
                _currentIndex = 0;
            }
        }
    }
}
