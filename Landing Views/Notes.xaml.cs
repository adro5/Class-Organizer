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
using Windows.Storage;
using Newtonsoft.Json;
using Windows.System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace College_Organizer.Landing_Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Assignments : Page
    {
        string JSON = "";
        List<Course> listCourses;
        Course course;
        private readonly string identityID = ApplicationData.Current.LocalSettings.Values["IDENTITYPOOL_ID"].ToString();
        private CognitoAWSCredentials cognitoCred;

        public Assignments()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            cognitoCred = e.Parameter as CognitoAWSCredentials;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            course = new Course()
            {
                courseName = ApplicationData.Current.LocalSettings.Values["CourseName"].ToString(),
                noteName = ApplicationData.Current.LocalSettings.Values["NoteName"].ToString()
            };
            rText.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string notes);
            course.notes = notes;

            await JSONFile(course);
            await UpdateAmazonDB();
            
        }

        private async Task JSONFile(Course course)
        {
            string next;
            StorageFile file2;
            bool check = false;

            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("Student.json") == null)
            {
                listCourses = new List<Course>();
                file2 = await ApplicationData.Current.LocalFolder.CreateFileAsync("Student.json");
                listCourses.Add(course);
                JSON = JsonConvert.SerializeObject(listCourses);
                await FileIO.WriteTextAsync(file2, JSON);
            }
            else
            {
                file2 = await ApplicationData.Current.LocalFolder.GetFileAsync("Student.json");
                using (StreamReader reader = new StreamReader(await file2.OpenStreamForReadAsync()))
                {
                    next = await reader.ReadToEndAsync();
                }

                listCourses = JsonConvert.DeserializeObject<List<Course>>(next);
                foreach (Course coursecheck in listCourses)
                {
                    if ((coursecheck.courseName == course.courseName) && (coursecheck.noteName == course.noteName) && (coursecheck.notes == course.notes))
                    {
                        check = true;
                        break;
                    }
                }
                if (check == false)
                {
                    listCourses.Add(course);
                }
                    
                JSON = JsonConvert.SerializeObject(listCourses);
                await FileIO.WriteTextAsync(file2, JSON);

            }
            
        }
        private async Task UpdateAmazonDB()
        {
            var dbClient = new AmazonDynamoDBClient(cognitoCred, Amazon.RegionEndpoint.USEast1);
            var addRequest = new PutItemRequest()
            {
                TableName = "Notes",
                Item = new Dictionary<string, AttributeValue>
                {
                    {"courseName", new AttributeValue { S = course.courseName}},
                    {"noteName", new AttributeValue {S = course.noteName} },
                    {"notes", new AttributeValue {S = course.notes} }
                }
            };

            await dbClient.PutItemAsync(addRequest);
        }

        private async void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file2 = await ApplicationData.Current.LocalFolder.GetFileAsync("Student.json");
            rText.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string notes);
            course.notes = notes;
            foreach (Course courseCheck in listCourses.ToList())
            {
                if ((courseCheck.courseName == course.courseName) && ((courseCheck.noteName == course.noteName) && (courseCheck.notes != course.notes)))
                {
                    courseCheck.notes = course.notes;
                }
            }

            JSON = JsonConvert.SerializeObject(listCourses);
            await FileIO.WriteTextAsync(file2, JSON);
            await UpdateAmazonDB();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("Student.json") != null)
            {
                var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Student.json");

            }
        }

        public void Remove()
        {

        }
    }
}
