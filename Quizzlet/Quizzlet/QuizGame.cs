using System.Collections.Generic;
using System.Web;

namespace Quizzlet
{
    public class QuizGame
    {
        public List<string> Answers { get; set; }
        public string CorrectAnswer { get; set; }
        public string Question { get; set; }
        public string Category { get; set; }

        public QuizGame( List<string> Answers, string CorrectAnswer )
        {
            this.Answers = Answers;
            this.CorrectAnswer = HttpUtility.HtmlDecode(CorrectAnswer);
        }
    }
}
