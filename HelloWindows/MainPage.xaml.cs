using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloWindows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();

            if (!keyCredentialAvailable)
            {
                // User didn't set up PIN yet
                await ShowMsg("No key credentials available");
            }

            await ShowMsg("KeyCredentials are available");

            string returnMessage = null;

            try
            {
                // Request the logged on user's consent via fingerprint swipe.
                var consentResult = await Windows.Security.Credentials.UI.UserConsentVerifier.RequestVerificationAsync("Require you to authenticate");

                switch (consentResult)
                {
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified:
                        returnMessage = "Fingerprint verified.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceBusy:
                        returnMessage = "Biometric device is busy.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DeviceNotPresent:
                        returnMessage = "No biometric device found.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.DisabledByPolicy:
                        returnMessage = "Biometric verification is disabled by policy.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.NotConfiguredForUser:
                        returnMessage = "The user has no fingerprints registered. Please add a fingerprint to the " +
                                        "fingerprint database and try again.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.RetriesExhausted:
                        returnMessage = "There have been too many failed attempts. Fingerprint authentication canceled.";
                        break;
                    case Windows.Security.Credentials.UI.UserConsentVerificationResult.Canceled:
                        returnMessage = "Fingerprint authentication canceled.";
                        break;
                    default:
                        returnMessage = "Fingerprint authentication is currently unavailable.";
                        break;
                }
            }
            catch (Exception ex)
            {
                returnMessage = "Fingerprint authentication failed: " + ex.ToString();
            }

            await ShowMsg(returnMessage);
        }

        private async Task ShowMsg(string msg)
        {
            var dlg = new MessageDialog(msg);
            await dlg.ShowAsync();
        }
    }
}
