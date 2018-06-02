using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace CroquisPlayer
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class ShowPage : Page
    {
        private bool bPause;
        private TextBlock m_CDText;

        public ShowPage()
        {
            this.InitializeComponent();

            m_CDText = new TextBlock();

            bPause = false;
            m_Index = 0;
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

            SetupTimer();
        }

        private void ShowPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
        }
    }
}
