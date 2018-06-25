/* ----------------------------------------------------------------------
Tanto
Copyright (C) 2018 Matt McManis
http://github.com/MattMcManis/Tanto
mattmcmanis@outlook.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see <http://www.gnu.org/licenses/>. 
---------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tanto
{
    public partial class Sort
    {
        /// <summary>
        ///    Sort (Method)
        /// </summary>
        public static void Sorting(MainWindow mainwindow)
        {
            // -------------------------
            // File Paths
            // -------------------------
            //MainWindow.listFilePaths.Sort();

            // Natural Sorting
            MainWindow.listFilePaths = MainWindow.listFilePaths.OrderBy(x => Regex.Replace(x, @"\d+", n => n.Value.PadLeft(5, '0'))).ToList();
        }
    }

}
