using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CroquisPlayer
{
    public sealed partial class ShowPage : Page
    {
        private DispatcherTimer m_ShowTimer;
        private DispatcherTimer m_BreakTimer;
        private DispatcherTimer m_CountDownTimer;
        private DispatcherTimer m_Timer;
        private bool bBeginFromPause;

        private int m_Index;
        private int m_CountDown;

        private async void ShowImage()
        {
            ShowRPanel.Children.Clear();

            StorageFile file = MainPage.Current.m_Files[m_Index];
            BitmapImage bitmap = new BitmapImage();
            Image image = new Image();

            var stream = await file.OpenReadAsync();
            await bitmap.SetSourceAsync(stream);

            image.Source = bitmap;
            ShowRPanel.Children.Add(image);
        }

        private void SetupTimer()
        {
            //! set show timer
            m_ShowTimer = new DispatcherTimer();
            m_ShowTimer.Tick += ShowTimeEnd;
            m_ShowTimer.Interval = TimeSpan.FromSeconds(MainPage.Current.m_ShowTime);


            //! set break timer
            m_BreakTimer = new DispatcherTimer();
            m_BreakTimer.Tick += BreakTimeEnd;
            m_BreakTimer.Interval = TimeSpan.FromSeconds(MainPage.Current.m_BreakTime);

            //! set count down timer
            m_CountDownTimer = new DispatcherTimer();
            m_CountDownTimer.Tick += CountDown;
            m_CountDownTimer.Interval = new TimeSpan(0, 0, 1);
            m_CountDown = (int)MainPage.Current.m_BreakTime;

            //! set instruction timer
            m_Timer = new DispatcherTimer();
            m_Timer.Tick += InstructionTimeEnd;
            m_Timer.Interval = TimeSpan.FromSeconds(3);

            //! set instruction and start timer
            {
                ShowRPanel.Children.Clear();

                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                stackPanel.VerticalAlignment = VerticalAlignment.Center;

                TextBlock escBlock = new TextBlock();
                escBlock.FontSize = 100;
                escBlock.Text = resourceLoader.GetString("ESCMessage");
                stackPanel.Children.Add(escBlock);

                TextBlock spaceBlock = new TextBlock();
                spaceBlock.FontSize = 100;
                spaceBlock.Text = resourceLoader.GetString("SpaceMessage");
                stackPanel.Children.Add(spaceBlock);

                ShowRPanel.Children.Add(stackPanel);

                m_Timer.Start();
            }
        }

        private void InstructionTimeEnd(object sender, object e)
        {
            m_Timer.Stop();

            ShowImage();
            m_ShowTimer.Start();
        }

        private void ShowTimeEnd(object sender, object e)
        {
            m_ShowTimer.Stop();

            if (m_Index + 1 < MainPage.Current.m_Files.Count)
            {
                //! set screen for break time
                ShowRPanel.Children.Clear();
                m_CDText.Text = m_CountDown.ToString();
                ShowRPanel.Children.Add(m_CDText);

                //! start break mode
                m_CountDownTimer.Start();
                m_BreakTimer.Start();
            }
            else
            {
                Window.Current.Close();
            }
        }

        private void CountDown(object sender, object e)
        {
            if (m_CountDown > 0)
            {
                --m_CountDown;
                m_CDText.Text = m_CountDown.ToString();
            }
            else
            {
                m_CountDownTimer.Stop();
                m_CountDown = (int)MainPage.Current.m_BreakTime;
            }
        }

        private void BreakTimeEnd(object sender, object e)
        {
            m_BreakTimer.Stop();

            if (bBeginFromPause == false)
                ++m_Index;
            else
                bBeginFromPause = false;
            ShowImage();
            m_ShowTimer.Start();
        }

        private void EnterPauseMode()
        {
            //! stop all timer
            m_ShowTimer.Stop();
            m_BreakTimer.Stop();
            m_CountDownTimer.Stop();
            m_Timer.Stop();

            //! reset break timer
            m_CountDown = (int)MainPage.Current.m_BreakTime;

            //! show pause icon
            ShowRPanel.Children.Clear();
            PauseIcon.Visibility = Visibility.Visible;
        }

        private void ExitPauseMode()
        {
            PauseIcon.Visibility = Visibility.Collapsed;

            //! starting from break mode
            ShowRPanel.Children.Clear();
            m_CDText.Text = m_CountDown.ToString();
            ShowRPanel.Children.Add(m_CDText);
            m_CountDownTimer.Start();
            m_BreakTimer.Start();

            bBeginFromPause = true;
        }
    }
}
