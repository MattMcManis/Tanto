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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace Tanto
{
    public partial class Rename
    {
        /// <summary>
        ///    Rename (Method)
        /// </summary>
        public static void Renamer(MainWindow mainwindow)
        {
            // -------------------------
            // Create Lists
            // -------------------------

            List<string> listFileNames = new List<string>();

            int FileNames_Count = MainWindow.listFilePaths.Count;

            // Add File Names to List
            for (var i = 0; i < FileNames_Count; i++)
            {
                string filename = Filter.FilterFileName(mainwindow, Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[i]));
                listFileNames.Add(filename);

                //MessageBox.Show(MainWindow.listFileNames[i]); //debug
                //MessageBox.Show(MainWindow.listFileNames[0]); //debug
            }


            // Extract Episode Names
            List<string> listEpisodeNames = new List<string>();

            if (MainWindow.th != null)
            {
                MainWindow.th.Abort();
            }
            MainWindow.th = new Thread(() => Extract.ExtractEpisodeNames(listFileNames, listEpisodeNames));
            MainWindow.th.IsBackground = true;
            MainWindow.th.Start();
            MainWindow.th.Join();


            //MessageBox.Show(string.Join("\n", listEpisodeNames)); //debug


            // Clear New File Names
            if (MainWindow.listNewFileNames != null
                && MainWindow.listNewFileNames.Count > 0)
            {
                MainWindow.listNewFileNames.Clear();
                MainWindow.listNewFileNames.TrimExcess();
            }

            // -------------------------
            // Rename each file in ListView
            // -------------------------
            int ep = 0;
            // Start Episode At Number
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxStartEpisodeAt.Text))
            {
                ep = Convert.ToInt32(mainwindow.tbxStartEpisodeAt.Text) - 1;
            }

            for (var i = 0; i < MainWindow.listFilePaths.Count; i++)
            {
                // -------------------------
                // File
                // -------------------------
                string dir = Path.GetDirectoryName(MainWindow.listFilePaths[i]).TrimEnd('\\') + @"\";
                string ext = Path.GetExtension(MainWindow.listFilePaths[i]);

                // --------------------------------------------------
                // New Name
                // --------------------------------------------------

                // -------------------------
                // Title
                // -------------------------
                string title = mainwindow.tbxTitle.Text;

                // -------------------------
                // Year
                // -------------------------
                string year = string.Empty;
                if (!string.IsNullOrWhiteSpace(mainwindow.tbxYear.Text))
                {
                    year = "(" + mainwindow.tbxYear.Text + ")";
                }

                // -------------------------
                // Season
                // -------------------------
                string season = string.Empty;
                if (!string.IsNullOrWhiteSpace(mainwindow.tbxSeason.Text))
                {
                    season = "S" + mainwindow.tbxSeason.Text.PadLeft(2, '0');
                }

                // -------------------------
                // Episode Number
                // -------------------------
                // File List Count
                string episodeNumber = string.Empty;
                // User Input TextBox - Count Digits
                int epCount = mainwindow.tbxStartEpisodeAt.Text.ToString().Length;

                if (mainwindow.cbxEpisodeNumbering.IsChecked == true)
                {
                    Regex regex = new Regex(@"(E\d+E\d+)");
                    MatchCollection matches = regex.Matches(listFileNames[i]);

                    string doubleEpisodeNumbersMatch = string.Empty;

                    // -------------------------
                    // Check for Double Episode
                    // -------------------------
                    if (mainwindow.cbxDoubleEpisodeNumbers.IsChecked == true)
                    {
                        if (matches.Count > 0)
                        {
                            doubleEpisodeNumbersMatch = matches[0].Value.ToString();
                        }
                    }


                    // -------------------------
                    // Single Episode - S01E01
                    // -------------------------
                    if (string.IsNullOrEmpty(doubleEpisodeNumbersMatch))
                    {
                        // Episode count less than 100 (no padding, 1 instead of 01)
                        if (FileNames_Count < 100 &&
                            epCount <= 1)
                        {
                            ep++; // add 1 to filename
                            episodeNumber = "E" + ep.ToString();
                        }
                        // Episode count greater than 99
                        else if (FileNames_Count >= 100 ||
                            epCount > 1)
                        {
                            ep++; // add 1 to filename
                            episodeNumber = "E" + ep.ToString().PadLeft(epCount, '0');
                        }
                    }

                    // -------------------------
                    // Double Episode - S01E01E02
                    // -------------------------
                    else if (!string.IsNullOrEmpty(doubleEpisodeNumbersMatch) &&
                            mainwindow.cbxDoubleEpisodeNumbers.IsChecked == true)
                    {
                        // Episode count less than 100 (no padding, 1 instead of 01)
                        if (FileNames_Count < 100 &&
                            epCount <= 1)
                        {
                            // 1st ep
                            ep++; // add 1 to filename
                            episodeNumber = "E" + ep.ToString();

                            // 2nd ep
                            ep++;
                            episodeNumber = episodeNumber + "E" + ep.ToString();
                        }
                        // Episode count greater than 99
                        else if (FileNames_Count >= 100 ||
                                 epCount > 1)
                        {
                            // 1st ep
                            ep++; // add 1 to filename
                            episodeNumber = "E" + ep.ToString().PadLeft(epCount, '0');

                            // 2nd ep
                            ep++;
                            episodeNumber = episodeNumber + "E" + ep.ToString().PadLeft(epCount, '0');
                        }
                    }
                }


                // -------------------------
                // Episode Name
                // -------------------------
                string episodeName = string.Empty;
                if (mainwindow.cbxEpisodeNames.IsChecked == true)
                {
                    try
                    {
                        if (listEpisodeNames.Count > 0)
                        {
                            episodeName = Filter.FilterRemove(mainwindow, listEpisodeNames[i]);

                            episodeName = Filter.FilterEpisodeName(mainwindow, episodeName);
                        }

                        //MessageBox.Show(string.Join("\n", MainWindow.listEpisodeNames)); //debug
                        //MessageBox.Show(string.Join("\n", MainWindow.listFileNames)); //debug
                        //MessageBox.Show(MainWindow.listEpisodeNames[i]); //debug
                        //MessageBox.Show(episodeName); //debug
                    }
                    catch
                    {

                    }
                }

                // -------------------------
                // Resolution
                // -------------------------
                string resolution = mainwindow.tbxResolution.Text;

                // -------------------------
                // Format
                // -------------------------
                string format = mainwindow.tbxFormat.Text;

                // -------------------------
                // Audio Channels
                // -------------------------
                string channels = mainwindow.tbxAudioChannels.Text;

                // -------------------------
                // Language
                // -------------------------
                string language = mainwindow.tbxLanguage.Text;

                // -------------------------
                // Subtitles
                // -------------------------
                string subtitles = mainwindow.tbxSubtitles.Text;

                // -------------------------
                // Encoding
                // -------------------------
                string encoding = mainwindow.tbxEncoding.Text;

                // -------------------------
                // Misc
                // -------------------------
                string misc = mainwindow.tbxMisc.Text;

                // -------------------------
                // Separator
                // -------------------------
                string separator = mainwindow.tbxSeparator.Text;

                // -------------------------
                // Combine
                // -------------------------
                //
                List<string> properties = new List<string>()
                {
                    title,
                    year,
                    season + episodeNumber,
                    episodeName,
                    resolution,
                    format,
                    channels,
                    language,
                    subtitles,
                    encoding,
                    misc,
                };

                // Join List with Separator
                string newFilename = string.Join(separator, properties
                                           .Where(s => !string.IsNullOrEmpty(s))
                                     );

                // -------------------------
                // Filter Replace
                // -------------------------
                newFilename = Filter.FilterReplace(mainwindow, newFilename);

                // -------------------------
                // Filter Finalize
                // -------------------------
                newFilename = Filter.FilterFinalize(mainwindow, newFilename).Trim();

                // -------------------------
                // New File Names List
                // -------------------------
                //MainWindow.listNewFileNames.Add(newFilename);
                MainWindow.listNewFileNames.Add(dir + newFilename + ext);

                //MessageBox.Show(string.Join("\n", listFilePaths)); //debug
                //MessageBox.Show(string.Join("\n", listFileNames)); //debug
                //MessageBox.Show(string.Join("\n", listNewFileNames)); //debug
            }

            // -------------------------
            // Close Thread
            // -------------------------
            MainWindow.th.Abort();
        }



        /// <summary>
        ///    Duplicate Append (Method)
        /// </summary>
        public static String DuplicateAppend(string file)
        {
            string dir = Path.GetDirectoryName(file).TrimEnd('\\') + @"\";
            string filename = Path.GetFileNameWithoutExtension(file);
            string ext = Path.GetExtension(file);

            string newFilename = string.Empty;

            int count = 1;

            if (File.Exists(file))
            {
                while (File.Exists(file))
                {
                    newFilename = string.Format("{0}({1})", filename + " ", count++);
                    file = Path.Combine(dir, newFilename + ext);
                }
            }

            return file;
        }
    }
}
