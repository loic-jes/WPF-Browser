﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;

namespace Braawser.view
{
    /// <summary>
    /// Logique d'interaction pour NavView.xaml
    /// </summary>
    public partial class NavView : Page, IRequestHandler
    {
        private const string HomeUrl = "https://www.google.fr";
        private MainWindow Window = (MainWindow)Application.Current.MainWindow;

        //Event Changed Loading
        public delegate void ChangedLoadingEventHandler(object sender, EventArgs e);
        public event ChangedLoadingEventHandler ChangedLoading;

        public NavView()
        {
            InitializeComponent();
            this.ChangedLoading += Window.NavViewChangedLoading;
            Browser.RequestHandler = this;
        }

        private void ChangedLoading_Browser(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                ChangedLoadingEventHandler handler = ChangedLoading;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }
        }

        private void Button_Click_Home(object sender, RoutedEventArgs e)
        {
            Browser.Load(HomeUrl);
        }

        private void TextUrl_KeyUp_Navigate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Browser.Load(TextUrl.Text);
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return false;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            if (targetDisposition == CefSharp.WindowOpenDisposition.NewBackgroundTab)
            {
                Window.Dispatcher.Invoke(() => Window.CreateNewTab(targetUrl));
                return true;
            }
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }
    }
}