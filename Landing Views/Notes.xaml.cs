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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace College_Organizer.Landing_Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Assignments : Page
    {
        string JSON = "";

        public Assignments()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string notes;
            Course course = new Course();
            course.courseName = ApplicationData.Current.LocalSettings.Values["CourseName"].ToString();
            course.noteName = ApplicationData.Current.LocalSettings.Values["NoteName"].ToString();
            rText.Document.GetText(Windows.UI.Text.TextGetOptions.None, out notes);
            course.notes = notes;

            JSON += JsonConvert.SerializeObject(course);

            await JSONFile();

        }

        private async Task JSONFile()
        {
            if (ApplicationData.Current.LocalFolder.TryGetItemAsync("Student.json") == null)
            {
                var file2 = await ApplicationData.Current.LocalFolder.CreateFileAsync("Student.json");
                await FileIO.WriteTextAsync(file2, JSON);
            }
            else
            {
                var file2 = await ApplicationData.Current.LocalFolder.GetFileAsync("Student.json");
                await FileIO.WriteTextAsync(file2, JSON);
            }
        }
    }
}
