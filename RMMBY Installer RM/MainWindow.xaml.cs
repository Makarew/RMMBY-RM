// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
        public MainWindow(string gameName)
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

                    GameIconData gid = GameIconData.GetIconFromSchema(game.gameSchema);

                    BitmapImage bi = new BitmapImage();
                    Uri uri = new Uri(gid._aPreviewMedia._aImages[0]._sUrl);
                    bi.UriSource = uri;

                    nvi.Icon = new ImageIcon() { Source = bi };

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

            if (gameName != null)
            {
                StartGameConnection(gameName);
                Title = "RMMBY - " + gameName;
            } else
            {
                Title = "RMMBY";
            }
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

        // Networking
        private void StartGameConnection(string gameName)
        {
            string testString = "";

            foreach (GetGameList.Game game in games)
            {
                if (game.gameName == gameName)
                {
                    testString = game.gameSchema;
                    GameData.currentGame = game;
                }
            }

            GameData.exclusiveMode = true;
            GameData.exclusiveSchema = testString;

            GameList.OpenPaneLength = 0;

            LoadPage();

            try
            {
                GameData.clientSocket = new TcpClient("127.0.0.1", 1290);

                byte[] data = Encoding.ASCII.GetBytes("Schema - " + testString);

                GameData.stream = GameData.clientSocket.GetStream();
                GameData.stream.Write(data, 0, data.Length);

                data = new byte[1024];

                string responseData = string.Empty;

                Int32 bytes = GameData.stream.Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                GameData.stream.Close();

                GameData.clientSocket.Close();
            }
            catch (ArgumentNullException e)
            {

            }
            catch (SocketException e)
            {

            }
        }
    }
}
