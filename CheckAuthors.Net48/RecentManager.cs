#region License
//------------------------------------------------------------------------------
// Copyright (c) Dmitrii Evdokimov
// Source https://github.com/diev/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace CheckAuthors.Net48
{
    internal class RecentManager
    {
        private readonly List<string> _list = new List<string>();
        private readonly ToolStripMenuItem _menu;
        private readonly EventHandler _handler;

        public int Capacity { get; set; } = 10;

        public StringCollection Items
        {
            get
            {
                var collection = new StringCollection();
                collection.AddRange(_list.ToArray());
                return collection;
            }
            set
            {
                if (value != null)
                {
                    foreach (string item in value)
                    {
                        Add(item);
                    }
                }
            }
        }

        public RecentManager(ToolStripMenuItem menu, EventHandler handler, StringCollection items)
        {
            _menu = menu;
            _handler = handler;
            Items = items;
        }

        public void Add(string path)
        {
            int index = _list.IndexOf(path);

            if (index == -1)
            {
                if (_list.Count == Capacity)
                {
                    _list.RemoveAt(Capacity - 1);
                }

                _list.Insert(0, path);
            }
            else if (index > 0)
            {
                _list.RemoveAt(index);
                _list.Insert(0, path);
            }

            var items = _menu.DropDownItems;
            items.Clear();

            for (int i = 0; i < _list.Count; i++)
            {
                //var item = new ToolStripMenuItem
                //{
                //    Text = _list[i]
                //};

                //item.Click += new EventHandler(_handler);

                var item = new ToolStripMenuItem(_list[i], null, _handler);
                items.Add(item);
            }

            _menu.Enabled = _menu.HasDropDownItems;
        }
    }
}
