using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CroquisPlayer
{
    public sealed partial class ShowPage : Page
    {
        private DispatcherTimer m_ShowTimer;
        private DispatcherTimer m_BreakTimer;
        private DispatcherTimer m_CountDownTimer;

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
            m_BreakTimer.Interval = new TimeSpan(0, 0, 1);
            m_CountDown = (int)MainPage.Current.m_BreakTime;

            //! TODO change this to instruction mode

            //! start show timer
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

            ++m_Index;
            ShowImage();
            m_ShowTimer.Start();
        }

    }
}
