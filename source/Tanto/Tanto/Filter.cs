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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Tanto
{
    public partial class Filter
    {
        /// <summary>
        ///    Filter File Name (Method)
        /// </summary>
        /// <remarks>
        ///    This filter is applied to Original File Name.
        /// </remarks>
        public static String FilterFileName(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Keep Original Spacing (Separate)
            // -------------------------
            // Example.01.The.Episode.Name.(1080p).Blu-Ray.mp4
            // to
            // Example . 01 . The . Episode . Name . (1080p) . Blu-Ray .mp4
            if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text))
                {
                    try
                    {
                        string character = @"\" + mainwindow.tbxSpacing.Text;
                        filename = Regex.Replace(filename, character, " " + mainwindow.tbxSpacing.Text + " ");
                    }
                    catch
                    {
                        MessageBox.Show("Could not replace spacing.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }

            // -------------------------
            // Remove Period Spacing
            // -------------------------
            if (mainwindow.cbxFilterPeriodSpacing.IsChecked == true)
            {
                try
                {
                    // replace with space
                    string regex = @"(?<!\.)\.(?!\.)";
                    filename = Regex.Replace(filename, regex, " ");
                }
                catch
                {
                    MessageBox.Show("Could not remove periods.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Underscore Spacing
            // -------------------------
            if (mainwindow.cbxFilterUnderscoreSpacing.IsChecked == true)
            {
                // replace with space
                filename = Regex.Replace(filename, "_", " ");
            }

            // -------------------------
            // Remove Dash Spacing
            // -------------------------
            if (mainwindow.cbxFilterDashSpacing.IsChecked == true
                && mainwindow.cbxFilterHyphens.IsChecked == false)
            {
                // replace with space
                filename = Regex.Replace(filename, "-", " "); 
            }

            // -------------------------
            // Remove All Dashes
            // -------------------------
            if (mainwindow.cbxFilterDashes.IsChecked == true
                && mainwindow.cbxFilterHyphens.IsChecked == false)
            {
                // remove dash
                filename = Regex.Replace(filename, "-", "").Trim();
            }
            // -------------------------
            // Remove Dashes Between - Words
            // Preserve Hyphenated-Words
            // -------------------------
            else if (mainwindow.cbxFilterDashes.IsChecked == true
                && mainwindow.cbxFilterHyphens.IsChecked == true)
            {
                // remove dash, preserve hyphens
                //filename = Regex.Replace(filename, @"((?<=[\s\.])\-+)|(\-+(?=[\s\.]))", "").Trim();
                filename = Regex.Replace(filename, @"\b(-+)\b|-", "$1").Trim();
                // multiple dashes to single dash
                filename = Regex.Replace(filename, "[-]{2,}", "").Trim();
            }

            // -------------------------
            // Remove Year
            // -------------------------
            if (mainwindow.cbxFilterRemoveYear.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"(\((1|2)\d?\d?\d?\))", "");
            }

            //// -------------------------
            //// Remove Episode Numbering
            //// -------------------------
            //if (mainwindow.cbxFilterRemoveEpisodeNumbering.IsChecked == true)
            //{
            //    //S000E000, EP000, E000, 000x000, 000v000, 000, 000v0-9
            //    filename = Regex.Replace(
            //                    filename
            //                    , @"(?i)\b(S\d\d\d?E\d\d\d?|(EP|EP((\s*)|-))\d\d\d?|E\d\d\d?|(?<![.])\d\d?\d?|(E(\s*)|EP(\s*))?(?<![.])\d\d?\d?(x|v)\d\d?\d?)\b"
            //                    , ""
            //                    , RegexOptions.IgnoreCase
            //                    );
            //}

            // -------------------------
            // Remove Double Spaces
            // -------------------------
            // Use at the very end of all filters
            if (mainwindow.cbxFilterDoubleSpaces.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"\s+", " ");
            }


            return filename;
        }

        /// <summary>
        ///    Filter Episode Name (Method)
        /// </summary>
        public static String FilterEpisodeName(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Title Case
            // -------------------------
            if (mainwindow.cbxFilterTitleCase.IsChecked == true)
            {
                string input = filename;
                int minLength = 2;
                string regexPattern = string.Format(@"(?<=(^|[.!?])\s*)\w|\b\w(?=[-\w]{{{0}}})", minLength);
                filename = Regex.Replace(input, regexPattern, m => m.Value.ToUpperInvariant());
            }

            return filename;
        }

        /// <summary>
        ///    Filter Remove (Method)
        /// </summary>
        /// <remarks>
        ///    This filter is applied to Episode Name.
        /// </remarks>
        public static String FilterRemove(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Remove Episode Numbering
            // -------------------------
            if (mainwindow.cbxFilterRemoveEpisodeNumbering.IsChecked == true)
            {
                //S000E000, EP000, E000, 000x000, 000v000, 000, 000v0-9
                filename = Regex.Replace(
                                filename
                                //, @"(?i)\b(S\d\d\d?E\d\d\d?|(EP|EP((\s*)|-))\d\d\d?|E\d\d\d?|(?<![.])\d\d?\d?|(E(\s*)|EP(\s*))?(?<![.])^\d\d?\d?(x|v)\d\d?\d?)\b" //old
                                , @"(?i)\b(S\d+E\d+)|(Episode(\s*)|(EP|EP((\s*)|-))\d+|E\d+|(?<![.])^\d+|(E(\s*)|EP(\s*))?(?<![.])\d\d?\d?(x|v)\d\d?\d?)\b"
                                , ""
                                , RegexOptions.IgnoreCase
                                );
            }

            // -------------------------
            // Remove Words
            // -------------------------
            if (mainwindow.tbxRemoveWords.Text != string.Empty)
            {
                try
                {
                    string words = mainwindow.tbxRemoveWords.Text.Replace(",", "|");

                    string regex = @"\b(" + words + @")\b";

                    filename = Regex.Replace(filename, Regex.Escape(regex), "");
                }
                catch
                {
                    MessageBox.Show("Could not remove words.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Characters
            // -------------------------
            if (mainwindow.tbxRemoveChars.Text != string.Empty)
            {
                try
                {
                    //string characters = mainwindow.tbxRemoveChars.Text.Replace(",", "|");
                    //string regex = characters;
                    //filename = Regex.Replace(filename, Regex.Escape(regex), "");

                    List<string> removeChars = new List<string>(mainwindow.tbxRemoveChars.Text.Split(','));

                    List<string> removeCharsEscaped = new List<string>();

                    // Escape special characters in list
                    foreach (string character in removeChars)
                    {
                        removeCharsEscaped.Add(Regex.Escape(character));
                    }

                    // Separate with |
                    string regex = string.Join("|", removeCharsEscaped
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .Where(s => !s.Equals("\n"))
                                        .Where(s => !s.Equals("\r\n"))
                                        );

                    filename = Regex.Replace(filename, regex, ""); // do not Regex.Escape()
                }
                catch
                {
                    MessageBox.Show("Could not remove characters.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Between Characters
            // -------------------------
            if (mainwindow.tbxRemoveRangeStart.Text != string.Empty
                && mainwindow.tbxRemoveRangeEnd.Text != string.Empty)
            {
                try
                {
                    string start = mainwindow.tbxRemoveRangeStart.Text;
                    string end = mainwindow.tbxRemoveRangeEnd.Text;

                    string regex = "(\\" + start + ".*\\" + end + ")";

                    filename = Regex.Replace(filename, regex, ""); // do not Regex.Escape()
                }
                catch
                {
                    MessageBox.Show("Could not remove between characters.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Tags
            // -------------------------
            if (mainwindow.cbxFilterRemoveTags.IsChecked == true)
            {
                filename = Regex.Replace(
                filename
                    , @"(?i)(\[).*?(\])|(\(*)(480p|576p|720p|1080p|2k|4k|8k|60fps|DVD|BD|BRD|Bluray|Blu-Ray|WebDL|Web-DL|WebRip|Web-Rip|RAW|HEVC|(h|x)(265|264)|AV1|VP8|VP9|Theora|AAC|AC3|Vorbis|Opus|FLAC|MP3|(DTS\s?|DD\s?)(5\.1|7\.1)(\s?CH)?|(2|6|5\.1|7\.1)\s?CH|Dual-Audio|Multi-Sub|English Dub)(\)*)(\,*)"
                    , ""
                    , RegexOptions.IgnoreCase
                );
            }

            // -------------------------
            // Remove Double Spaces
            // -------------------------
            // Use at the very end of all filters
            // Also in File Name Filter
            if (mainwindow.cbxFilterDoubleSpaces.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"\s+", " ");
            }

            filename = filename.Trim();

            return filename;
        }


        /// <summary>
        ///    Filter Replace (Method)
        /// </summary>
        /// <remarks>
        ///    This filter is applied to New File Name.
        /// </remarks>
        public static String FilterReplace(MainWindow mainwindow, string filename)
        {
            // Replace These Words With
            if (mainwindow.tbxReplaceWords.Text != string.Empty)
            {
                try
                {
                    string words = mainwindow.tbxReplaceWords.Text.Replace(",", "|");
                    string replacement = mainwindow.tbxReplaceWordsWith.Text;

                    string regex = @"\b(" + words + @")\b";

                    filename = Regex.Replace(filename, Regex.Escape(regex), Regex.Escape(replacement));
                }
                catch
                {
                    MessageBox.Show("Could not replace words.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // Replace These Characters With
            if (mainwindow.tbxReplaceChars.Text != string.Empty)
            {
                try
                {
                    string characters = mainwindow.tbxReplaceChars.Text.Replace(",", "|");
                    string replacement = mainwindow.tbxReplaceCharsWith.Text;

                    string regex = characters;

                    filename = Regex.Replace(filename, Regex.Escape(regex), Regex.Escape(replacement));
                }
                catch
                {
                    MessageBox.Show("Could not replace characters.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // Replace Spaces With
            if (mainwindow.tbxReplaceSpacesWith.Text != string.Empty)
            {
                try
                {
                    string replacement = mainwindow.tbxReplaceSpacesWith.Text;

                    filename = Regex.Replace(filename, " ", Regex.Escape(replacement));
                }
                catch
                {
                    MessageBox.Show("Could not replace spaces.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            filename = filename.Trim();

            return filename;
        }


        /// <summary>
        ///    Serites Title Filter (Method)
        /// </summary>
        public static String SeriesTitleFilter(MainWindow mainwindow, string filename)
        {
            //MessageBox.Show(filename); //debug

            // -------------------------
            // Keep Original Spacing (Separate)
            // -------------------------
            // Example.01.The.Episode.Name.(1080p).Blu-Ray.mp4
            // to
            // Example . 01 . The . Episode . Name . (1080p) . Blu-Ray .mp4
            if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text))
                {
                    try
                    {
                        string character = @"\" + mainwindow.tbxSpacing.Text; // to remove
                        filename = Regex.Replace(filename, character, " " + mainwindow.tbxSpacing.Text + " ");
                    }
                    catch
                    {
                        MessageBox.Show("Could not replace spacing.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }

            //MessageBox.Show(filename); //debug

            // -------------------------
            // Remove Year
            // -------------------------
            filename = Regex.Replace(filename, @"\((\d\d\d\d)\)", "");

            // -------------------------
            // Remove Strange Episode Abbreviations
            // -------------------------
            filename = Regex.Replace(filename, @"(?!\.)", "");
            filename = Regex.Replace(filename, @"\b(Ep\.)\B", "", RegexOptions.IgnoreCase);

            // -------------------------
            // Remove Spacing
            // -------------------------
            //if (mainwindow.cbxFilterOriginalSpacing.IsChecked == false)
            //{
                // Period Spacing
                filename = Regex.Replace(filename, @"(?<!\.)\.(?!\.)", " ");

                // Underscore Spacing
                filename = Regex.Replace(filename, "_", " ");
            //}

            // -------------------------
            // Remove Dash, Preserve Hyphens
            // -------------------------
            filename = Regex.Replace(filename, @"((?<=[\s\.])\-+)|(\-+(?=[\s\.]))", "").Trim();
            // Multiple Dashes to Single Dash
            filename = Regex.Replace(filename, "[-]{2,}", "").Trim();

            // -------------------------
            // Remove Double Space
            // -------------------------
            filename = Regex.Replace(filename, @"\s+", " ");

            filename = filename.Trim();


            return filename;
        }


        /// <summary>
        ///    Finalize Filter (Method)
        /// </summary>
        /// <remarks>
        ///    Applied to the New File Name.
        /// </remarks>
        public static String FilterFinalize(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Keep Original Spacing (Put back together)
            // -------------------------
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text))
            {
                try
                {
                    string character = @"\" + mainwindow.tbxSpacing.Text;

                    // Add period where missing between items
                    // Example . S01E02 Episode . Name
                    // Example . S01E02 . Episode . Name
                    // Example.S01E02.Episode.Name (Finalized)
                    //filename = Regex.Replace(filename, @"(?<!\.) (?!\.)", " " + character + " ");
                    filename = Regex.Replace(filename, @"(?<!" + character + ") (?!" + character + ")", " " + mainwindow.tbxSpacing.Text + " ");

                    filename = Regex.Replace(filename, @"( " + character + " )", mainwindow.tbxSpacing.Text);
                }
                catch
                {
                    MessageBox.Show("Could not finalize.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // Remove Spaces
            if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"( )", "");
            }

            filename = filename.Trim();

            return filename;
        }

     }
}
