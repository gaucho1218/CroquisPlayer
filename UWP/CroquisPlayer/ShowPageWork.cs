using System;
using Windows.Storage;
using Windows.UI.Core;
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

        private int m_Index;
        private int m_CountDown;
        private int m_LeftTime;

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
                if (m_State == ShowState.Pause)
                {
                    ExitPauseMode();
                    m_State = ShowState.PauseToShow;
                }
                else if (m_State != ShowState.Hello && m_State != ShowState.Bye)
                {
                    EnterPauseMode();
                    m_State = ShowState.Pause;
                }
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Right && m_State == ShowState.Show)
            {
                if (m_Index + 1 < MainPage.Current.m_Files.Count)
                    StartShowTime();
                else
                {
                    ShowLeftTimeText.Visibility = Visibility.Collapsed;
                    StartFinishTime();
                }
            }
        }

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
            m_Timer.Tick -= InstructionTimeEnd;

            StartShowTime();
        }

        private void StartFinishTime()
        {
            m_Timer.Tick += FinishTimeEnd;

            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

            ShowRPanel.Children.Clear();
            TextBlock text = new TextBlock();
            text.FontSize = 100;
            text.Text = resourceLoader.GetString("FinishMessage");
            ShowRPanel.Children.Add(text);
            m_State = ShowState.Bye;

            m_Timer.Start();
        }

        private void FinishTimeEnd(object sender, object e)
        {
            m_Timer.Stop();

            Window.Current.Close();
        }

        private void StartShowTime()
        {
            m_ShowTimer.Stop();

            if (m_State != ShowState.PauseToShow)
            {
                ++m_Index;
                m_CountDown = (int)MainPage.Current.m_ShowTime;
            }
            else
                m_CountDown = m_LeftTime;
            m_State = ShowState.Show;
            ShowImage();

            ShowLeftTimeText.Text = m_CountDown.ToString();
            ShowLeftTimeText.Visibility = Visibility.Visible;

            m_CountDownTimer.Start();
            m_ShowTimer.Start();
        }

        private void ShowTimeEnd(object sender, object e)
        {
            ShowLeftTimeText.Visibility = Visibility.Collapsed;
            m_ShowTimer.Stop();

            if (m_Index + 1 < MainPage.Current.m_Files.Count)
                StartBreakTime();
            else
                StartFinishTime();
        }

        private void CountDown(object sender, object e)
        {
            if (m_CountDown > 0)
            {
                --m_CountDown;
                if (m_State == ShowState.Show)
                    ShowLeftTimeText.Text = m_CountDown.ToString();
                else
                    m_CDText.Text = m_CountDown.ToString();
            }
            else
            {
                m_CountDownTimer.Stop();
            }
        }

        private void StartBreakTime()
        {
            m_State = ShowState.Break;

            //! set break time
            m_CountDown = (int)MainPage.Current.m_BreakTime;

            //! set screen for break time
            ShowRPanel.Children.Clear();
            m_CDText.Text = m_CountDown.ToString();
            ShowRPanel.Children.Add(m_CDText);

            //! start break mode
            m_CountDownTimer.Start();
            m_BreakTimer.Start();
        }

        private void BreakTimeEnd(object sender, object e)
        {
            m_BreakTimer.Stop();
            StartShowTime();
        }

        private void EnterPauseMode()
        {
            //! store left time
            if (m_State == ShowState.Show)
                Int32.TryParse(ShowLeftTimeText.Text, out m_LeftTime);
            else
                m_LeftTime = (int)MainPage.Current.m_ShowTime;

            //! stop all timer
            m_ShowTimer.Stop();
            m_BreakTimer.Stop();
            m_CountDownTimer.Stop();
            m_Timer.Stop();

            m_State = ShowState.Pause;
            ShowLeftTimeText.Visibility = Visibility.Collapsed;

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
            m_State = ShowState.PauseToShow;
            StartBreakTime();
        }
    }
}
