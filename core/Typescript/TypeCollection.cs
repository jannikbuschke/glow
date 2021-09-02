using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using OneOf;

namespace Glow.Core.Typescript
{
    public class TypeCollection
    {
        public Dictionary<string, TsType> Types { get; set; }
        public Dictionary<string, TsEnum> Enums { get; set; }

        public bool Exists(Type t)
        {
            var id = t.GetTypeId();
            return Types.ContainsKey(id) || Enums.ContainsKey(id);
        }

        public OneOf<TsType, TsEnum> Find(Type t)
        {
            var id = t.GetTypeId();
            if (Types.ContainsKey(id))
            {
                return Types[id];
            }
            else if (Enums.ContainsKey(id))
            {
                return Enums[id];
            }

            throw new Exception("Did not find " + t.GetTypeId());
        }

        public IEnumerable<OneOf<TsType, TsEnum>> All()
        {
            var result = new List<OneOf<TsType, TsEnum>>();
            foreach (TsType v in Types.Values)
            {
                result.Add(v);
            }

            foreach (TsEnum v in Enums.Values)
            {
                result.Add(v);
            }

            return result;
        }

        public IEnumerable<Module> Modules
        {
            get
            {
                IEnumerable<IGrouping<string, OneOf<TsType, TsEnum>>> byNamespace =
                    All().GroupBy(v => v.Match(v1 => v1.Namespace, v2 => v2.Namespace));

                var modules = byNamespace.Select(v => new Module(v.Key, v)).ToList();
                return modules;
            }
        }
    }
}