using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Quizzlet
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamePage : ContentPage
	{
        public QuizGame Game { get; set; }
        public int SelectedIndex { get; set; }

		public GamePage ( QuizGame GameArg )
		{
			InitializeComponent ();

            SelectedIndex = -1;
            Game = GameArg;

            CategoryLabel.Text = HttpUtility.HtmlDecode( Game.Category );
            QuestionLabel.Text = HttpUtility.HtmlDecode( Game.Question );

            foreach(string answer in Game.Answers)
            {
                Button AnswerBtn = new Button();
                AnswerBtn.Text = HttpUtility.HtmlDecode( answer );
                AnswerBtn.TextColor = Color.GhostWhite;
                AnswerBtn.CornerRadius = 50;
                AnswerBtn.BackgroundColor = Color.Transparent;
                AnswerBtn.BorderWidth = 3;
                AnswerBtn.BorderColor = Color.GhostWhite;
                AnswerBtn.Margin = new Thickness(30,0,30,5);

                AnswerBtn.Clicked += Answer_Clicked;
                

                AnswersLayout.Children.Add(AnswerBtn);
            }
		}

        private async void Answer_Clicked(object sender, EventArgs e)
        {
            Button senderObj = (Button)sender;

            LoadingIndicator.IsRunning = true;
            await Task.Delay( new Random().Next( 50, 300 ) ); //False sense of security is the best.
            LoadingIndicator.IsRunning = false;

            if ( senderObj.Text.Equals( Game.CorrectAnswer ) )
            {
                await DisplayAlert( "Alert", "Correct! Good job!", "Okay");
                await Navigation.PopModalAsync();
            }
            else
            {
                IsEnabled = false;
                await DisplayAlert("Alert", "Incorrect! Better luck next time!", "Okay");
                await Navigation.PopModalAsync();
            }
        }
    }
}