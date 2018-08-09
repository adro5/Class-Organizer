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
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

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
        private int currentID = ApplicationView.GetForCurrentView().Id;
        CognitoAWSCredentials credentials;
        CognitoUser userF;
        public Landing()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            signOut.Tapped += tappedEventHandler;
        }

        private async void tappedEventHandler(object sender, TappedRoutedEventArgs e)
        {
            await userF.GlobalSignOutAsync();
            
            CoreApplicationView loginPage = CoreApplication.CreateNewView();
            int newViewId = 0;
            await loginPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(MainPage));
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.SwitchAsync(newViewId, currentID, ApplicationViewSwitchingOptions.ConsolidateViews);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var user = e.Parameter as CognitoUser;
            userF = user;
            NaviView_LandingLoginUpdate();
        }

        

        private void NaviView_LandingLoginUpdate()
        {
            credentials = userF.GetCognitoAWSCredentials(identityID, RegionEndpoint.USEast1);
            
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
            CourseName courseDialog = new CourseName()
            {
                Title = "New Note"
            };
            var result = await courseDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var item = AddNaviItem(courseDialog.Title.ToString());
                item.Tag = "NoteItem";
                landingNaviView.MenuItems.Add(item);

                landingNaviView.SelectedItem = item;
                lContentFrame.Navigate(typeof(Landing_Views.Assignments), credentials);
            }
        }

        private void UpdateCourseInfo()
        {
            
        }

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
            else if (pageTypeName == "NoteItem")
            {
                ApplicationData.Current.LocalSettings.Values["NoteName"] = invokedMenuItem.Content;
                lContentFrame.Navigate(typeof(Landing_Views.Assignments), credentials);
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            
            
        }

        private async void landingNaviView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("Student.json");
            string text;
            List<int> elements = new List<int>();
            using (StreamReader stream = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                text = await stream.ReadToEndAsync();
            }

            List<Course> courses = JsonConvert.DeserializeObject<List<Course>>(text);

            NavigationView source = sender as NavigationView;
            var selectedItem = source.SelectedItem as NavigationViewItem;

            var dbClient = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);
            var table = Table.LoadTable(dbClient, "Notes");

            DeleteItemResponse deleteItemResponse;
            

            foreach (Course forRemove in courses.ToList())
            {
                if (selectedItem.Content.ToString() == forRemove.courseName)
                {
                    var itemToDelete = new DeleteItemRequest()
                    {
                        //Update Primary Key & Make Batch Op
                        TableName = "Notes",
                        Key = new Dictionary<string, AttributeValue>() { {forRemove.courseName, new AttributeValue { S = forRemove.noteName} }}
                    };
                    
                    deleteItemResponse = await dbClient.DeleteItemAsync(itemToDelete);
                    forRemove.Remove();

                    foreach (NavigationViewItemBase item in landingNaviView.MenuItems.ToList())
                    {
                        if (item.Content == selectedItem.Content)
                        {
                            landingNaviView.MenuItems.Remove(item);
                        }
                    }
                }
                else if (selectedItem.Content.ToString() == forRemove.noteName)
                {
                    var itemToDelete = new DeleteItemRequest()
                    {
                        TableName = "Notes",
                        Key = new Dictionary<string, AttributeValue>()
                        {
                            {"courseName" ,new AttributeValue { S = forRemove.courseName } },
                            {"noteName" ,new AttributeValue { S = forRemove.noteName } }
                        }
                    };
                    deleteItemResponse = await dbClient.DeleteItemAsync(itemToDelete);
                    forRemove.Remove();

                    foreach (var item in landingNaviView.MenuItems)
                    {
                        if (item != null && item is string)
                        {
                            continue;
                        }
                        if (item is NavigationViewItem && ((NavigationViewItem)item).Content.ToString() == selectedItem.Content.ToString())
                        {
                            landingNaviView.MenuItems.Remove(item);
                            landingNaviView.SelectedItem = landingNaviView.MenuItems.FirstOrDefault();
                            break;
                        }
                    }
                }
            }
            // Checks for null courses and adds the indices to a list
            for (int i = 0; i <= courses.Count - 1; i++)
            {
                if (courses.ElementAt(i).courseName == null)
                {
                    System.Diagnostics.Debug.WriteLine(i);
                    elements.Add(i);
                }
            }
            // Deletes
            foreach (int i in elements)
            {
                System.Diagnostics.Debug.WriteLine(i + "Delete");
                courses.RemoveAt(i);
            }

            text = JsonConvert.SerializeObject(courses);
            await FileIO.WriteTextAsync(file, text);
        }
    }
}
