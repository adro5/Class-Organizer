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
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Windows.Storage;
using Windows.UI.Popups;
using Amazon.Extensions.CognitoAuthentication;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace College_Organizer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        private readonly string _clientId = ApplicationData.Current.LocalSettings.Values["CLIENT_ID"].ToString();
        private readonly string _poolId = ApplicationData.Current.LocalSettings.Values["USERPOOL_ID"].ToString();
        private readonly AmazonCognitoIdentityProviderClient _client;
        CognitoUser user;
        private int currentID = ApplicationView.GetForCurrentView().Id;

        public Login()
        {
            this.InitializeComponent();
            _client = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);
            
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool loggedIn = await CheckPasswordAsync(txtLogin.Text, pWLogin.Password);
                if (loggedIn)
                {
                    CreateNewView();
                }
            }
            catch (Exception ex)
            {
                var msgBox = new MessageDialog("Wrong Username/Password", "Log In Error");
                await msgBox.ShowAsync();
            }
        }

        private async Task<bool> CheckPasswordAsync(string username, string password)
        {
            try
            {
                CognitoUserPool userPool = new CognitoUserPool(_poolId, _clientId, _client);
                user = new CognitoUser(txtLogin.Text, _clientId, userPool, _client);

                AuthFlowResponse authFlow = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
                {
                    Password = pWLogin.Password
                }).ConfigureAwait(false);
                
                return true;

            }
            catch (Exception ex)
            {
                var msgBox = new MessageDialog(ex.Message,"Log In Error");
                await msgBox.ShowAsync();
                return false;
            }
        }

        private async void CreateNewView()
        {
            CoreApplicationView landing = CoreApplication.CreateNewView();
            int newViewId = 0;
            await landing.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(Landing), user);
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.SwitchAsync(newViewId, currentID, ApplicationViewSwitchingOptions.ConsolidateViews);
        }
    }
}
