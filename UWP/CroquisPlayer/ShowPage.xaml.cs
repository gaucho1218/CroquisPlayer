using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace CroquisPlayer
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class ShowPage : Page
    {
        private bool bPause;

        public ShowPage()
        {
            this.InitializeComponent();

            bPause = false;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if (e.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                //! kill this page
                Window.Current.Close();
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Space)
            {
                //! pause
                bPause = bPause == true ? false : true;
            }
        }

        private void ShowPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            ShowImageAsync();
        }

        private void ShowPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
        }

        private async System.Threading.Tasks.Task ShowImageAsync()
        {
            StorageFile file = MainPage.Current.m_Files[0];
            BitmapImage bitmap = new BitmapImage();
            Image image = new Image();

            var stream = await file.OpenReadAsync();
            await bitmap.SetSourceAsync(stream);

            image.Source = bitmap;
            ShowRPanel.Children.Add(image);
        }
    }
}
