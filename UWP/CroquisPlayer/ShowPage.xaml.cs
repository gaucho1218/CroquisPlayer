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
        enum ShowState
        {
            None,
            Hello,
            Bye,
            Show,
            Break,
            Pause,
            PauseToShow
        };

        private ShowState m_State;
        private TextBlock m_CDText;

        public ShowPage()
        {
            this.InitializeComponent();

            m_State = ShowState.None;
            m_CDText = new TextBlock();
            m_CDText.FontSize = 250;

            m_Index = 0;
        }

        private void ShowPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            m_State = ShowState.Hello;
            SetupTimer();
        }

        private void ShowPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
        }
    }
}
