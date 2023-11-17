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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Intrinsics.Arm;
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
        }

        private void nav_iteminvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            UpdateCategory(args.InvokedItemContainer.Tag.ToString());
        }

        private void UpdateCategory(string tag)
        {
            //TestText.Text = tag;

            for (int i = 0; i < GameData.currentGame.modTypes.Count; i++)
            {
                if (GameData.currentGame.gameSchema + "-" + GameData.currentGame.modTypes[i] == tag)
                {
                   // TestText.Text += "\n" + GameData.currentGame.typeDirectories[i];
                }
            }

            ModListView.ItemsSource = DisplayMod.GetGameMods("", "");
        }

        void Save(object sender, RoutedEventArgs e)
        {

        }

        void Reset(object sender, RoutedEventArgs e)
        {

        }
    }
}
