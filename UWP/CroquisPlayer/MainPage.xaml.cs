using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private ObservableCollection<Button> dataCollection;

        public MainPage()
        {
            this.InitializeComponent();

            dataCollection = new ObservableCollection<Button>();

            {
                Button btn = new Button();
                btn.Margin = new Thickness(2);
                btn.Width = 100;
                btn.Height = 100;

                StackPanel stackPanel = new StackPanel();
                SymbolIcon symbolIcon = new SymbolIcon(Symbol.Add);
                TextBlock textBlock = new TextBlock();

                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                textBlock.Text = resourceLoader.GetString("AddText");
                textBlock.Margin = new Thickness(0, 10, 0, 0);

                stackPanel.Children.Add(symbolIcon);
                stackPanel.Children.Add(textBlock);

                btn.Content = stackPanel;

                dataCollection.Add(btn);
            }
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

            TimeSec.Text = seconds.ToString();
        }

        private void BreakSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double seconds = BreakSlider.Value;
            BreakSec.Text = seconds.ToString();
        }
    }
}
