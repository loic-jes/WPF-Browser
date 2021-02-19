using Braawser.view;
using CefSharp.Wpf;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Braawser
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void NavViewChangedLoading(object sender, EventArgs e)
        {
            MainTabControl.Dispatcher.Invoke(() =>
            {
                Image img = new Image();
                BitmapImage bitmap = new BitmapImage();
                TabItem tab = (TabItem)MainTabControl.SelectedItem;
                Frame frame = (Frame)tab.Content;
                NavView view = (NavView)frame.Content;
                ChromiumWebBrowser browser = view.Browser;
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("https://www.google.com/s2/favicons?domain=" + browser.Address);
                bitmap.EndInit();
                img.Source = bitmap;
                tab.Header = img;
            });
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            CreateNewTab();
        }

        public void CreateNewTab(string url = "")
        {
            TabItem tab = new TabItem
            {
                Header = "N",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 34,
                FontSize = 20
            };
            Frame frame = new Frame
            {
                Content = new NavView(),
                Margin = new Thickness(-5)
            };
            tab.Content = frame;
            MainTabControl.Items.Add(tab);
            MainTabControl.SelectedItem = tab;

            if(url != "")
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    NavView view = (NavView)frame.Content;
                    view.Browser.Address = url;
                }));
            }
        }
    }
}