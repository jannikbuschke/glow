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
                var all = All().ToList();

                var t0  = all.Where(v => v.IsT0).Select(v => v.AsT0).ToList();

                IEnumerable<IGrouping<string, OneOf<TsType, TsEnum>>> byNamespace =
                    all.GroupBy(v => v.Match(v1 => v1.Namespace, v2 => v2.Namespace));

                var modules = byNamespace.Select(v => new Module(v.Key, v)).ToList();
                var tsTypes = modules.SelectMany(v => v.TsTypes).ToList();

                foreach (TsType v in tsTypes)
                {
                    t0.Remove(v);
                }

                if (t0.Count > 0)
                {
                    foreach (TsType v in t0)
                    {
                        if (v.IsCollection)
                        {

                        }
                        else
                        {
                        Console.WriteLine("Type went missing:  " + v.FullName + " " + v.Id);
                        }
                    }
                }

                return modules;
            }
        }
    }
}