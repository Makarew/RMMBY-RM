// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RMMBY_Installer_RM
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Get The Supported Games List
            games = GetGameList.GitGameList();

            foreach(GetGameList.Game game in games)
            {
                string result = "LOCATIONNOTFOUND";

                // Check If The User Has RMMBY Installed For The Current Game
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(string.Format("SOFTWARE\\Classes\\{0}\\shell\\open\\command", game.gameSchema)))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("");

                        if (o != null)
                        {
                            result = o as string;
                            result = result.Replace("\"", "");
                        }
                    }
                }

                // If RMMBY Is Installed, Add The Game To The Navigation Bar
                // If "Test Game" Ever Appears, Something Went Wrong
                if (result != "LOCATIONNOTFOUND")
                {
                    NavigationViewItem nvi = new NavigationViewItem();
                    nvi.Tag = game.gameSchema;
                    nvi.Content = game.gameName;

                    GameList.MenuItems.Add(nvi);
                }
            }

            // Set Navigation Bar Layout
            GameList.IsSettingsVisible = false;
            GameList.ExpandedModeThresholdWidth = 0;
            GameList.IsPaneToggleButtonVisible = false;
            GameList.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            GameList.OpenPaneLength = 200;

            GameList.SelectedItem = GameList.MenuItems[0];

            NavigationViewItem nvi2 = GameList.MenuItems[0] as NavigationViewItem;
            string defaultSchema = nvi2.Tag.ToString().Split('-')[0];

            // Set The Current Game In GameData
            foreach (GetGameList.Game game in games)
            {
                if (game.gameSchema == defaultSchema)
                {
                    GameData.currentGame = game;
                }
            }

            LoadPage();

            // Set Window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 700, Height = 800 });
            appWindow.SetIcon("Assets/RMMBY-Bee.ico");

            OverlappedPresenter pres = appWindow.Presenter as OverlappedPresenter;
            pres.IsResizable = false;

            Title = "RMMBY";
        }

        // An Array Of All Supported Games
        private GetGameList.Game[] games;

        private void nav_iteminvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // Set The Current Game In GameData
            foreach(GetGameList.Game game in games)
            {
                if (game.gameSchema == args.InvokedItemContainer.Tag.ToString())
                {
                    GameData.currentGame = game;
                }
            }

            // Load A New Page
            Type newPage = Type.GetType("WinUI3.Views.BlankPage1");
            LoadPage();
        }

        private void LoadPage()
        {
            contentFrame.Navigate(typeof(BlankPage1));
        }
    }
}
