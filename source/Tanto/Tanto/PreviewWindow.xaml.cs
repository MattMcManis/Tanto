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

using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Linq;

namespace Tanto
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : Window
    {
        private MainWindow mainwindow;

        public Preview(MainWindow mainwindow)
        {
            InitializeComponent();

            this.mainwindow = mainwindow;

            this.MinWidth = 400;
            this.MinHeight = 300;

            var gridView = new GridView();
            lsvPreview.View = gridView;
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Original",
                DisplayMemberBinding = new Binding("Original")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "New",
                DisplayMemberBinding = new Binding("New")
            });

            //MessageBox.Show(string.Join("\n", MainWindow.listNewFileNames)); //debug

            // -------------------------
            // Create Preview List
            // -------------------------
            for (var i = 0; i < MainWindow.listFilePaths.Count; i++)
            {
                lsvPreview.Items.Add(new MyItem {
                                Original = Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[i]),
                                New = Path.GetFileNameWithoutExtension(MainWindow.listNewFileNames[i])
                });

            }
        }

        /// <summary>
        ///    Window Loaded
        /// </summary>
        private void Window_Loaded(object sender, EventArgs e)
        {
            //base.OnContentRendered(e);

            //this.UpdateLayout();

            try
            {
                // Detect which screen we're on
                var allScreens = System.Windows.Forms.Screen.AllScreens.ToList();
                var thisScreen = allScreens.SingleOrDefault(s => this.Left >= s.WorkingArea.Left && this.Left < s.WorkingArea.Right);
                if (thisScreen == null) thisScreen = allScreens.First();

                // Position Relative to MainWindow
                this.Left = Math.Max((mainwindow.Left + (mainwindow.Width - this.Width) / 2), thisScreen.WorkingArea.Left);
                this.Top = Math.Max((mainwindow.Top + (mainwindow.Height - this.Height) / 2), thisScreen.WorkingArea.Top);
            }
            catch
            {

            }

        }


        /// <summary>
        ///    Rename Button
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            // Progress Info
            mainwindow.lblProgressInfo.Content = "Working";

            // Start
            if (MainWindow.ReadyCheck(mainwindow) == true)
            {
                // -------------------------
                // Call Rename Method
                // -------------------------
                Rename.Renamer(mainwindow);

                //MessageBox.Show(string.Join("\n", listFilePaths)); //debug
                //MessageBox.Show(string.Join("\n", listFileNames)); //debug
                //MessageBox.Show(string.Join("\n", listNewFileNames)); //debug
                //MessageBox.Show(string.Join("\n", Preview.listPeviewNames)); //debug

                // Start File Rename/Move
                for (var i = 0; i < MainWindow.listFilePaths.Count; i++)
                {
                    // -------------------------
                    // File
                    // -------------------------
                    string filename = Path.GetFileNameWithoutExtension(MainWindow.listFilePaths[i]);

                    string original = MainWindow.listFilePaths[i]; // path + filename + ext
                    string newName = MainWindow.listNewFileNames[i]; // path + filename + ext

                    // -------------------------
                    // Rename
                    // -------------------------
                    try
                    {
                        // If original file exists
                        if (File.Exists(original))
                        {
                            // If new filename does not already exist
                            if (!File.Exists(newName))
                            {
                                // Rename File
                                File.Move(original, newName);
                            }

                            // New filename already exists
                            else
                            {
                                // Handle file duplicate rename
                                // Yes/No Dialog Confirmation
                                MessageBoxResult result = MessageBox.Show(
                                                          newName + " already exists.",
                                                          "Append Duplicate Number",
                                                          MessageBoxButton.YesNoCancel);

                                switch (result)
                                {
                                    case MessageBoxResult.Yes:
                                        // Rename File
                                        File.Move(original, Rename.DuplicateAppend(newName));
                                        break;
                                    case MessageBoxResult.No:
                                        break;
                                    case MessageBoxResult.Cancel:
                                        // Progress Info
                                        mainwindow.lblProgressInfo.Content = "Canceled";
                                        return;
                                }
                            }
                        }

                        // Original file does not exist
                        else
                        {
                            // Handle write permissions
                            MessageBox.Show(original + " does not exist.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }

                    // Could not rename file
                    catch (IOException)
                    {
                        // Handle write permissions
                        MessageBox.Show(filename + " Could not be renamed.\nSaving to" + newName + " may require Admin Privileges.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }

                // Progress Info
                mainwindow.lblProgressInfo.Content = "Complete";
            }

            // Close Window
            this.Close();
        }

    }



    /// <summary>
    ///    Item Class
    /// </summary>
    public class MyItem
    {
        public string Original { get; set; }

        public string New { get; set; }
    }
}
