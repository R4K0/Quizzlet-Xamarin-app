using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web;

namespace Quizzlet
{
    public partial class MainPage : ContentPage
    {
        string[] Difficulties = new string[] { "Easy", "Medium", "Hard", "Any" };
        private int DifficultyIndex { get; set; }
        private int CategoryIndex { get; set; }
        static public HttpClient HTTPClient = new HttpClient();

        private Dictionary<string, int> IDMapping = new Dictionary<string, int>();
        private List<string> CategoriesList = new List<string>();

        public MainPage()
        {
            InitializeComponent();
            DifficultyIndex = 3;
            CategoryIndex = -1;

            PopulateCategories();
        }

        private async void PopulateCategories()
        {
            CategoriesList.Add("Random Category");
            LoadingIndicator.IsRunning = true;

            HttpResponseMessage Response;
            try
            {
                Response = await HTTPClient.GetAsync("https://opentdb.com/api_category.php");
            }
            catch( Exception E )
            {
                await DisplayAlert("Alert", "Couldn't load the categories!\nReason: " + E.Message, "Okay");
                LoadingIndicator.IsRunning = false;
                App.Current.MainPage = new SplashPage();
                return;
            }
            
            if (!Response.IsSuccessStatusCode)
            {
                await DisplayAlert("Alert", "Couldn't load the categories!\nReason: " + Response.ReasonPhrase, "Okay");
                LoadingIndicator.IsRunning = false;
                App.Current.MainPage = new SplashPage();
                return;
            }

            String RawContent = await Response.Content.ReadAsStringAsync();

            JObject JSON = JObject.Parse(RawContent);
            JToken Categories = JSON.GetValue("trivia_categories");

            foreach(JToken Category in Categories)
            {
                String CatName = Category.Value<String>("name");
                CategoriesList.Add(CatName);
                IDMapping.Add(CatName, Category.Value<int>("id"));
            }
            LoadingIndicator.IsRunning = false;
        }

        private async void Start_Game(object sender, EventArgs e)
        {
            LoadingIndicator.IsRunning = true;
            HttpResponseMessage Response;

            string Difficulty = DifficultyIndex == 3 ? "" : "&difficulty=" + Difficulties[DifficultyIndex].ToLower();
            string Category = CategoryIndex == -1 ? "" : "&category=" + CategoryIndex;
            try
            {
                Response = await HTTPClient.GetAsync(string.Format( "https://opentdb.com/api.php?type=multiple&amount=1&{0}{1}", Difficulty, Category ) );
            }
            catch (Exception E)
            {
                await DisplayAlert("Alert", "Couldn't get the game details!\nReason: " + E.Message, "Okay");
                LoadingIndicator.IsRunning = false;
                return;
            }

            String RawContent = await Response.Content.ReadAsStringAsync();

            JObject JSON = JObject.Parse(RawContent);
            JObject Quiz = (JObject) JSON.GetValue("results").First;

            List<string> Questions = Quiz.GetValue("incorrect_answers").ToObject<List<string>>();
            String CorrectQuestion = Quiz.Value<string>("correct_answer");

            Questions.Add(CorrectQuestion);
            Questions = Questions.OrderBy(element => Guid.NewGuid()).ToList<string>();

            QuizGame Game = new QuizGame(Questions, CorrectQuestion);
            Game.Category = (string)Quiz.GetValue("category");
            Game.Question = (string)Quiz.GetValue("question");


            await Navigation.PushModalAsync( new GamePage( Game ) );
            LoadingIndicator.IsRunning = false;
        }

        private async void Difficulty_Change(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;

            String Answer = await DisplayActionSheet("Select a Difficulty", "Cancel", null, Difficulties);

            if ( Answer == null || Answer.Equals( "Cancel" ) )
            {
                return;
            }

            DifficultyIndex = Array.IndexOf(Difficulties, Answer );

            btnSender.Text = Answer;
        }

        private async void Category_Change(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;

            String Answer = await DisplayActionSheet("Select a Category", "Cancel", null, CategoriesList.ToArray());

            if (Answer == null || Answer.Equals("Cancel"))
            {
                return;
            }

            CategoryIndex = Answer == "Random Category" ? -1 : IDMapping[Answer];
            btnSender.Text = Answer;
        }
    }
}
