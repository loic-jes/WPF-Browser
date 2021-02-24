using Braawser.Model;
using Braawser.view;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utils.IO;

namespace Braawser
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Favori> favlist = new List<Favori>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /* Sérialisation :
         * A l'ouverture du browser, s'il existe un fichier favori.xml, le lit et met automatiquement
         la liste des favoris à jour 
         * S'il ne trouve pas le fichier favori.xml, le try/catch empêche l'erreur et ne fait rien*/
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Serializer<List<Favori>> serializer = new Serializer<List<Favori>>("favoris.xml", SerializeFormat.Xml);

            try
            {
                favlist = serializer.Read("favoris.xml");

                foreach (var item in favlist)
                {

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("https://www.google.com/s2/favicons?sz=64&domain=" + item.Url);
                    bitmap.EndInit();

                    Image img = new Image
                    {
                        Source = bitmap
                    };

                    MenuItem newFav = new MenuItem
                    {
                        Header = item.Url,
                        Icon = img
                    };

                    newFav.Click += GoToFav_Click;
                    this.FavContextMenu.Items.Add(newFav);
                }
            }

            catch
            {

            }
        }

        /* Changer l'icône de l'onglet de navigation pour matcher la favicon du site actuel 
         Utilisation du dispatcher car le site est affiché sur un navView, mais le tabcontrol est sur le MainWindow*/
        public void NavViewChangedLoading(object sender, EventArgs e)
        {
            MainTabControl.Dispatcher.BeginInvoke((Action)(() =>          
            {
                Image img = new Image();
                TabItem tab = (TabItem)MainTabControl.SelectedItem;
                img.Source = BitmapImageWithTab(tab);
                img.Width = 22;
                StackPanel stackPanel = (StackPanel)tab.Header;
                stackPanel.Children.RemoveAt(0);
                stackPanel.Children.Insert(0, img);
            }));
        }

        /* Récupération, grâce à l'API Google, de l'icône du site actuel, en size 64, pour les onglets et favoris */
        private ImageSource BitmapImageWithTab(TabItem tab)
        {
            NavView view = GetNavView(tab);
            ChromiumWebBrowser browser = view.Browser;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("https://www.google.com/s2/favicons?sz=64&domain=" + browser.Address);
            bitmap.EndInit();
            return bitmap;
        }

        /* Factorisation pour récupérer la vue d'un onglet */
        private NavView GetNavView(TabItem tab)
        {
            Frame frame = (Frame)tab.Content;
            return (NavView)frame.Content;
        }

        /* Fermeture d'un onglet via le clic sur la croix
         * Ferme l'application si le dernier onglet est ouvert */
        private void CloseTab(TabItem tab)
        {
            if (MainTabControl.Items.Count == 1)
            {
                Application.Current.Shutdown();
            }
            MainTabControl.Items.Remove(tab);
        }

        /* Selectionne l'onglet en cours (via le parent du parent de l'image de croix pour fermer) et l'envoie en argument
         * à la méthode de fermeture d'onglet */
        private void ImageCross_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TabItem tab = (TabItem)((StackPanel)(((Image)sender).Parent)).Parent;
            CloseTab(tab);
        }
        /* Selectionne l'onglet en cours (s'il est selectionné par un clic droit) et l'envoie en argument
         * à la méthode de fermeture d'onglet */
        private void Click_Item_Close(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)((ContextMenu)(((MenuItem)sender).Parent)).PlacementTarget;
            CloseTab(tab);
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            CreateNewTab();
        }

        /* Construction d'une nouvelle tab / onglet : Un texte placeholder et une image de croix dans un stackpanel, inséré dans
         * un tab, inséré dans le tabcontrol*/
        public void CreateNewTab(string url = "")
        {
            if (MainTabControl.Items.Count < 27)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("asset/img/close_cross.png", UriKind.Relative);
                bitmapImage.EndInit();
                StackPanel stackPanel = new StackPanel
                {
                    Margin = new Thickness(-4, 4, 0, 0),
                    Orientation = Orientation.Horizontal
                };
                Image imageCross = new Image
                {
                    Source = bitmapImage,
                    Width = 16
                };
                imageCross.MouseDown += ImageCross_MouseDown;
                TextBlock textBlock = new TextBlock
                {
                    Text = "N"
                };
                stackPanel.Children.Insert(0, textBlock);
                stackPanel.Children.Insert(1, imageCross);
                SolidColorBrush bgColor = new SolidColorBrush();
                bgColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF5B5B5B"));
                SolidColorBrush borderColor = new SolidColorBrush();
                borderColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6A6666"));

                //Create ContextMenu
                ContextMenu contextMenu = new ContextMenu();
                MenuItem menuItemDup = new MenuItem
                {
                    Header = "Dupliquer"
                };
                menuItemDup.Click += Click_Item_Duplicate;
                MenuItem menuItemClose = new MenuItem
                {
                    Header = "Fermer"
                };
                menuItemClose.Click += Click_Item_Close;
                contextMenu.Items.Add(menuItemDup);
                contextMenu.Items.Add(menuItemClose);

                TabItem tab = new TabItem
                {
                    Header = stackPanel,
                    ContextMenu = contextMenu,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0),
                    Height = 34,
                    FontSize = 20,
                    Background = bgColor,
                    BorderBrush = borderColor
                };
                Frame frame = new Frame
                {
                    Content = new NavView(),
                    Margin = new Thickness(-5)
                };
                tab.Content = frame;
                MainTabControl.Items.Add(tab);
                MainTabControl.SelectedItem = tab;

                if (url != "")
                {
                    this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        NavView view = (NavView)frame.Content;
                        view.Browser.Address = url;
                    }));
                }
            }
        }


        /* Permet de dupliquer un onglet via un contextmenu (clic droit)  */
        private void Click_Item_Duplicate(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)((ContextMenu)(((MenuItem)sender).Parent)).PlacementTarget;
            NavView view = GetNavView(tab);
            string url = view.Browser.Address;
            CreateNewTab(url);
        }

        /* Evènement attribué à la création de nouveaux favoris pour la navigation vers le lien lié */
        private void GoToFav_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)MainTabControl.SelectedItem;
            Frame frame = (Frame)tab.Content;
            NavView view = (NavView)frame.Content;
            var x = sender;
            view.Browser.Address = (String)((MenuItem)sender).Header;
        }

        /* Capture l'url de l'onglet en cours, et créé un favori en l'ajoutant dans un contextmenu 
         + Serialization : A chaque ajout de favori, l'ajoute au fichier favori.xml (ou le créé s'il n'existe pas)*/
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)MainTabControl.SelectedItem;
            NavView view = GetNavView(tab);
            Image img = new Image
            {
                Source = BitmapImageWithTab(tab)
            };

            Favori favori = new Favori(view.Browser.Address);
            favlist.Add(favori);
            Serializer<List<Favori>> serializer = new Serializer<List<Favori>>("favoris.xml", SerializeFormat.Xml);
            serializer.Write(favlist);


            MenuItem newFav = new MenuItem
            {
                Header = view.Browser.Address,
                Icon = img
            };
            newFav.Click += GoToFav_Click;
            this.FavContextMenu.Items.Add(newFav);
        }
    }
}