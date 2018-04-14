﻿/* ----------------------------------------------------------------------
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Tanto
{
    public partial class Extract
    {
        /// <summary>
        ///    Extract Series Title
        /// </summary>
        public static void ExtractSeriesTitle(MainWindow mainwindow)
        {
            var prefix = FindMatchingPattern(
                        Filter.SeriesTitleFilter(mainwindow, Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[0])),
                        Filter.SeriesTitleFilter(mainwindow, Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[1])), true);

            string seriesTitle = prefix;

            // Update Textbox
            if (!string.IsNullOrEmpty(seriesTitle))
            {
                mainwindow.tbxTitle.Text = seriesTitle.Trim();
            }
            
        }

        /// <summary>
        ///    Extract Season Number
        /// </summary>
        public static void ExtractSeasonNumber(MainWindow mainwindow)
        {
            try
            {
                // S00E00
                Regex regex = new Regex(@"\b(S\d\d\d?E\d\d\d?)\b\s*", RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[0]));
                string SeasonEpisode = string.Empty;
                if (matches.Count > 0)
                {
                    SeasonEpisode = matches[0].Value.ToString();
                }

                // S00
                regex = new Regex(@"(S\d\d\d?)\s*", RegexOptions.IgnoreCase);
                matches = regex.Matches(SeasonEpisode);
                string season = string.Empty;
                if (matches.Count > 0)
                {
                    season = matches[0].Value.ToString();
                }

                // 00
                regex = new Regex(@"(\d\d\d?)\s*", RegexOptions.IgnoreCase);
                matches = regex.Matches(season);
                string number = string.Empty;
                if (matches.Count > 0)
                {
                    number = matches[0].Value.ToString().TrimStart('0');
                }

                // Update Textbox
                if (!string.IsNullOrEmpty(number))
                {
                    mainwindow.tbxSeason.Text = number;
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Extract Year
        /// </summary>
        public static String ExtractYear(string filename)
        {
            // (2018)
            Regex regex = new Regex(@"\(.*?(\d+).*\)");
            MatchCollection matches = regex.Matches(filename);

            string year = string.Empty;

            if (matches.Count > 0)
            {
                year = matches[0].Value.ToString();

                // remove parentheses
                year = Regex.Replace(year, @"([()])", "");
            }

            return year;
        }



        /// <summary>
        ///    Extract Episode Name
        /// </summary>
        public static void ExtractEpisodeNames(List<string> listFileNames, List<string> listEpisodeNames)
        {
            var filenames = GetList(listFileNames);
            var padLength = filenames.Max(t => t.Max(s => s.Length)) + 2;

            //MessageBox.Show(string.Join("\n", MainWindow.listFileNames)); //debug

            for (var i = 0; i < filenames.Count; i++)
            {
                var trimmedFilenames = RemoveCommonPrefixAndSuffix(filenames[i]);

                for (var j = 0; j < trimmedFilenames.Count; j++)
                {
                    listEpisodeNames.Add(trimmedFilenames[j]);

                    //MessageBox.Show(trimmedFilenames[j]); //debug
                }
            }
        }

        /// <summary>
        ///    Extract Series Title - Get Common Word Sequences
        /// </summary>
        private static string FindMatchingPattern(string sample1, string sample2, bool forwardDirection)
        {
            string shorter = string.Empty;
            string longer = string.Empty;

            if (sample1.Length <= sample2.Length)
            {
                shorter = sample1;
                longer = sample2;
            }
            else
            {
                shorter = sample2;
                longer = sample1;
            }

            StringBuilder matchingPattern = new StringBuilder();
            StringBuilder wordHolder = new StringBuilder();

            if (forwardDirection)
            {
                for (int idx = 0; idx < shorter.Length; idx++)
                {
                    if (shorter[idx] == longer[idx])
                        if (shorter[idx] == ' ')
                        {
                            matchingPattern.Append(wordHolder + " ");
                            wordHolder.Clear();
                        }
                        else
                            wordHolder.Append(shorter[idx]);
                    else
                        break;
                }
            }
            else
            {
                while (true)
                {
                    if (shorter.Length > 0 && shorter[shorter.Length - 1] == longer[longer.Length - 1])
                    {
                        if (shorter[shorter.Length - 1] == ' ')
                        {
                            matchingPattern.Insert(0, " " + wordHolder);
                            wordHolder.Clear();
                        }
                        else
                            wordHolder.Insert(0, shorter[shorter.Length - 1]);

                        shorter = shorter.Remove(shorter.Length - 1, 1);
                        longer = longer.Remove(longer.Length - 1, 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return matchingPattern.ToString();
        }



        /// <summary>
        ///    Extract Episode Name - RemoveCommonPrefixAndSuffix
        /// </summary>
        private static List<List<string>> GetList(List<string> fileList)
        {
            return new List<List<string>>
            {
                fileList,
            };
        }
        public static List<string> RemoveCommonPrefixAndSuffix(List<string> filenames, int minSeqenceLength = 2)
        {
            if (filenames == null) return null;

            if (filenames.All(x => x == filenames.First())) return new List<string>();

            if (filenames.Count < 2 || 
                filenames.Any(s => s.Count(c => c == ' ') < minSeqenceLength - 1))
            {
                return filenames.ToList();
            }

            if (filenames.All(s => s == filenames[0]))
            {
                return filenames.Select(s => string.Empty).ToList();
            }

            var filenamesWords = filenames.Select(s => s.Split()).ToList();
            var firstFileName = filenamesWords[0];
            var length = filenamesWords.Min(s => s.Length);
            var commonPrefix = new StringBuilder();
            var commonSuffix = new StringBuilder();
            var prefixDone = false;
            var suffixDone = false;

            for (var i = 0; i < length && !(prefixDone && suffixDone); i++)
            {
                if (!prefixDone && filenamesWords.All(s => s[i] == firstFileName[i]))
                {
                    commonPrefix.Append(firstFileName[i] + " ");
                }
                else
                {
                    prefixDone = true;
                }

                if (!suffixDone && filenamesWords.All(s =>
                    s[s.Length - i - 1] == firstFileName[firstFileName.Length - i - 1]))
                {
                    commonSuffix.Insert(0, firstFileName[firstFileName.Length - i - 1] + " ");
                }
                else
                {
                    suffixDone = true;
                }
            }

            var prefix = commonPrefix.ToString().Count(c => c == ' ') >= minSeqenceLength - 1
                ? commonPrefix.ToString()
                : string.Empty;

            var suffix = commonSuffix.ToString().Count(c => c == ' ') >= minSeqenceLength - 1
                ? commonSuffix.ToString()
                : string.Empty;

            var commonLength = prefix.Length + suffix.Length;

            return filenames
                .Select(s => s.Length > commonLength
                    ? s.Substring(prefix.Length, s.Length - prefix.Length - suffix.Length)
                    : string.Empty)
                .ToList();
        }       



        /// <summary>
        ///    Extract Spacing
        /// </summary>
        public static String ExtractSpacing(string filename)
        {
            int periods = filename.Count(c => c == '.');
            int underscores = filename.Count(c => c == '_');
            int dashes = filename.Count(c => c == '-');

            string common = string.Empty;

            // Periods largest
            if (periods > underscores && periods > dashes)
            {
                common = ".";
            }
            // Underscores largest
            else if (underscores > periods && underscores > dashes)
            {
                common = "_";
            }
            // Dashes largest
            else if (dashes > periods && dashes > underscores)
            {
                common = "-";
            }

            return common;
        }

    }
}