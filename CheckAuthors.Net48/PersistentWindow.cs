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

using CheckAuthors.Net48.Properties;

using System.Linq;
using System.Windows.Forms;

namespace CheckAuthors.Net48
{
    static class PersistentWindow
    {
        public static void Restore(Form form)
        {
            var p = Settings.Default;

            if (p.IsMaximized)
            {
                form.WindowState = FormWindowState.Maximized;
            }
            else if (Screen.AllScreens.Any(screen =>
                screen.WorkingArea.IntersectsWith(p.WindowPosition)))
            {
                form.StartPosition = FormStartPosition.Manual;
                form.DesktopBounds = p.WindowPosition;
                form.WindowState = FormWindowState.Normal;
            }

            //form.Font = p.Font;
        }

        public static void Save(Form form)
        {
            var p = Settings.Default;

            p.IsMaximized = form.WindowState == FormWindowState.Maximized;
            p.WindowPosition = form.DesktopBounds;

            //p.Font = form.Font;

            p.Save();
        }
    }
}
