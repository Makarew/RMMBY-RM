// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Preview.GamesEnumeration;
using static RMMBY_Installer_RM.GetGameList;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RMMBY_Installer_RM
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();

            ModList.IsSettingsVisible = false;
            ModList.ExpandedModeThresholdWidth = 0;
            ModList.IsPaneToggleButtonVisible = false;
            ModList.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;

            GetGameList.Game game = GameData.currentGame;

            // For Testing
            // Set A Text Box To Show The Game's Schema
            //TestText.Text = game.gameSchema;

            // Add Each Supported Mod Category Under The Schema;
            for (int i = 0; i < game.modTypes.Count; i++)
            {
                //TestText.Text += "\n" + game.modTypes[i];

                NavigationViewItem nvi = new NavigationViewItem();
                nvi.Tag = game.gameSchema + "-" + game.modTypes[i];
                nvi.Content = game.modTypes[i];

                ModList.MenuItems.Add(nvi);
            }

            ModList.SelectedItem = ModList.MenuItems[0];
            NavigationViewItem nvi2 = ModList.MenuItems[0] as NavigationViewItem;
            UpdateCategory(nvi2.Tag.ToString());

            //BackgroundImage.ImageSource = "";
        }

        private void nav_iteminvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            UpdateCategory(args.InvokedItemContainer.Tag.ToString());
        }

        private void UpdateCategory(string tag)
        {
            //TestText.Text = tag;

            string schema = "";
            string category = "";

            for (int i = 0; i < GameData.currentGame.modTypes.Count; i++)
            {
                if (GameData.currentGame.gameSchema + "-" + GameData.currentGame.modTypes[i] == tag)
                {
                    // TestText.Text += "\n" + GameData.currentGame.typeDirectories[i];

                    schema = GameData.currentGame.gameSchema;
                    category = GameData.currentGame.modTypes[i];
                    GameData.currentCategory = category;
                }
            }

            ModListView.ItemsSource = DisplayMod.GetGameMods(schema, category);

            GetFromFile();
        }

        void Save(object sender, RoutedEventArgs e)
        {
            string filePath = Path.Combine(GameData.currentGame.defaultLocation, GameData.currentCategory + ".rmmby");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            IList<object> modsList = ModListView.SelectedItems;

            List<int> enabledModID = new List<int>();

            List<string> enabledMods = new List<string>();

            for (int i = 0; i < modsList.Count; i++)
            {
                for (int j = 0; j < GameData.modCount; j++)
                {
                    if (modsList[i] == ModListView.Items[j])
                    {
                        enabledModID.Add(j);
                    }
                }
            }

            List<Metadata> catMods = new List<Metadata>();
            for (int i = 0; i < GameData.currentGame.installedMods.Count; i++)
            {
                if (GameData.currentGame.installedMods[i].Type == GameData.currentCategory)
                {
                    catMods.Add(GameData.currentGame.installedMods[i]);
                }
            }

            foreach (int modID in enabledModID)
            {
                enabledMods.Add(catMods[modID].Location);
            }

            File.Create(filePath).Close();

            StreamWriter sw = new StreamWriter(filePath);

            for (int i = 0; i < enabledMods.Count; i++)
            {
                sw.WriteLine(enabledMods[i]);
            }
            sw.Close();

            if (!GameData.exclusiveMode) return;

            GameData.clientSocket = new TcpClient("127.0.0.1", 1290);
            GameData.stream = GameData.clientSocket.GetStream();

            byte[] data = Encoding.ASCII.GetBytes("End Connection No Restart");
            GameData.stream.Write(data, 0, data.Length);

            GameData.stream.Close();
            GameData.stream.Flush();

            GameData.clientSocket.Close();
            GameData.clientSocket.Dispose();

            Application.Current.Exit();
        }

        void Reset(object sender, RoutedEventArgs e)
        {
            GetFromFile();
        }

        private void GetFromFile()
        {
            ModListView.DeselectRange(new ItemIndexRange(0, Convert.ToUInt32(ModListView.Items.Count)));

            if (!File.Exists(Path.Combine(GameData.currentGame.defaultLocation, GameData.currentCategory + ".rmmby"))) return;

            List<Metadata> catMods = new List<Metadata>();
            for (int i = 0; i < GameData.currentGame.installedMods.Count; i++)
            {
                if (GameData.currentGame.installedMods[i].Type == GameData.currentCategory)
                {
                    catMods.Add(GameData.currentGame.installedMods[i]);
                }
            }

            List<string> enabledMods = new List<string>();

            string line;
            using (StreamReader r = new StreamReader(Path.Combine(GameData.currentGame.defaultLocation, GameData.currentCategory + ".rmmby")))
            {
                do
                {
                    line = r.ReadLine();
                    enabledMods.Add(line);
                } while (line != null);
            }

            for (int i = 0; i < catMods.Count; i++)
            {
                for (int j = 0; j < enabledMods.Count; j++)
                {
                    if (catMods[i].Location == enabledMods[j])
                        ModListView.SelectRange(new ItemIndexRange(i, 1));
                }
            }
        }
    }
}
