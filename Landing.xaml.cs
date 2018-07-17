using Amazon.Extensions.CognitoAuthentication;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace College_Organizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Landing : Page
    {
        private readonly string identityID = ApplicationData.Current.LocalSettings.Values["IDENTITYPOOL_ID"].ToString();
        public Landing()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var user = e.Parameter as CognitoUser;
            NaviView_LandingLoginUpdate(user);
            base.OnNavigatedTo(e);
        }

        private void NaviView_LandingLoginUpdate(CognitoUser userF)
        {
            CognitoAWSCredentials credentials = userF.GetCognitoAWSCredentials(identityID, RegionEndpoint.USEast1);

        }

        private void landingNaviView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            
        }

        private async void AddNewCourse()
        {
            CourseName course = new CourseName();
            var result  = await course.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                RichEditBox richEdit = new RichEditBox();
                
                var item = AddNaviItem();
                landingNaviView.MenuItems.Add(item);
                landingNaviView.MenuItems.Add(new NavigationViewItemSeparator());
                landingNaviView.MenuItems.Add(new NavigationViewItemHeader().Content = "Assignments");
                landingNaviView.MenuItems.Add(new NavigationViewItem().Content = "Add New Assignment");
                landingNaviView.MenuItems.Add(new NavigationViewItemSeparator());
                landingNaviView.MenuItems.Add(new NavigationViewItemHeader().Content = "Tests");
                landingNaviView.MenuItems.Add(new NavigationViewItem().Content = "Add New Test");
                landingNaviView.MenuItems.Add(new NavigationViewItemSeparator());
                landingNaviView.MenuItems.Add(new NavigationViewItemHeader().Content = "Projects");
                landingNaviView.MenuItems.Add(new NavigationViewItem().Content = "Add New Project");

                landingNaviView.SelectedItem = item;
                landingGrid.Children.Add(richEdit);
            }
        }

        private void UpdateCourseInfo()
        {
            
        }
        // Reminder: Make some changes later to make more generic
        private NavigationViewItem AddNaviItem()
        {
            NavigationViewItem viewItem = new NavigationViewItem();
            viewItem.Content = ApplicationData.Current.LocalSettings.Values["CourseName"];
            return viewItem;
        }

        private void landingNaviView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            NavigationViewItem invokedMenuItem = sender.MenuItems
                            .OfType<NavigationViewItem>()
                            .Where(item =>
                                 item.Content.ToString() ==
                                 args.InvokedItem.ToString())
                            .FirstOrDefault();
            var pageTypeName = invokedMenuItem.Tag.ToString();

            if (pageTypeName == firstItem.Tag.ToString())
            {
                AddNewCourse();
            }
        }
    }
}
