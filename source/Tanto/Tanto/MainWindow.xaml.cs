/* ----------------------------------------------------------------------
Tanto
Copyright (C) 2018-2020 Matt McManis
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

using Tanto.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace Tanto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Tanto Current Version
        public static Version currentVersion;
        // Tanto GitHub Latest Version
        public static Version latestVersion;
        // Alpha, Beta, Stable
        public static string currentBuildPhase = "alpha";
        public static string latestBuildPhase;
        public static string[] splitVersionBuildPhase;

        public string TitleVersion
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Thread
        public static Thread th = null;
        //public static Task task = null;

        // System
        public static string appDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + @"\"; // Tanto.exe directory

        // Preview Window
        public static Preview previewWindow;
        // Update Window
        public static UpdateWindow updateWindow;

        // File List
        public static List<string> listFilePaths = new List<string>();
        public static List<string> listNewFileNames = new List<string>();
        public static List<string> listOriginalFileNamesBackup = new List<string>();

        // Series Titles
        public static List<string> listSeriesTitles = new List<string>();


        /// <summary>
        ///     Main Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // -----------------------------------------------------------------
            /// <summary>
            ///     Window & Components
            /// </summary>
            // -----------------------------------------------------------------
            this.MinWidth = 800;
            this.MinHeight = 450;

            ((INotifyCollectionChanged)lsvFileNames.Items).CollectionChanged += lsvFileNames_CollectionChanged;

            // -----------------------------------------------------------------
            /// <summary>
            ///     Load Saved Settings
            /// </summary>
            // -----------------------------------------------------------------  
            // -------------------------
            // Prevent Loading Corrupt App.Config
            // -------------------------
            try
            {
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationErrorsException ex)
            {
                string filename = ex.Filename;

                if (File.Exists(filename) == true)
                {
                    File.Delete(filename);
                    Settings.Default.Upgrade();
                }
                else
                {

                }
            }

            // -------------------------
            // Set Current Version to Assembly Version
            // -------------------------
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string assemblyVersion = fvi.FileVersion;
            currentVersion = new Version(assemblyVersion);

            // -------------------------
            // Title + Version
            // -------------------------
            TitleVersion = "Tantō (" + Convert.ToString(currentVersion) + "-" + currentBuildPhase + ")";
            DataContext = this;

            // -------------------------
            // Window Position
            // -------------------------
            if (Convert.ToDouble(Settings.Default["Left"]) == 0
                && Convert.ToDouble(Settings.Default["Top"]) == 0)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            // Load Saved
            else
            {
                this.Top = Settings.Default.Top;
                this.Left = Settings.Default.Left;
                this.Height = Settings.Default.Height;
                this.Width = Settings.Default.Width;

                if (Settings.Default.Maximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }

            // -------------------------
            // Load Checkboxes
            // -------------------------
            // Auto Sort
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxAutoSort.IsChecked = Convert.ToBoolean(Settings.Default.AutoSort);
            }
            catch
            {

            }

            // Episode Numbering
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxEpisodeNumbering.IsChecked = Convert.ToBoolean(Settings.Default.EpisodeNumbering);
            }
            catch
            {

            }


            // Episode Number Letter
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                if (string.IsNullOrEmpty(Settings.Default.EpisodeNumberLetter))
                {
                    tbxEpisodeNumberLetter.Text = "E";
                }

                // --------------------------
                // Load Saved
                // --------------------------
                else if (!string.IsNullOrEmpty(Settings.Default.EpisodeNumberLetter))
                {
                    tbxEpisodeNumberLetter.Text = Settings.Default.EpisodeNumberLetter;
                }
            }
            catch
            {

            }


            // Filter Keep Auto Year
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterAutoYear.IsChecked = Convert.ToBoolean(Settings.Default.FilterAutoYear);
            }
            catch
            {

            }

            // Filter Keep Series Title
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxAutoSeriesTitle.IsChecked = Convert.ToBoolean(Settings.Default.FilterSeriesTitle);
            }
            catch
            {

            }

            // Filter Auto Season Number
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxAutoSeasonNumber.IsChecked = Convert.ToBoolean(Settings.Default.FilterAutoSeasonNumber);
            }
            catch
            {

            }

            // Filter Auto Starting Episode Number
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxAutoStartingEpisodeNumber.IsChecked = Convert.ToBoolean(Settings.Default.FilterAutoStartingEpisodeNumber);
            }
            catch
            {

            }

            // Multi-Episode Numbers
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxMultiEpisodeNumbers.IsChecked = Convert.ToBoolean(Settings.Default.FilterMultiEpisodeNumbers);
            }
            catch
            {

            }

            // Filter Keep Episode Names
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxEpisodeNames.IsChecked = Convert.ToBoolean(Settings.Default.EpisodeNames);
            }
            catch
            {

            }

            // Filter Enable Title Case
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterTitleCase.IsChecked = Convert.ToBoolean(Settings.Default.FilterTitleCase);
            }
            catch
            {

            }

            // Filter Remove Episode Numbering
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterRemoveEpisodeNumbering.IsChecked = Convert.ToBoolean(Settings.Default.FilterRemoveEpisodeNumbering);
            }
            catch
            {

            }

            // Filter Keep Hyphens
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterHyphens.IsChecked = Convert.ToBoolean(Settings.Default.FilterHyphens);
            }
            catch
            {

            }

            // Episode Name Spacing
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterEpisodeNameSpacing.IsChecked = Convert.ToBoolean(Settings.Default.FilterEpisodeNameSpacing);
            }
            catch
            {

            }

            // Filter OriginalSpacing
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterOriginalSpacing.IsChecked = Convert.ToBoolean(Settings.Default.FilterOriginalSpacing);
            }
            catch
            {

            }

            // Filter Remove Episode Name Tags
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterRemoveTags.IsChecked = Convert.ToBoolean(Settings.Default.FilterRemoveTags);
            }
            catch
            {

            }


            // Filter Remove Year
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterRemoveYear.IsChecked = Convert.ToBoolean(Settings.Default.FilterRemoveYear);
            }
            catch
            {

            }

            // Filter Remove Period Spacing
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterPeriodSpacing.IsChecked = Convert.ToBoolean(Settings.Default.FilterPeriodSpacing);
            }
            catch
            {

            }

            // Filter Remove Underscore Spacing
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterUnderscoreSpacing.IsChecked = Convert.ToBoolean(Settings.Default.FilterUnderscoreSpacing);
            }
            catch
            {

            }

            // Filter Remove Dash Spacing
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterDashSpacing.IsChecked = Convert.ToBoolean(Settings.Default.FilterDashSpacing);
            }
            catch
            {

            }

            // Filter Remove Dashes
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterDashes.IsChecked = Convert.ToBoolean(Settings.Default.FilterDashes);
            }
            catch
            {

            }

            // Filter Remove Double Spaces
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                cbxFilterDoubleSpaces.IsChecked = Convert.ToBoolean(Settings.Default.FilterDoubleSpaces);
            }
            catch
            {

            }

            // -------------------------
            // Load Textboxes
            // -------------------------

            // Separator
            //
            // Safeguard Against Corrupt Saved Settings
            try
            {
                // --------------------------
                // First time use
                // --------------------------
                if (string.IsNullOrEmpty(Settings.Default.Separator))
                {
                    tbxSeparator.Text = " - ";
                }

                // --------------------------
                // Load Saved
                // --------------------------
                else if (!string.IsNullOrEmpty(Settings.Default.Separator))
                {
                    if (Settings.Default.Separator == "u0020")
                    {
                        tbxSeparator.Text = Settings.Default.Separator.Replace("u0020", " "); ;
                        //MessageBox.Show(tbxSeparator.Text); //debug
                    }
                    else
                    {
                        tbxSeparator.Text = Settings.Default.Separator;
                        //MessageBox.Show(Settings.Default.Separator);
                    }
                }
            }
            catch
            {

            }


            // -------------------------
            // Control Defaults
            // -------------------------

            // Year
            string year = "Year";

            try
            {
                year = year + " (" + DateTime.Now.Year.ToString() + ")";
            }
            catch
            {

            }

            cbxFilterRemoveYear.Content = year;
        }

        /// <summary>
        ///    Window Loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        ///     Closing
        /// </summary>
        void Window_Closing(object sender, CancelEventArgs e)
        {
            // Save Window Position

            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Settings.Default.Top = RestoreBounds.Top;
                Settings.Default.Left = RestoreBounds.Left;
                Settings.Default.Height = RestoreBounds.Height;
                Settings.Default.Width = RestoreBounds.Width;
                Settings.Default.Maximized = true;
            }
            else
            {
                Settings.Default.Top = this.Top;
                Settings.Default.Left = this.Left;
                Settings.Default.Height = this.Height;
                Settings.Default.Width = this.Width;
                Settings.Default.Maximized = false;
            }

            Settings.Default.EpisodeNumberLetter = tbxEpisodeNumberLetter.Text;

            if (!string.IsNullOrEmpty(tbxSeparator.Text) &&
                tbxSeparator.Text.Length > 0 &&
                tbxSeparator.Text.Trim().Length == 0) // single space
            {
                Settings.Default.Separator = "u0020";
                //MessageBox.Show("whitespace"); //debug
            }
            else
            {
                Settings.Default.Separator = tbxSeparator.Text;
            }
            
            Settings.Default.Save();

            // Exit
            e.Cancel = true;
            //System.Windows.Forms.Application.ExitThread();
            Environment.Exit(0);
        }

        /// <summary>
        ///     Close / Exit (Method)
        /// </summary>
        //protected override void OnClosed(EventArgs e)
        //{
        //    // Force Exit All Executables
        //    //base.OnClosed(e);
        //    //Application.Current.Shutdown();
        //}

        // --------------------------------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     CONTROLS
        /// </summary>
        // --------------------------------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------------------------------

        /// <summary>
        ///    List View File Names Items Added
        /// </summary>
        private void lsvFileNames_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Autofit column to new item name
                GridView gv = lsvFileNames.View as GridView;
                gv.Columns[1].Width = gv.Columns[1].ActualWidth;
                gv.Columns[1].Width = double.NaN;
            }
        }

        /// <summary>
        ///    List View File Names - Drag and Drop
        /// </summary>
        private void lsvFileNames_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = DragDropEffects.Copy;
        }

        private void lsvFileNames_PreviewDrop(object sender, DragEventArgs e)
        {
            string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

            // convert array to list
            List<string> files = new List<string>(dropFiles);

            // Remove Folders
            for (int i = files.Count - 1; i >= 0; --i)
            {
                if (files.Count > i && files.Count != 0)
                {
                    string fileExt = Path.GetExtension(files[i]);

                    // Check for File Extension
                    // Ignore if missing (Ignores folders)
                    if (string.IsNullOrEmpty(fileExt))
                    {
                        files.RemoveAt(i);
                    }
                }
            }

            AddFiles(files);
        }



        /// <summary>
        ///    Info Button
        /// </summary>
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
@"Copyright © 2018, 2019 Matt McManis

Logo by Kelig Le Luron from Noun Project.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see http://www.gnu.org/licenses/.
"
);
        }

        /// <summary>
        ///    Website Button
        /// </summary>
        private void btnWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/MattMcManis/Tanto");
        }

        /// <summary>
        ///    Update Button
        /// </summary>
        private Boolean IsUpdateWindowOpened = false;
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Proceed if Internet Connection
            //
            if (UpdateWindow.CheckForInternetConnection() == true)
            {
                // Parse GitHub .version file
                //
                string parseLatestVersion = string.Empty;

                try
                {
                    parseLatestVersion = UpdateWindow.wc.DownloadString("https://raw.githubusercontent.com/MattMcManis/Tanto/master/.version");
                }
                catch
                {
                    MessageBox.Show("GitHub version file not found.");

                    return;
                }


                //Split Version & Build Phase by dash
                //
                if (!string.IsNullOrEmpty(parseLatestVersion)) //null check
                {
                    try
                    {
                        // Split Version and Build Phase
                        splitVersionBuildPhase = Convert.ToString(parseLatestVersion).Split('-');

                        // Set Version Number
                        latestVersion = new Version(splitVersionBuildPhase[0]); //number
                        latestBuildPhase = splitVersionBuildPhase[1]; //alpha
                    }
                    catch
                    {
                        MessageBox.Show("Error reading version.");

                        return;
                    }

                    // Debug
                    //MessageBox.Show(Convert.ToString(latestVersion));
                    //MessageBox.Show(latestBuildPhase);


                    // Check if Tanto is the Latest Version
                    // Update Available
                    if (latestVersion > currentVersion)
                    {
                        // Yes/No Dialog Confirmation
                        //
                        MessageBoxResult result = MessageBox.Show(
                                                            "v" + latestVersion + "-" + latestBuildPhase + "\n\nDownload Update?", 
                                                            "Update Available ", 
                                                            MessageBoxButton.YesNo
                                                            );
                        switch (result)
                        {
                            case MessageBoxResult.Yes:
                                // Check if Window is already open
                                if (IsUpdateWindowOpened) return;

                                // Start Window
                                updateWindow = new UpdateWindow();

                                // Keep in Front
                                updateWindow.Owner = Window.GetWindow(this);

                                // Only allow 1 Window instance
                                updateWindow.ContentRendered += delegate { IsUpdateWindowOpened = true; };
                                updateWindow.Closed += delegate { IsUpdateWindowOpened = false; };

                                // Position Relative to MainWindow
                                // Keep from going off screen
                                updateWindow.Left = Math.Max((this.Left + (this.Width - updateWindow.Width) / 2), this.Left);
                                updateWindow.Top = Math.Max((this.Top + (this.Height - updateWindow.Height) / 2), this.Top);

                                // Open Window
                                updateWindow.Show();
                                break;
                            case MessageBoxResult.No:
                                break;
                        }
                    }
                    // Update Not Available
                    else if (latestVersion <= currentVersion)
                    {
                        MessageBox.Show("This version is up to date.");
                    }
                    // Unknown
                    else // null
                    {
                        MessageBox.Show("Could not find download. Try updating manually.");
                    }
                }
                // Version is Null
                else
                {
                    MessageBox.Show("GitHub version file returned empty.");
                }
            }
            else
            {
                MessageBox.Show("Could not detect Internet Connection.");
            }
        }


        /// <summary>
        ///    Settings Button
        /// </summary>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            // Yes/No Dialog Confirmation
            //
            MessageBoxResult result = MessageBox.Show(
                                                "Reset Saved Settings?",
                                                "Settings",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Exclamation
                                                );
            switch (result)
            {
                case MessageBoxResult.Yes:

                    // Reset AppData Settings
                    Settings.Default.Reset();
                    Settings.Default.Reload();

                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();

                    break;

                case MessageBoxResult.No:

                    break;
            }
        }


        /// <summary>
        ///    Input Button
        /// </summary>
        private void btnInput_Click(object sender, RoutedEventArgs e)
        {
            // Progress Info
            lblProgressInfo.Content = "";

            // -------------------------
            // Open Select File Window
            // -------------------------
            Microsoft.Win32.OpenFileDialog selectFiles = new Microsoft.Win32.OpenFileDialog();

            // -------------------------
            // Defaults
            // -------------------------
            selectFiles.Multiselect = true;
            selectFiles.RestoreDirectory = true;

            // -------------------------
            // Process Dialog Box
            // -------------------------
            Nullable<bool> result = selectFiles.ShowDialog();
            if (result == true)
            {
                List<string> files = new List<string>();
                for (var i = 0; i < selectFiles.FileNames.Length; i++)
                {
                    string fileExt = Path.GetExtension(selectFiles.FileNames[i]);

                    // Check for File Extension
                    // Ignore if missing (Ignores folders)
                    if (!string.IsNullOrEmpty(fileExt))
                    {
                        files.Add(selectFiles.FileNames[i]);
                    }
                }

                AddFiles(files);
            }
        }


        /// <summary>
        ///    Add Files (Method)
        /// </summary>
        public void AddFiles(List<string> files)
        {
            // Clear ListView
            lsvFileNames.Items.Clear();

            // -------------------------
            // Add Path+Filename to List
            // -------------------------
            for (var i = 0; i < files.Count; i++)
            {
                // Used for file renaming/moving
                listFilePaths.Add(files[i]);

                // ListView Display File Names + Ext
                lsvFileNames.Items.Add(Path.GetFileName(files[i]));
            }

            // -------------------------
            // Extract Series Title
            // -------------------------
            if (cbxAutoSeriesTitle.IsChecked == true)
            {
                try
                {
                    Extract.ExtractSeriesTitle(this);
                }
                catch
                {

                }
            }

            // -------------------------
            // Extract Year
            // -------------------------
            if (cbxFilterAutoYear.IsChecked == true)
            {
                tbxYear.Text = Extract.ExtractYear(Path.GetFileNameWithoutExtension(listFilePaths[0]));
            }

            // -------------------------
            // Extract Season Number
            // -------------------------
            if (cbxAutoSeasonNumber.IsChecked == true)
            {
                Extract.ExtractSeasonNumber(this);
            }

            // -------------------------
            // Extract Episode Number
            // -------------------------
            if (cbxAutoStartingEpisodeNumber.IsChecked == true)
            {
                Extract.ExtractEpisodeNumber(this);
            }

            // -------------------------
            // Extract Spacing
            // -------------------------
            if (cbxFilterOriginalSpacing.IsChecked == true)
            {
                tbxSpacing.Text = Extract.ExtractSpacing(Path.GetFileNameWithoutExtension(listFilePaths[0]));
            }

            // -------------------------
            // Auto Sort
            // -------------------------
            if (cbxAutoSort.IsChecked == true)
            {
                Sort.Sorting(this);
            }

            // -------------------------
            // Clear and Re-Add List Filename to ListView
            // -------------------------
            if (lsvFileNames.Items.Count > 0)
            {
                lsvFileNames.Items.Clear();
            }

            foreach (var name in listFilePaths.Select(f => Path.GetFileName(f)))
            {
                lsvFileNames.Items.Add(name);
            }

            //MessageBox.Show(string.Join("\n", listFilePaths)); //debug
            //MessageBox.Show(string.Join("\n", listEpisodeNames)); //debug
        }


        /// <summary>
        ///    Open Directory Button
        /// </summary>
        private void btnOpenDir_Click(object sender, RoutedEventArgs e)
        {
            if (listFilePaths != null 
                && listFilePaths.Count > 0)
            {
                if (Directory.Exists(Path.GetDirectoryName(listFilePaths[1])))
                {
                    Process.Start("explorer.exe", Path.GetDirectoryName(listFilePaths[1]));
                }
            }
        }

        /// <summary>
        ///    Sort Button
        /// </summary>
        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            // -------------------------
            // Progress Info
            // -------------------------
            lblProgressInfo.Content = "";

            // -------------------------
            // Sort
            // -------------------------
            Sort.Sorting(this);

            // -------------------------
            // Clear and Re-Add List Filename to ListView
            // -------------------------
            if (lsvFileNames.Items.Count > 0)
            {
                lsvFileNames.Items.Clear();
            }

            foreach (var name in MainWindow.listFilePaths.Select(f => Path.GetFileName(f)))
            {
                lsvFileNames.Items.Add(name);
            }
        }
        /// <summary>
        ///    Sort Button Up
        /// </summary>
        private void btnSortUp_Click(object sender, RoutedEventArgs e)
        {
            // Progress Info
            lblProgressInfo.Content = "";

            if (lsvFileNames.SelectedItems.Count > 0)
            {
                var selectedIndex = this.lsvFileNames.SelectedIndex;

                if (selectedIndex > 0)
                {
                    // ListView Items
                    var itemlsvFileNames = lsvFileNames.Items[selectedIndex];
                    lsvFileNames.Items.RemoveAt(selectedIndex);
                    lsvFileNames.Items.Insert(selectedIndex - 1, itemlsvFileNames);
                    
                    // List File Paths
                    string itemFilePaths = listFilePaths[selectedIndex];
                    listFilePaths.RemoveAt(selectedIndex);
                    listFilePaths.Insert(selectedIndex - 1, itemFilePaths);

                    // Clear and Re-Add List File Name to ListView
                    // Preserves Column Numbering
                    lsvFileNames.Items.Clear();
                    foreach (var name in listFilePaths.Select(f => Path.GetFileName(f)))
                    {
                        lsvFileNames.Items.Add(name);
                    }

                    // Highlight Selected Index
                    lsvFileNames.SelectedIndex = selectedIndex - 1;
                }

                //MessageBox.Show(string.Join("\n", listFilePaths)); //debug
                //MessageBox.Show(string.Join("\n", listFileNames)); //debug
            }
        }
        /// <summary>
        ///    Sort Button Down
        /// </summary>
        private void btnSortDown_Click(object sender, RoutedEventArgs e)
        {
            // Progress Info
            lblProgressInfo.Content = "";

            if (lsvFileNames.SelectedItems.Count > 0)
            {
                var selectedIndex = this.lsvFileNames.SelectedIndex;

                if (selectedIndex + 1 < lsvFileNames.Items.Count)
                {
                    // ListView Items
                    var itemlsvFileNames = lsvFileNames.Items[selectedIndex];
                    lsvFileNames.Items.RemoveAt(selectedIndex);
                    lsvFileNames.Items.Insert(selectedIndex + 1, itemlsvFileNames);

                    // List File Paths
                    string itemFilePaths = listFilePaths[selectedIndex];
                    listFilePaths.RemoveAt(selectedIndex);
                    listFilePaths.Insert(selectedIndex + 1, itemFilePaths);

                    // Clear and Re-Add List File Name to ListView
                    // Preserves Column Numbering
                    lsvFileNames.Items.Clear();
                    foreach (var name in listFilePaths.Select(f => Path.GetFileName(f)))
                    {
                        lsvFileNames.Items.Add(name);
                    }

                    // Highlight Selected Index
                    lsvFileNames.SelectedIndex = selectedIndex + 1;

                    //MessageBox.Show(string.Join("\n", listFilePaths)); //debug
                    //MessageBox.Show(string.Join("\n", listFileNames)); //debug
                }
            }
        }

        /// <summary>
        ///    Remove Button Down
        /// </summary>
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Progress Info
            lblProgressInfo.Content = "";

            if (lsvFileNames.SelectedItems.Count > 0)
            {
                var selectedIndex = lsvFileNames.SelectedIndex;

                // ListView Items
                var itemlsvFileNames = lsvFileNames.Items[selectedIndex];
                lsvFileNames.Items.RemoveAt(selectedIndex);

                // List File Paths
                string itemFilePaths = listFilePaths[selectedIndex];
                listFilePaths.RemoveAt(selectedIndex);

                // Clear and Re-Add List File Name to ListView
                // Preserves Column Numbering
                lsvFileNames.Items.Clear();
                foreach (var name in listFilePaths.Select(f => Path.GetFileName(f)))
                {
                    lsvFileNames.Items.Add(name);
                }
            }
        }


        /// <summary>
        ///    Clear List Button
        /// </summary>
        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            // Progress Info
            lblProgressInfo.Content = "";

            // File Paths
            if (listFilePaths != null && listFilePaths.Count > 0)
            {
                listFilePaths.Clear();
                listFilePaths.TrimExcess();
            }

            // New File Names
            if (listNewFileNames != null && listNewFileNames.Count > 0)
            {
                listNewFileNames.Clear();
                listNewFileNames.TrimExcess();
            }

            // ListView Names
            if (lsvFileNames.Items != null && lsvFileNames.Items.Count > 0)
            {
                lsvFileNames.Items.Clear();
            }

            // Original Backup File Names
            if (listOriginalFileNamesBackup != null && listOriginalFileNamesBackup.Count > 0)
            {
                listOriginalFileNamesBackup.Clear();
                listOriginalFileNamesBackup.TrimExcess();
            }
            
        }

        public void  DenySpecialKeys(KeyEventArgs e)
        {
            // Disallow Special Characters
            if ((Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.D8) ||  // *
                (Keyboard.IsKeyDown(Key.RightShift) && e.Key == Key.D8) ||  // *

                (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.OemPeriod) ||  // >
                (Keyboard.IsKeyDown(Key.RightShift) && e.Key == Key.OemPeriod) ||  // >

                (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.OemComma) ||  // <
                (Keyboard.IsKeyDown(Key.RightShift) && e.Key == Key.OemComma) ||  // <

                (Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.OemQuotes) ||  // <
                (Keyboard.IsKeyDown(Key.RightShift) && e.Key == Key.OemQuotes) ||  // <

                e.Key == Key.Tab ||  // tab
                e.Key == Key.Oem2 ||  // forward slash
                e.Key == Key.OemBackslash ||  // backslash
                e.Key == Key.OemQuestion ||  // ?
                e.Key == Key.OemSemicolon ||  // ;
                e.Key == Key.OemPipe // |
                )
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///     Title TextBox
        /// </summary>
        private void tbxTitle_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Year TextBox
        /// </summary>
        private void tbxYear_KeyDown(object sender, KeyEventArgs e)
        {
            // Only allow Numbers or Backspace
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }


        /// <summary>
        ///     Episode TextBox
        /// </summary>
        private void tbxEpisode_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Season Number TextBox
        /// </summary>
        private void tbxSeason_KeyDown(object sender, KeyEventArgs e)
        {
            // Only allow Numbers or Backspace
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///     Episode Number Letter (E)
        /// </summary>
        private void tbxEpisodeNumberLetter_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxEpisodeNumberLetter_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Save Settings
            //Settings.Default.EpisodeNumberLetter = tbxEpisodeNumberLetter.Text;
            //Settings.Default.Save();
        }

        /// <summary>
        ///     Episode Number (01)
        /// </summary>
        private void tbxStartEpisodeAt_KeyDown(object sender, KeyEventArgs e)
        {
            // Only allow Numbers or Backspace
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        ///     Resolution TextBox
        /// </summary>
        private void tbxResolution_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Format TextBox
        /// </summary>
        private void tbxFormat_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Audio Channels TextBox
        /// </summary>
        private void tbxAudioChannels_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Language TextBox
        /// </summary>
        private void tbxLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Subtitles TextBox
        /// </summary>
        private void tbxSubtitles_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Encoding TextBox
        /// </summary>
        private void tbxEncoding_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Misc TextBox
        /// </summary>
        private void tbxMisc_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Separator TextBox
        /// </summary>
        private void tbxSeparator_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxSeparator_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Save Settings
            //Settings.Default.Separator = tbxSeparator.Text;
            //Settings.Default.Save();
        }

        /// <summary>
        ///     Spacing TextBox
        /// </summary>
        private void tbxSpacing_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxSpacing_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        ///     Preserve TextBox
        /// </summary>
        private void tbxPreserveChars_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxPreserveWords_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        private void tbxPreserveBetweenStart_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        private void tbxPreserveBetweenEnd_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Remove TextBox
        /// </summary>
        private void tbxRemoveChars_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxRemoveWords_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        private void tbxRemoveRangeStart_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        private void tbxRemoveRangeEnd_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }

        /// <summary>
        ///     Replace TextBox
        /// </summary>
        private void tbxReplaceChars_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxReplaceCharsWith_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxReplaceWords_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxReplaceWordsWith_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }
        private void tbxReplaceSpacesWith_KeyDown(object sender, KeyEventArgs e)
        {
            DenySpecialKeys(e);
        }


        /// <summary>
        ///    Auto Sort Checkbox - Checked
        /// </summary>
        private void cbxAutoSort_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxAutoSort.IsChecked == true)
                {
                    Settings.Default.AutoSort = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoSort.IsChecked == false)
                {
                    Settings.Default.AutoSort = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///    Auto Sort Checkbox - Unchecked
        /// </summary>
        private void cbxAutoSort_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxAutoSort.IsChecked == true)
                {
                    Settings.Default.AutoSort = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoSort.IsChecked == false)
                {
                    Settings.Default.AutoSort = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///    Episode Numbering Checkbox - Checked
        /// </summary>
        private void cbxEpisodeNumbering_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxEpisodeNumbering.IsChecked == true)
                {
                    Settings.Default.EpisodeNumbering = true;
                    Settings.Default.Save();
                }
                else if (cbxEpisodeNumbering.IsChecked == false)
                {
                    Settings.Default.EpisodeNumbering = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Episode Numbering Checkbox - Unchecked
        /// </summary>
        private void cbxEpisodeNumbering_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxEpisodeNumbering.IsChecked == true)
                {
                    Settings.Default.EpisodeNumbering = true;
                    Settings.Default.Save();
                }
                else if (cbxEpisodeNumbering.IsChecked == false)
                {
                    Settings.Default.EpisodeNumbering = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Filter Auto Year Checkbox - Checked
        /// </summary>
        private void cbxFilterAutoYear_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxFilterAutoYear.IsChecked == true)
                {
                    Settings.Default.FilterAutoYear = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterAutoYear.IsChecked == false)
                {
                    Settings.Default.FilterAutoYear = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Filter Auto Year Checkbox - Unchecked
        /// </summary>
        private void cbxFilterAutoYear_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxFilterAutoYear.IsChecked == true)
                {
                    Settings.Default.FilterAutoYear = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterAutoYear.IsChecked == false)
                {
                    Settings.Default.FilterAutoYear = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Filter Remove Year Checkbox - Checked
        /// </summary>
        private void cbxFilterRemoveYear_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterRemoveYear.IsChecked == true)
                {
                    Settings.Default.FilterRemoveYear = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterRemoveYear.IsChecked == false)
                {
                    Settings.Default.FilterRemoveYear = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Filter Remove Year Checkbox - Unchecked
        /// </summary>
        private void cbxFilterRemoveYear_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterRemoveYear.IsChecked == true)
                {
                    Settings.Default.FilterRemoveYear = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterRemoveYear.IsChecked == false)
                {
                    Settings.Default.FilterRemoveYear = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Auto Series Title Checkbox - Checked
        /// </summary>
        private void cbxSeriesTitle_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxAutoSeriesTitle.IsChecked == true)
                {
                    Settings.Default.FilterSeriesTitle = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoSeriesTitle.IsChecked == false)
                {
                    Settings.Default.FilterSeriesTitle = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Auto Series Title Checkbox - Unchecked
        /// </summary>
        private void cbxSeriesTitle_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxAutoSeriesTitle.IsChecked == true)
                {
                    Settings.Default.FilterSeriesTitle = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoSeriesTitle.IsChecked == false)
                {
                    Settings.Default.FilterSeriesTitle = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Auto Season Number Checkbox - Checked
        /// </summary>
        private void cbxSeasonNumber_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxAutoSeasonNumber.IsChecked == true)
                {
                    Settings.Default.FilterAutoSeasonNumber = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoSeasonNumber.IsChecked == false)
                {
                    Settings.Default.FilterAutoSeasonNumber = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Auto Season Number Checkbox - Unchecked
        /// </summary>
        private void cbxSeasonNumber_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxAutoSeasonNumber.IsChecked == true)
                {
                    Settings.Default.FilterAutoSeasonNumber = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoSeasonNumber.IsChecked == false)
                {
                    Settings.Default.FilterAutoSeasonNumber = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Auto Starting Episode Number Checkbox - Checked
        /// </summary>
        private void cbxAutoStartingEpisodeNumber_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxAutoStartingEpisodeNumber.IsChecked == true)
                {
                    Settings.Default.FilterAutoStartingEpisodeNumber = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoStartingEpisodeNumber.IsChecked == false)
                {
                    Settings.Default.FilterAutoStartingEpisodeNumber = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Auto Starting Episode Number Checkbox - Unchecked
        /// </summary>
        private void cbxAutoStartingEpisodeNumber_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxAutoStartingEpisodeNumber.IsChecked == true)
                {
                    Settings.Default.FilterAutoStartingEpisodeNumber = true;
                    Settings.Default.Save();
                }
                else if (cbxAutoStartingEpisodeNumber.IsChecked == false)
                {
                    Settings.Default.FilterAutoStartingEpisodeNumber = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Multi-Episode Numbers Checkbox - Checked
        /// </summary>
        private void cbxMultiEpisodeNumbers_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxMultiEpisodeNumbers.IsChecked == true)
                {
                    Settings.Default.FilterMultiEpisodeNumbers = true;
                    Settings.Default.Save();
                }
                else if (cbxMultiEpisodeNumbers.IsChecked == false)
                {
                    Settings.Default.FilterMultiEpisodeNumbers = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Multi-Episode Numbers Checkbox - Unchecked
        /// </summary>
        private void cbxMultiEpisodeNumbers_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxMultiEpisodeNumbers.IsChecked == true)
                {
                    Settings.Default.FilterMultiEpisodeNumbers = true;
                    Settings.Default.Save();
                }
                else if (cbxMultiEpisodeNumbers.IsChecked == false)
                {
                    Settings.Default.FilterMultiEpisodeNumbers = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Episode Names Checkbox - Checked
        /// </summary>
        private void cbxEpisodeNames_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxEpisodeNames.IsChecked == true)
                {
                    Settings.Default.EpisodeNames = true;
                    Settings.Default.Save();
                }
                else if (cbxEpisodeNames.IsChecked == false)
                {
                    Settings.Default.EpisodeNames = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///    Episode Names Checkbox - Unchecked
        /// </summary>
        private void cbxEpisodeNames_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxEpisodeNames.IsChecked == true)
                {
                    Settings.Default.EpisodeNames = true;
                    Settings.Default.Save();
                }
                else if (cbxEpisodeNames.IsChecked == false)
                {
                    Settings.Default.EpisodeNames = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Filter Remove - Episode Numbering 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterRemoveEpisodeNumbering_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterRemoveEpisodeNumbering.IsChecked == true)
                {
                    Settings.Default.FilterRemoveEpisodeNumbering = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterRemoveEpisodeNumbering.IsChecked == false)
                {
                    Settings.Default.FilterRemoveEpisodeNumbering = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Filter Remove - Episode Numbering 
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterRemoveEpisodeNumbering_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterRemoveEpisodeNumbering.IsChecked == true)
                {
                    Settings.Default.FilterRemoveEpisodeNumbering = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterRemoveEpisodeNumbering.IsChecked == false)
                {
                    Settings.Default.FilterRemoveEpisodeNumbering = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Filter Enable - Title Case 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterTitleCase_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterTitleCase.IsChecked == true)
                {
                    Settings.Default.FilterTitleCase = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterTitleCase.IsChecked == false)
                {
                    Settings.Default.FilterTitleCase = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Filter Enable - Title Case
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterTitleCase_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterTitleCase.IsChecked == true)
                {
                    Settings.Default.FilterTitleCase = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterTitleCase.IsChecked == false)
                {
                    Settings.Default.FilterTitleCase = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///     Filter Enable - Original Spacing
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterOriginalSpacing_Checked(object sender, RoutedEventArgs e)
        {
            // Turn off Other Spacing Removal
            cbxFilterPeriodSpacing.IsChecked = false;
            cbxFilterUnderscoreSpacing.IsChecked = false;
            cbxFilterDashSpacing.IsChecked = false;

            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterOriginalSpacing.IsChecked == true)
                {
                    Settings.Default.FilterOriginalSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterOriginalSpacing.IsChecked == false)
                {
                    Settings.Default.FilterOriginalSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Filter Enable - Title Case
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterOriginalSpacing_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterOriginalSpacing.IsChecked == true)
                {
                    Settings.Default.FilterOriginalSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterOriginalSpacing.IsChecked == false)
                {
                    Settings.Default.FilterOriginalSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///     Filter Keep - Hyphens 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterHyphens_Checked(object sender, RoutedEventArgs e)
        {
            // Dash Spacing must be unchecked if keeping hyphens
            // There-is-no-way-to-determine-what-is-dash-and-what-is-hyphen
            cbxFilterDashSpacing.IsChecked = false;

            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterHyphens.IsChecked == true)
                {
                    Settings.Default.FilterHyphens = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterHyphens.IsChecked == false)
                {
                    Settings.Default.FilterHyphens = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Filter Keep - Hyphens 
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterHyphens_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterHyphens.IsChecked == true)
                {
                    Settings.Default.FilterHyphens = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterHyphens.IsChecked == false)
                {
                    Settings.Default.FilterHyphens = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///     Filter Add - Episode Name Spacing
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterEpisodeNameSpacing_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxFilterEpisodeNameSpacing.IsChecked == true)
                {
                    Settings.Default.FilterEpisodeNameSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterEpisodeNameSpacing.IsChecked == false)
                {
                    Settings.Default.FilterEpisodeNameSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Filter Add - Episode Name Spacing
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterEpisodeNameSpacing_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxFilterEpisodeNameSpacing.IsChecked == true)
                {
                    Settings.Default.FilterEpisodeNameSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterEpisodeNameSpacing.IsChecked == false)
                {
                    Settings.Default.FilterEpisodeNameSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///     Filter Remove - Episode Name Tags
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterRemoveTags_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxFilterRemoveTags.IsChecked == true)
                {
                    Settings.Default.FilterRemoveTags = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterRemoveTags.IsChecked == false)
                {
                    Settings.Default.FilterRemoveTags = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Filter Remove - Episode Name Tags
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterRemoveTags_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings

                if (cbxFilterRemoveTags.IsChecked == true)
                {
                    Settings.Default.FilterRemoveTags = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterRemoveTags.IsChecked == false)
                {
                    Settings.Default.FilterRemoveTags = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///     Filter Remove - Period Spacing 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterPeriodSpacing_Checked(object sender, RoutedEventArgs e)
        {
            // Turn off Keep Original Spacing
            cbxFilterOriginalSpacing.IsChecked = false;

            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterPeriodSpacing.IsChecked == true)
                {
                    Settings.Default.FilterPeriodSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterPeriodSpacing.IsChecked == false)
                {
                    Settings.Default.FilterPeriodSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Filter Remove - Period Spacing 
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterPeriodSpacing_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterPeriodSpacing.IsChecked == true)
                {
                    Settings.Default.FilterPeriodSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterPeriodSpacing.IsChecked == false)
                {
                    Settings.Default.FilterPeriodSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Filter Remove - Underscore Spacing 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterUnderscoreSpacing_Checked(object sender, RoutedEventArgs e)
        {
            // Turn off Keep Original Spacing
            cbxFilterOriginalSpacing.IsChecked = false;

            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterUnderscoreSpacing.IsChecked == true)
                {
                    Settings.Default.FilterUnderscoreSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterUnderscoreSpacing.IsChecked == false)
                {
                    Settings.Default.FilterUnderscoreSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Filter Remove - Underscore Spacing
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterUnderscoreSpacing_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterUnderscoreSpacing.IsChecked == true)
                {
                    Settings.Default.FilterUnderscoreSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterUnderscoreSpacing.IsChecked == false)
                {
                    Settings.Default.FilterUnderscoreSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Filter Remove - Dash Spacing 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterDashSpacing_Checked(object sender, RoutedEventArgs e)
        {
            // Turn off Keep Original Spacing
            cbxFilterOriginalSpacing.IsChecked = false;

            // Keep Hyphens must be unchecked if using Dash Spacing
            // There-is-no-way-to-determine-what-is-dash-and-what-is-hyphen
            cbxFilterHyphens.IsChecked = false;

            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterDashSpacing.IsChecked == true)
                {
                    Settings.Default.FilterDashSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterDashSpacing.IsChecked == false)
                {
                    Settings.Default.FilterDashSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Filter Remove - Dash Spacing 
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterDashSpacing_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterDashSpacing.IsChecked == true)
                {
                    Settings.Default.FilterDashSpacing = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterDashSpacing.IsChecked == false)
                {
                    Settings.Default.FilterDashSpacing = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///     Filter Remove - Dashes 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterDashes_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterDashes.IsChecked == true)
                {
                    Settings.Default.FilterDashes = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterDashes.IsChecked == false)
                {
                    Settings.Default.FilterDashes = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Filter Remove - Dashes 
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterDashes_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterDashes.IsChecked == true)
                {
                    Settings.Default.FilterDashes = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterDashes.IsChecked == false)
                {
                    Settings.Default.FilterDashes = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        ///     Filter Remove - Double Spaces 
        ///     Checkbox - Checked
        /// </summary>
        private void cbxFilterDoubleSpaces_Checked(object sender, RoutedEventArgs e)
        {
            //Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterDoubleSpaces.IsChecked == true)
                {
                    Settings.Default.FilterDoubleSpaces = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterDoubleSpaces.IsChecked == false)
                {
                    Settings.Default.FilterDoubleSpaces = false;
                    Settings.Default.Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Filter Remove - Double Spaces 
        ///     Checkbox - Unchecked
        /// </summary>
        private void cbxFilterDoubleSpaces_Unchecked(object sender, RoutedEventArgs e)
        {
            // Prevent Saving Corrupt App.Config
            try
            {
                // Save Toggle Settings
                
                if (cbxFilterDoubleSpaces.IsChecked == true)
                {
                    Settings.Default.FilterDoubleSpaces = true;
                    Settings.Default.Save();
                }
                else if (cbxFilterDoubleSpaces.IsChecked == false)
                {
                    Settings.Default.FilterDoubleSpaces = false;
                    Settings.Default.Save();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///     Filters Default
        /// </summary>
        private void btnFiltersDefault_Click(object sender, RoutedEventArgs e)
        {
            // Detect
            cbxAutoSeriesTitle.IsChecked = true;
            cbxFilterAutoYear.IsChecked = true;
            cbxAutoSeasonNumber.IsChecked = true;
            cbxAutoStartingEpisodeNumber.IsChecked = false;
            cbxMultiEpisodeNumbers.IsChecked = true;

            // Keep
            cbxEpisodeNames.IsChecked = true;
            cbxFilterTitleCase.IsChecked = false;
            cbxFilterOriginalSpacing.IsChecked = false;
            cbxFilterHyphens.IsChecked = true;

            // Add
            cbxFilterEpisodeNameSpacing.IsChecked = false;

            // Remove
            cbxFilterRemoveEpisodeNumbering.IsChecked = true;
            cbxFilterRemoveTags.IsChecked = true;
            cbxFilterRemoveYear.IsChecked = true;
            cbxFilterPeriodSpacing.IsChecked = true;
            cbxFilterUnderscoreSpacing.IsChecked = true;
            cbxFilterDashSpacing.IsChecked = false;
            cbxFilterDashes.IsChecked = true;
            cbxFilterDoubleSpaces.IsChecked = true;
        }


        /// <summary>
        ///    Preview Button
        /// </summary>
        private Boolean IsPreviewWindowOpened = false;
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            // Original File Names Backup
            // (Used for Undo Rename)
            listOriginalFileNamesBackup = listNewFileNames;

            // Progress Info
            lblProgressInfo.Content = "";

            if (ReadyCheck(this) == true)
            {
                // Call Rename Method
                Rename.Renamer(this);

                //MessageBox.Show(string.Join("\n", listFilePaths)); //debug
                //MessageBox.Show(string.Join("\n", listFileNames)); // debug
                //MessageBox.Show(string.Join("\n", listNewFileNames)); //debug

                // -------------------------
                // Open Preview Window
                // -------------------------

                // Check if Window is already open
                if (IsPreviewWindowOpened) return;

                // Open Preview Window
                previewWindow = new Preview(this);

                // Only allow 1 Window instance
                previewWindow.ContentRendered += delegate { IsPreviewWindowOpened = true; };
                previewWindow.Closed += delegate { IsPreviewWindowOpened = false; };

                // Keep Window on Top
                previewWindow.Owner = Window.GetWindow(this);

                // Size to Content
                previewWindow.SizeToContent = SizeToContent.Width;

                // Open Window
                previewWindow.Show();
            }
        }


        // --------------------------------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------------------------------
        /// <summary>
        ///     MAIN METHODS
        /// </summary>
        // --------------------------------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------------------------------

        /// <summary>
        ///    Ready Check
        /// </summary>
        public static bool ReadyCheck(MainWindow mainwindow)
        {
            bool ready = true;

            // -------------------------
            // File Properties
            // -------------------------
            if (string.IsNullOrWhiteSpace(mainwindow.tbxTitle.Text)
                && string.IsNullOrWhiteSpace(mainwindow.tbxYear.Text)
                && string.IsNullOrWhiteSpace(mainwindow.tbxSeason.Text)
                && string.IsNullOrWhiteSpace(mainwindow.tbxResolution.Text)
                && string.IsNullOrWhiteSpace(mainwindow.tbxFormat.Text)
                && string.IsNullOrWhiteSpace(mainwindow.tbxEncoding.Text)
                && string.IsNullOrWhiteSpace(mainwindow.tbxMisc.Text)
                && string.IsNullOrWhiteSpace(mainwindow.tbxSeparator.Text)
                )
            {
                ready = false;

                // Error
                MessageBox.Show("Must enter at least one Filename Property.",
                          "Error",
                          MessageBoxButton.OK,
                          MessageBoxImage.Warning);
            }

            // -------------------------
            // List View is Empty
            // -------------------------
            if (mainwindow.lsvFileNames.Items.Count == 0 || mainwindow.lsvFileNames == null)
            {
                ready = false;

                // Error
                MessageBox.Show("File list cannot be empty.",
                          "Error",
                          MessageBoxButton.OK,
                          MessageBoxImage.Warning);
            }

            // -------------------------
            // Original Spacing was checked without entering Spacing Character
            // -------------------------
            //if (string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text) &&
            //    mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            //{
            //    ready = false;

            //    MessageBox.Show("Must use Original Spacing CheckBox with Spacing TextBox.",
            //              "Error",
            //              MessageBoxButton.OK,
            //              MessageBoxImage.Warning);
            //}


            return ready;
        }


    }
    // End Main Class


    /// <summary>
    ///    ListView Indexer Class
    /// </summary>
    // ListView Numbering
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            var item = (ListViewItem)value;
            var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
