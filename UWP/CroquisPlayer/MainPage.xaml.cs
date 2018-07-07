using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace CroquisPlayer
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        private ObservableCollection<Button> dataCollection;
        private Image m_tempImage;
        public double m_ShowTime;
        public double m_BreakTime;

        public MainPage()
        {
            this.InitializeComponent();

            //! min size
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));

            //! application datas
            Current = this;

            dataCollection = new ObservableCollection<Button>();
            m_Files = new List<StorageFile>();

            setDataCollection();
        }

        private void TimeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double seconds = TimeSlider.Value;

            if (seconds >= 60)
            {
                //! handle minutes
                double min = Math.Truncate((seconds / 60));
                TimeMin.Text = min.ToString();
                seconds -= (min * 60);
            }
            else
                TimeMin.Text = "0";

            TimeSec.Text = seconds.ToString();
        }

        private void BreakSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double seconds = BreakSlider.Value;
            BreakSec.Text = seconds.ToString();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                foreach (Windows.Storage.StorageFile file in files)
                {
                    //! load file and add to list
                    makeImageButton(file);
                }
            }
        }

        private void ImgButton_Entered(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            m_tempImage = button.Content as Image;
            SymbolIcon icon = new SymbolIcon(Symbol.Delete);
            button.Content = icon;
        }

        private void ImgButton_Exited(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (m_tempImage != null)
            {
                button.Content = m_tempImage;
                m_tempImage = null;
            }
        }

        private void ImgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            foreach (Button target in dataCollection)
            {
                if (button == target)
                {
                    m_Files.RemoveAt(dataCollection.IndexOf(button) - 1);
                    dataCollection.Remove(target);
                    break;
                }
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataCollection.Count > 1)
            {
                m_ShowTime = TimeSlider.Value;
                m_BreakTime = BreakSlider.Value;

                CoreApplicationView newView = CoreApplication.CreateNewView();

                int newViewId = 0;

                await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

                    Frame frame = new Frame();
                    frame.Navigate(typeof(ShowPage), null);
                    Window.Current.Content = frame;
                    Window.Current.Activate();

                    newViewId = ApplicationView.GetForCurrentView().Id;
                });

                bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            }
            else
            {
                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                var dialog = new MessageDialog(resourceLoader.GetString("NoImage"));
                await dialog.ShowAsync();
            }
        }

        private void AdRect_AdRefreshed(object sender, RoutedEventArgs e)
        {
            AdRect.Visibility = Visibility.Visible;
        }
    }
}
