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
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Windows.Storage;
using Windows.UI.Popups;
using Amazon.CognitoIdentityProvider.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace College_Organizer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Sign_Up : Page
    {
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(ApplicationData.Current.LocalSettings.Values["IDENTITYPOOL_ID"].ToString(), RegionEndpoint.USEast1);
        private readonly string _clientId = ApplicationData.Current.LocalSettings.Values["CLIENT_ID"].ToString();
        private readonly string _poolId = ApplicationData.Current.LocalSettings.Values["USERPOOL_ID"].ToString();
        private readonly AmazonCognitoIdentityProviderClient _client;

        public Sign_Up()
        {
            this.InitializeComponent();
            _client = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.USEast1);

        }

        private async void btnSign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SignUpRequest signUpRequest = new SignUpRequest()
                {
                    ClientId = _clientId,
                    Password = pW.Password,
                    Username = txtEmail.Text
                };
                AttributeType nameAttribute = new AttributeType()
                {
                    Name = "given_name",
                    Value = txtName.Text
                };
                signUpRequest.UserAttributes.Add(nameAttribute);

                var signUpResult = await _client.SignUpAsync(signUpRequest);

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                var msgBox = new MessageDialog(message, "Sign Up Error");
                await msgBox.ShowAsync();
            }
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Amazon.CognitoIdentityProvider.Model.ConfirmSignUpRequest confirmSignUpRequest = new ConfirmSignUpRequest()
                {
                    Username = txtEmail.Text,
                    ClientId = _clientId,
                    ConfirmationCode = txtConfirm.Text
                };

                var confirmResult = await _client.ConfirmSignUpAsync(confirmSignUpRequest);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                var msgBox = new MessageDialog(message, "Sign Up Error");
                await msgBox.ShowAsync();
            }
        }
    }
}
