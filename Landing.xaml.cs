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
using Newtonsoft.Json;
using Windows.ApplicationModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace College_Organizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Landing : Page
    {
        private readonly string identityID = ApplicationData.Current.LocalSettings.Values["IDENTITYPOOL_ID"].ToString();
        private event SuspendingEventHandler Suspending;
        int countEdits = -1, countAssi = -1;

        public Landing()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending; 
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            
            
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
            CourseName courseDialog = new CourseName();
            var result  = await courseDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {

                var item = AddNaviItem("Add New Course");
                landingNaviView.MenuItems.Add(item);
                landingNaviView.MenuItems.Add(new NavigationViewItemSeparator());
                landingNaviView.MenuItems.Add(new NavigationViewItemHeader().Content = "Notes");
                landingNaviView.MenuItems.Add(new NavigationViewItem() { Content = "Add New Note", Tag = "Note"});
 
                landingNaviView.SelectedItem = item;
            }
        }

        private async void AddNewNote()
        {
            CourseName courseDialog = new CourseName();
            courseDialog.Title = "New Note";
            var result = await courseDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var item = AddNaviItem(courseDialog.Title.ToString());
                landingNaviView.MenuItems.Add(item);

                landingNaviView.SelectedItem = item;
                lContentFrame.Navigate(typeof(Landing_Views.Assignments));
            }
        }

        private void UpdateCourseInfo()
        {
            
        }
        // Reminder: Make some changes later to make more generic
        private NavigationViewItem AddNaviItem(string title)
        {
            NavigationViewItem viewItem = new NavigationViewItem();
            if (title == "Add New Course")
            {
                viewItem.Content = ApplicationData.Current.LocalSettings.Values["CourseName"];
            }
            else
            {
                viewItem.Content = ApplicationData.Current.LocalSettings.Values["NoteName"];
            }
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
            else if (pageTypeName == "Note")
            {
                AddNewNote();
            }
        }
    }
}
