using System;
using System.Collections.Generic;
using Bogus;

namespace Glow.Core.FakeData
{
    public abstract class FakeBase<T> where T : class
    {
        public List<T> Data { get; private set; }

        public FakeBase(Func<Faker<T>, List<T>> func)
        {
            var faker = new Faker<T>();
            faker.UseSeed(10);
            Data = func(faker);
        }
    }
}