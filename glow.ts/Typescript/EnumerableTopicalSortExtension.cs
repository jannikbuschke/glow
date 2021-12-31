using System;
using System.Collections.Generic;

namespace Glow.Core.Typescript
{
    public class CyclicDependencyException : Exception
    {
        public CyclicDependencyException(string msg) : base(msg) { }
    }

    public static class EnumerableTopicalSortExtension
    {
        public static IList<T> TopologicalSort<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> getDependencies
        ) where T : TsType
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (T item in source)
            {
                try
                {
                    Visit(item, getDependencies, sorted, visited);
                }
                catch (CyclicDependencyException) { }
            }

            return sorted;
        }

        public static void Visit<T>(
            T item,
            Func<T, IEnumerable<T>> getDependencies,
            List<T> sorted,
            Dictionary<T, bool> visited
        ) where T : TsType
        {
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    // foreach (var v in visited)
                    // {
                    //     v.Key.HasCyclicDependency = true;
                    // }
                    item.HasCyclicDependency = true;
                    throw new CyclicDependencyException($"Cyclic dependency found. ({item.Name})");
                }
            }
            else
            {
                visited[item] = true;

                IEnumerable<T> dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (T dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}