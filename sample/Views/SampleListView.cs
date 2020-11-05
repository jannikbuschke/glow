using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Glow.Core.Views;
using Microsoft.AspNetCore.Mvc;

namespace Glow.Sample.Views
{
    public class ListViewItem
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
    }

    [Route("api/list-view/data")]
    public class SampleListView : ListView<ListViewItem>
    {
        private static readonly List<ListViewItem> data = new Faker<ListViewItem>()
            .RuleFor(v => v.Id, f => Guid.NewGuid())
            .RuleFor(v => v.DisplayName, f => f.Person.UserName)
            .Generate(1000);

        protected override IQueryable<ListViewItem> Get()
        {
            return data.AsQueryable();
        }
    }
}
