using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly List<ListViewItem> data = new List<ListViewItem>
            {
                new ListViewItem { Id = Guid.NewGuid(), DisplayName = "Item1" },
                new ListViewItem { Id = Guid.NewGuid(), DisplayName = "Item2" },
                new ListViewItem { Id = Guid.NewGuid(), DisplayName = "Item3" }
            };

        protected override IQueryable<ListViewItem> Get()
        {
            return data.AsQueryable();
        }
    }
}
