using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Quizzlet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashPage : ContentPage
	{
		public SplashPage ()
		{
			InitializeComponent ();
		}

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            IsEnabled = false;
            await SplashImage.FadeTo(0, 250 );
            LoadingIndicator.IsRunning = true;

            await Navigation.PushModalAsync(new MainPage());
            LoadingIndicator.IsRunning = false;
            SplashImage.Opacity = 1;
            IsEnabled = true;
        }
    }
}