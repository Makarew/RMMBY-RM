// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using RMMBYLib;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RMMBY_Installer_RM
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallWindow : Window
    {
        public InstallWindow(string ocURL)
        {
            this.InitializeComponent();

            url = ocURL;
            gbInfo = GetSubmissionInfo();
            installerName = GetInstallerName(gbInfo._aGame._sName);

            // Set Window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 700, Height = 800 });
            appWindow.SetIcon("Assets/RMMBY-Bee.ico");

            OverlappedPresenter pres = appWindow.Presenter as OverlappedPresenter;
            pres.IsResizable = false;

            Title = "RMMBY - OneClick Installer";

            bee.RenderTransformOrigin = new Point(0.5,0.5);

            string[] paths = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "OneClickInstallers"), "*.dll");

            var assembly = FindInstaller(paths);

            var info = Utils.PullAttribute<RMMBYOneClickAttribute>(assembly);

            MainText.Text = string.Format(MainText.Text, gbInfo._sName, gbInfo._aSubmitter._sName);

            LargeText.Text = "Loaded One Click Installer: " + info.Name + 
                string.Format("\nGame: {0}", gbInfo._aGame._sName) +
                string.Format("\nCategory: {0}", gbInfo._aCategory._sName) +
                string.Format("\nMod Name: {0}", gbInfo._sName) +
                string.Format("\nSubmitter: {0}", gbInfo._aSubmitter._sName);

            installer = (OneClick)assembly.CreateInstance(info.SystemType.FullName);
            installer.UpdateLogText += () => GetInstallerLog();
            installer.UpdateMainText += () => GetMainLog();
            installer.InstallerFinished += () => OnFinish();
        }

        Storyboard storyboard = new Storyboard();
        OneClick installer;
        string url;
        GBSubmissionInfo gbInfo;
        string installerName;

        private async void Speen(object sender, RoutedEventArgs e)
        {
            StartInstallVisuals();
            installer.StartInstall().Start();
        }

        private async void StartInstallVisuals()
        {
            DoubleAnimation rotate = new DoubleAnimation()
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(10)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(rotate, bee);
            Storyboard.SetTargetProperty(rotate, "(UIElement.RenderTransform).(RotateTransform.Angle)");

            storyboard.Children.Add(rotate);

            storyboard.Begin();

            InstallButton.Visibility = Visibility.Collapsed;
            TextBorder.Width = 608;
        }

        private void StopSpeen(object sender, RoutedEventArgs e)
        {
            OnFinish();
        }
        private void OnFinish()
        {
            storyboard.Stop();
        }

        private void GetInstallerLog()
        {
            LargeText.Text += "\n" + installer.nextLogText;
        }
        private void GetMainLog()
        {
            MainText.Text = installer.nextMainText;
        }

        private Assembly FindInstaller(string[] paths)
        {
            Assembly assembly = null;

            foreach (string path in paths)
            {
                Assembly assembly2 = Assembly.LoadFrom(path);
                var info = Utils.PullAttribute<RMMBYOneClickAttribute>(assembly2);

                if (info.Name == installerName)
                {
                    assembly = assembly2;
                }
            }

            return assembly;
        }

        private GBSubmissionInfo GetSubmissionInfo()
        {
            string[] paths = GameBanana.GetPaths(url);
            return GBSubmissionInfo.GetMetadata(paths[1], paths[2]);
        }

        private string GetInstallerName(string name)
        {
            var webRequest = (HttpWebRequest)HttpWebRequest.Create("https://raw.githubusercontent.com/Makarew/RMMBYInstallers/main/ocinstallers.txt");

            var response = webRequest.GetResponse();
            var content = response.GetResponseStream();

            string inName = "";

            using (var reader = new StreamReader(content))
            {
                string line;
                int id = 0;
                using (reader)
                {
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            string[] lineData = line.Split(';');
                            
                            if (lineData[0] == name)
                            {
                                inName = lineData[2];
                                break;
                            }

                            id++;
                        }
                    }
                    while (line != null);
                }
            }

            return inName;
        }
    }
}
