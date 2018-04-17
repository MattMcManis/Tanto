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
                    string character = @"\" + mainwindow.tbxSpacing.Text;
                    filename = Regex.Replace(filename, character, " " + mainwindow.tbxSpacing.Text + " ");
                }
            }

            // -------------------------
            // Remove Period Spacing
            // -------------------------
            if (mainwindow.cbxFilterPeriodSpacing.IsChecked == true)
            {
                // replace with space
                string regex = @"(?<!\.)\.(?!\.)";
                filename = Regex.Replace(filename, regex, " ");
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
                filename = Regex.Replace(filename, @"\(.*?(\d+).*\)", "");
            }

            // -------------------------
            // Remove Episode Numbering
            // -------------------------
            if (mainwindow.cbxFilterEpisodeNumbering.IsChecked == true)
            {
                filename = Regex.Replace(
                                filename
                                , @"\b(S\d\d\d?E\d\d\d?|EP\d?|E\d\d?|\d\d\d?)\b\s*"
                                , ""
                                , RegexOptions.IgnoreCase
                                );
            }

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
        ///    Filter Remove (Method)
        /// </summary>
        /// <remarks>
        ///    This filter is applied to Episode Name.
        /// </remarks>
        public static String FilterRemove(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Title Case
            // -------------------------
            if (mainwindow.cbxFilterTitleCase.IsChecked == true)
            {
                int minLength = 2;
                string regex = string.Format(@"(?<=(^|[.!?])\s*)\w|\b\w(?=[-\w]{{{0}}})", minLength);
                filename = Regex.Replace(filename, regex, m => m.Value.ToUpperInvariant());
            }

            // -------------------------
            // Remove Words
            // -------------------------
            if (mainwindow.tbxRemoveWords.Text != string.Empty)
            {
                string words = mainwindow.tbxRemoveWords.Text.Replace(",", "|");

                string regex = @"\b(" + words + @")\b";

                filename = Regex.Replace(filename, regex, "");
            }

            // -------------------------
            // Remove Characters
            // -------------------------
            if (mainwindow.tbxRemoveChars.Text != string.Empty)
            {
                string characters = mainwindow.tbxRemoveChars.Text.Replace(",", "|");

                string regex = characters;

                filename = Regex.Replace(filename, regex, "");
            }

            // -------------------------
            // Remove Between Characters
            // -------------------------
            if (mainwindow.tbxRemoveRangeStart.Text != string.Empty
                && mainwindow.tbxRemoveRangeEnd.Text != string.Empty)
            {
                string start = mainwindow.tbxRemoveRangeStart.Text;
                string end = mainwindow.tbxRemoveRangeEnd.Text;

                string regex = "(\\" + start + ".*\\" + end + ")";

                filename = Regex.Replace(filename, regex, "");
            }

            // -------------------------
            // Remove Tags
            // -------------------------
            if (mainwindow.cbxFilterRemoveTags.IsChecked == true)
            {
                List<string> filter = new List<string>()
                {
                    "(8k)", "(4k)", "(2k)",
                    "8k", "4k", "2k",
                    "(8K)", "(4K)", "(2K)",
                    "8K", "4K", "2K",
                    "(1080p)", "(720p)", "(480p)",
                    "1080p", "720p", "480p",
                    "(1080P)", "(720P)", "(480P)",
                    "1080P", "720P", "480P",

                    "(DVD)", "(BD)", "(Blu-Ray)", "(BluRay)",
                    "DVD", "Blu-Ray", "BLU-RAY", "BluRay",  "BLURAY",

                    "(x264)", "(x265)", "(hevc)",
                    "x264", "x265", "hevc",
                    "(X264)", "(X265)", "(HEVC)",
                    "X264", "X265", "HEVC",

                     "(H264)", "(H265)",
                     "H264", "H265",
                     "(h264)", "(h265)",
                     "h264", "h265",

                     "(2CH)", "(6CH)", "(DD5.1)",
                     "2CH", "6CH", "DD5.1",
                     "(2ch)", "(6ch)", "(dd5.1)",
                     "2ch", "6ch", "dd5.1"
                };

                for (var i = 0; i < filter.Count; i++)
                {
                    if (filename.Contains(filter[i]))
                    {
                        if (!filename.StartsWith(filter[i]))
                        {
                            int index = filename.IndexOf(filter[i]);

                            if (index >= 0)
                            {
                                filename = filename.Substring(0, index);
                            }
                        }
                    }
                }
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
                string words = mainwindow.tbxReplaceWords.Text.Replace(",", "|");
                string replacement = mainwindow.tbxReplaceWordsWith.Text;

                string regex = @"\b(" + words + @")\b";

                filename = Regex.Replace(filename, regex, replacement);
            }

            // Replace These Characters With
            if (mainwindow.tbxReplaceChars.Text != string.Empty)
            {
                string characters = mainwindow.tbxReplaceChars.Text.Replace(",", "|");
                string replacement = mainwindow.tbxReplaceCharsWith.Text;

                string regex = characters;

                filename = Regex.Replace(filename, regex, replacement);
            }

            // Replace Spaces With
            if (mainwindow.tbxReplaceSpacesWith.Text != string.Empty)
            {
                string replacement = mainwindow.tbxReplaceSpacesWith.Text;

                filename = Regex.Replace(filename, " ", replacement);
            }


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
                    string character = @"\" + mainwindow.tbxSpacing.Text; // to remove
                    filename = Regex.Replace(filename, character, " " + mainwindow.tbxSpacing.Text + " ");
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
            //if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            //{
            //MessageBox.Show(filename); //debug              

            if (!string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text))
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

            // Remove Spaces
            if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"( )", "");
            }

            return filename;
        }

     }
}
