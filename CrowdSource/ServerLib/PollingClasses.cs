using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerLib
{
    public class Poll
    {

        public Guid PollId { get; set; }

        public string Question { get; set; }

        public Answers Answers { get; set; }

        public Author Author { get; set; }

        public string Language { get; set; }

        public List<Comment> Comments { get; set; }

        public int flagId { get; set; }

        public string category { get; set; }

        public int censorshipId { get; set; }

        // Allow questions to persist forever? Database will get LARGE - Get a timeout algorithm to push old entries down the splay tree
        //public DateTime LifeTime { get; set; }
        public Poll()
        {
            PollId = Guid.NewGuid();
        }



    }

    public class Author
    {
        public Guid AuthorId { get; set; }

        public Author()
        {
            AuthorId = Guid.NewGuid();
           
        }

        public string gender {get; set; }

        public List<Poll> questions { get; set; }

        //handle?

    }

    public class Answers
    {
        
        public Guid AnswersId { get; set; }

        public Guid PollId { get; set; }

        public string answer1 { get; set; }
        public int answer1Malefrequency { get; set; }
        public int answer1Femalefrequency { get; set; }

        public string answer2 { get; set; }
        public int answer2Malefrequency { get; set; }
        public int answer2Femalefrequency { get; set; }

        public string answer3 { get; set; }
        public int answer3Malefrequency { get; set; }
        public int answer3Femalefrequency { get; set; }

        public string answer4 { get; set; }
        public int answer4Malefrequency { get; set; }
        public int answer4Femalefrequency { get; set; }

        public string answer5 { get; set; }
        public int answer5Malefrequency { get; set; }
        public int answer5Femalefrequency { get; set; }

        public string answer6 { get; set; }
        public int answer6Malefrequency { get; set; }
        public int answer6Femalefrequency { get; set; }

        public string answer7 { get; set; }
        public int answer7Malefrequency { get; set; }
        public int answer7Femalefrequency { get; set; }

        public string answer8 { get; set; }
        public int answer8Malefrequency { get; set; }
        public int answer8Femalefrequency { get; set; }

        public string answer9 { get; set; }
        public int answer9Malefrequency { get; set; }
        public int answer9Femalefrequency { get; set; }

        public string answer10 { get; set; }
        public int answer10Malefrequency { get; set; }
        public int answer10Femalefrequency { get; set; }

        // might remove for redunancies sake
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }

        public Answers()
        {
            AnswersId = Guid.NewGuid();
        }

    }

    public class Comment
    {
        // might not need this
        public Guid CommentId { get; set; }

        public Guid PollId { get; set; }

        public string comment { get; set; }

        public Comment()
        {
            CommentId = Guid.NewGuid();
        }
    }

#region Helper Classes
    public class submittedAnswer
    {
        public int answer;
        public Guid PollId;
    }

#endregion

   
}