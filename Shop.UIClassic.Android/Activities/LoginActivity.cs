namespace Shop.UIClassic.Android.Activities
{
    using System;
    using global::Android.App;
    using global::Android.Content;
    using global::Android.OS;
    using global::Android.Support.V7.App;
    using global::Android.Views;
    using global::Android.Widget;
    using Newtonsoft.Json;
    using Shop.Common.Models;
    using Shop.Common.Services;
    using Shop.UIClassic.Android.Helpers;

    [Activity(
        Label = "@string/login",
        Theme = "@style/AppTheme",
        MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {
        private ApiService apiService;
        private EditText emailEditText;
        private EditText passwordEditText;
        private ProgressBar activityIndicatorProgressBar;
        private Button loginButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.LoginPage);
            this.FindViews();
            this.HandleEvents();
            this.SetInitialData();
        }

        private void SetInitialData()
        {
            this.apiService = new ApiService();
            this.emailEditText.Text = "jzuluaga55@gmail.com";
            this.passwordEditText.Text = "123456";
            this.activityIndicatorProgressBar.Visibility = ViewStates.Invisible;
        }

        private void HandleEvents()
        {
            this.loginButton.Click += LoginButton_Click;
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.emailEditText.Text))
            {
                DiaglogService.ShowMessage(this, "Error", "You must enter an email.", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.passwordEditText.Text))
            {
                DiaglogService.ShowMessage(this, "Error", "You must enter a password.", "Accept");
                return;
            }

            this.activityIndicatorProgressBar.Visibility = ViewStates.Visible;
            this.loginButton.Enabled = false;

            var request = new TokenRequest
            {
                Password = this.passwordEditText.Text,
                Username = this.emailEditText.Text
            };

            var response = await this.apiService.GetTokenAsync(
                "https://shopzulu.azurewebsites.net",
                "/Account",
                "/CreateToken",
                request);

            this.activityIndicatorProgressBar.Visibility = ViewStates.Invisible;
            this.loginButton.Enabled = true;

            if (!response.IsSuccess)
            {
                DiaglogService.ShowMessage(this, "Error", "User or password incorrect.", "Accept");
                return;
            }

            var token = (TokenResponse)response.Result;
            var intent = new Intent(this, typeof(ProductsActivity));
            intent.PutExtra("token", JsonConvert.SerializeObject(token));
            intent.PutExtra("email", this.emailEditText.Text);
            this.StartActivity(intent);
        }

        private void FindViews()
        {
            this.emailEditText = this.FindViewById<EditText>(Resource.Id.emailEditText);
            this.passwordEditText = this.FindViewById<EditText>(Resource.Id.passwordEditText);
            this.activityIndicatorProgressBar = this.FindViewById<ProgressBar>(Resource.Id.activityIndicatorProgressBar);
            this.loginButton = this.FindViewById<Button>(Resource.Id.loginButton);
        }
    }
}