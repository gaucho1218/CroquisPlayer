using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CroquisPlayer
{
    public sealed partial class MainPage : Page
    {
        private Button m_addButton;
        public List<StorageFile> m_Files;

        private void setDataCollection()
        {
            if (m_addButton == null)
                makeAddButton();
            else
                dataCollection.Add(m_addButton);
        }

        private async void makeAddButton()
        {
            if (m_addButton == null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    m_addButton = new Button();
                    m_addButton.Margin = new Thickness(2);
                    m_addButton.Width = 100;
                    m_addButton.Height = 100;
                    m_addButton.Click += AddButton_Click;

                    StackPanel stackPanel = new StackPanel();
                    SymbolIcon symbolIcon = new SymbolIcon(Symbol.Add);
                    TextBlock textBlock = new TextBlock();

                    var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                    textBlock.Text = resourceLoader.GetString("AddText");
                    textBlock.Margin = new Thickness(0, 10, 0, 0);

                    stackPanel.Children.Add(symbolIcon);
                    stackPanel.Children.Add(textBlock);

                    m_addButton.Content = stackPanel;

                    dataCollection.Add(m_addButton);
                });
            }
        }

        private async void makeImageButton(StorageFile file)
        {
            if (file != null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    Button button = new Button();
                    button.Margin = new Thickness(2);
                    button.Width = 100;
                    button.Height = 100;

                    //! button event
                    button.PointerEntered += ImgButton_Entered;
                    button.PointerExited += ImgButton_Exited;
                    button.Click += ImgButton_Click;

                    Image image = new Image();
                    BitmapImage bitmap = new BitmapImage();

                    var stream = await file.OpenReadAsync();
                    await bitmap.SetSourceAsync(stream);

                    image.Source = bitmap;
                    button.Content = image;

                    m_Files.Add(file);
                    dataCollection.Add(button);
                });
            }
        }
    }
}
