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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RMMBY_Installer_RM
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallWindow : Window
    {
        public InstallWindow()
        {
            this.InitializeComponent();

            // Set Window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 700, Height = 800 });
            appWindow.SetIcon("Assets/RMMBY-Bee.ico");

            OverlappedPresenter pres = appWindow.Presenter as OverlappedPresenter;
            pres.IsResizable = false;

            Title = "RMMBY - OneClick Installer";

            //RotateTransform rt = new RotateTransform()
            //{
            //    CenterX = bee.Width / 2,
            //    CenterY = bee.Height / 2,
            //};
            //bee.RenderTransform = rt;

            bee.RenderTransformOrigin = new Point(0.5,0.5);
        }

        Storyboard storyboard = new Storyboard();

        private void Speen(object sender, RoutedEventArgs e)
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

            MainText.Text = "Downloading";
            LargeText.Text += "\nDownloading";

            InstallButton.Visibility = Visibility.Collapsed;
            TextBorder.Width = 608;
        }

        private void StopSpeen(object sender, RoutedEventArgs e)
        {
            storyboard.Stop();
            MainText.Text = "Installed";
            LargeText.Text += "\nExtracting\nInstalled";
        }
    }
}
