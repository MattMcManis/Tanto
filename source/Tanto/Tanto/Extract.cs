/* ----------------------------------------------------------------------
Tanto
Copyright (C) 2018, 2019 Matt McManis
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
                //Regex regex = new Regex(@"\b(S\d\d\d?E\d\d\d?)\b\s*", RegexOptions.IgnoreCase);
                Regex regex = new Regex(@"\b(S\d+E\d+)\b\s*", RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[0]));
                string SeasonEpisode = string.Empty;
                if (matches.Count > 0)
                {
                    SeasonEpisode = matches[0].Value.ToString();
                }

                // S00
                //regex = new Regex(@"(S\d\d\d?)\s*", RegexOptions.IgnoreCase);
                regex = new Regex(@"(S\d+)\s*", RegexOptions.IgnoreCase);
                matches = regex.Matches(SeasonEpisode);
                string season = string.Empty;
                if (matches.Count > 0)
                {
                    season = matches[0].Value.ToString();
                }

                // 00
                //regex = new Regex(@"(\d\d\d?)\s*", RegexOptions.IgnoreCase);
                regex = new Regex(@"(\d+)\s*", RegexOptions.IgnoreCase);
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
        ///    Extract Episode Number
        /// </summary>
        public static void ExtractEpisodeNumber(MainWindow mainwindow)
        {
            try
            {
                List<string> seasonEpisodeNumbersList = new List<string>();
                List<string> episodeNumbersList = new List<string>();
                List<int> numbersList = new List<int>();

                // S00E00
                //Regex regex = new Regex(@"\b(S\d\d\d?E\d\d\d?)\b\s*", RegexOptions.IgnoreCase);
                //Regex regex = new Regex(@"\b(S\d+E\d+)\b\s*", RegexOptions.IgnoreCase);
                Regex regex = new Regex(@"(S\d+E\d+)", RegexOptions.IgnoreCase);

                for (var i = 0; i < MainWindow.listFilePaths.Count; i++)
                {
                    MatchCollection matches = regex.Matches(Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[i]));

                    if (matches.Count > 0)
                    {
                        if (Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[i]).Contains(matches[0].Value.ToString()))
                        {
                            seasonEpisodeNumbersList.Add(matches[0].Value.ToString());
                        }
                    }
                }


                // E00
                //regex = new Regex(@"(E\d+)\s*", RegexOptions.IgnoreCase);
                regex = new Regex(@"(E\d+)", RegexOptions.IgnoreCase);

                for (var i = 0; i < seasonEpisodeNumbersList.Count; i++)
                {
                    MatchCollection matches = regex.Matches(seasonEpisodeNumbersList[i]);

                    if (matches.Count > 0)
                    {
                        if (seasonEpisodeNumbersList[i].Contains(matches[0].Value.ToString()))
                        {
                            episodeNumbersList.Add(matches[0].Value.ToString());
                        }
                    }
                }

                // 00
                //regex = new Regex(@"(\d+)\s*", RegexOptions.IgnoreCase);
                regex = new Regex(@"(\d+)", RegexOptions.IgnoreCase);

                for (var i = 0; i < episodeNumbersList.Count; i++)
                {
                    MatchCollection matches = regex.Matches(episodeNumbersList[i]);

                    if (matches.Count > 0)
                    {
                        if (episodeNumbersList[i].Contains(matches[0].Value.ToString()))
                        {
                            numbersList.Add(int.Parse(matches[0].Value));
                        }
                    }
                }

                // Update Textbox
                if (numbersList.Count > 0)
                {
                    int lowestNumber = numbersList.Min(c => c);
                    //string highestNumber = numbersList.Max(c => c).ToString();

                    //int highestNumberLength = highestNumber.Length;

                    //MessageBox.Show(string.Join("\n", episodeNumbersList)); //debug
                    //MessageBox.Show(lowestNumber.ToString()); //debug

                    List<string> listFileNames = new List<string>();
                    string FileNames_Count = MainWindow.listFilePaths.Count.ToString();
                    int padLength = FileNames_Count.Length;


                    // 0 Padding
                    if (padLength <= 1)
                    {
                        mainwindow.tbxStartEpisodeAt.Text = lowestNumber.ToString().PadLeft(2, '0');
                    }
                    else if (padLength > 1)
                    {
                        mainwindow.tbxStartEpisodeAt.Text = lowestNumber.ToString().PadLeft(padLength, '0');
                    }
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
            Regex regex = new Regex(@"\((\d\d\d\d)\)");
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

            // -------------------------
            // Remove Title Tags
            // -------------------------
            string title = Regex.Replace(
                matchingPattern.ToString()
                , @"(?i)(\[).*?(\])"
                , ""
                , RegexOptions.IgnoreCase
            );

            return title;
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
