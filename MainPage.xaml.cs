using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Amazon;
using Amazon.CognitoIdentity;
using Windows.Storage;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace College_Organizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly string _clientId = ApplicationData.Current.LocalSettings.Values["CLIENT_ID"].ToString();
        private readonly string _poolId = ApplicationData.Current.LocalSettings.Values["USERPOOL_ID"].ToString();

        public MainPage()
        {
            this.InitializeComponent();
            
        }

        private void NaviView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(NavigationViewItemBase item in NaviView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "login")
                {
                    NaviView.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(Views.Login));
        }

        private void NaviView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var invokedMenuItem = sender.MenuItems
                            .OfType<NavigationViewItem>()
                            .Where(item =>
                                 item.Content.ToString() ==
                                 args.InvokedItem.ToString())
                            .First();
            var pageTypeName = invokedMenuItem.Tag.ToString();

            switch (pageTypeName)
            {
                case "login":
                    ContentFrame.Navigate(typeof(Views.Login));
                    break;
                case "signup":
                    ContentFrame.Navigate(typeof(Views.Sign_Up));
                    break;
            }
        }
    }
}
