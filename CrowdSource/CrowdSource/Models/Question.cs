using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using Microsoft.Advertising.Mobile.UI;
using System.Windows.Controls.DataVisualization;

namespace CrowdSource
{
    public class Question
    {
        public enum FlagStatus
        {
            NotFlagged,
            FlaggedInappropriate,
            FlaggedWrongCategory,
            FlaggedSpam
        };

        public FlagStatus flagStatus = FlagStatus.NotFlagged;
        private string questionText;

        private int? _currentAnswerSelected = null;

        private int _questionID;

        private int timesSpamFlagged = 0;
        private int timesInappropriateFlagged = 0;
        private int timesWrongCategoriesFlagged = 0;

        private List<PollOption> _pollOptionList = new List<PollOption>();
        private CensorshipLevel _censorshipLevel = CensorshipLevel.Kids;
        private TypeOfQuestion _questionType = TypeOfQuestion.None;
        private List<QuestionCategory> questionCategoriesList = new List<QuestionCategory>();

        private List<Response> _responses = new List<Response>();

        private List<InfoBreakdown> breakdowns = new List<InfoBreakdown>();
        public List<InfoBreakdown> Breakdowns
        {
            get { return breakdowns; }
        }

        public int QuestionID
        {
            get
            {
                return _questionID;
            }
            set
            {
                _questionID = value;
            }
        }

        //public int? SelectedPollIndex
        //{
        //    get { return _currentAnswerSelected; }
        //    set
        //    {
        //        int newIndex = value ?? -1;
        //        SwitchVote(newIndex);
        //        _currentAnswerSelected = value;
        //    }
        //}

        public List<PollOption> PollOptions
        {
            get
            {
                return _pollOptionList;
            }
            set
            {
                _pollOptionList = value;
            }
        }

        public int TotalVotes
        {
            get
            {
                return Votes.Count;
            }
            //set
            //{
            //    totalNumberVotes = value;
            //}
        }

        //If we're sending a list of every vote for a poll, won't this be really awful when there are a lot of votes on a question?
        private List<Vote> _voteList = new List<Vote>();
        public List<Vote> Votes
        {
            get { return _voteList; }
            set { _voteList = value; }
        }

        public void VoteForOption(PollOption option)
        {
            Vote v = new Vote(this, option, MainPage.userInfo);
            
            //This makes sure that only one person can have one vote for a question
            //Uncomment this when testing full functoinality in Release.
            //if (_voteList.Where(x => (x.Voter == MainPage.userInfo && x.VoteQuestion._questionID == this._questionID)).Count() > 0)
            //{
            //    _voteList.Remove(_voteList.Where(x => (x.Voter == MainPage.userInfo && x.VoteQuestion._questionID == this._questionID)).First());
            //}

            _voteList.Add(v);
        }       

        private int totalCountOfVotesAndResponses(User.Gender gender)
        {
            int total = 0;
            foreach (Vote vote in Votes)
            {
                if (vote.UserGender == gender)
                    total++;
            }
            foreach (Response res in Responses)
            {
                if (res.UserGender == gender)
                    total++;
            }

            return total;
        }
        
#region Constructors
        /// <summary>
        /// Initializes an object of type PollQuestion.
        /// </summary>
        /// <param name="questionText"></param>
        /// <param name="answers"></param>
        /// <param name="categories"></param>
        /// <param name="censorship"></param>
        public Question(string qText, List<PollOption> options, List<QuestionCategory> categories, CensorshipLevel censorship, TypeOfQuestion type)
        {
            questionText = qText;
            _pollOptionList = options;
            questionCategoriesList = categories;
            _censorshipLevel = censorship;
            _questionType = type;
            CreateBreakdownList();
        }

        public Question(string qText, List<PollOption> options, List<QuestionCategory> categories, CensorshipLevel censorship, TypeOfQuestion type, int qID)
        {
            questionText = qText;
            _pollOptionList = options;
            questionCategoriesList = categories;
            _censorshipLevel = censorship;
            _questionType = type;
            QuestionID = qID;
            CreateBreakdownList();
        }

        public Question()
        {
            CreateBreakdownList();
        }
#endregion

        private void CreateBreakdownList()
        {
            breakdowns.Add(new InfoBreakdown(this, User.Gender.Male));          //Breakdown for males
            breakdowns.Add(new InfoBreakdown(this, User.Gender.Female));        //and females
        }

        public override string ToString()
        {
            return questionText;
        }

        public string ReturnCategoriesString()
        {
            string text = "";

            for (int i = 0; i < questionCategoriesList.Count; i++)
            {
                if (i == questionCategoriesList.Count - 1)
                    text += questionCategoriesList[i].ToString();
                else text += questionCategoriesList[i].ToString() + ", ";
            }
            
            
            return text;
        }

        public List<Response> Responses
        {
            get
            {
                return _responses;
            }
            set
            {
                _responses = value;
            }
        }

        /// <summary>
        /// Returns the number of times a question has been flagged as spam.
        /// </summary>
        /// <returns></returns>
        public int GetTimesSpamFlagged()
        {
            return timesSpamFlagged;
        }

        public void FlagSpam()
        {
            timesSpamFlagged++;
        }

        /// <summary>
        /// Returns the number of times a question has been flagged as inappropriate for a censorship level.
        /// </summary>
        /// <returns></returns>
        public int GetTimesInappropriateFlagged()
        {
            return timesInappropriateFlagged;
        }

        /// <summary>
        /// Increments the number of times a question was flagged inappropriate.
        /// </summary>
        public void FlagInappropriate()
        {
            timesInappropriateFlagged++;
        }

        /// <summary>
        /// Decrements the number of times a question was flagged inappropriate.
        /// </summary>
        public void UnflagInappropriate()
        {
            timesInappropriateFlagged--;
        }

        /// <summary>
        /// Returns the number of times a question has been flagged as the wrong category.
        /// </summary>
        /// <returns></returns>
        public int GetWrongCategoriesFlagged()
        {
            return timesWrongCategoriesFlagged;
        }

        /// <summary>
        /// Increments the number of times that a question has been flagged as the wrong category.
        /// </summary>
        public void FlagWrongCategory()
        {
            timesWrongCategoriesFlagged++;
        }

        /// <summary>
        /// Decrements the number of times that a question has been flagged as the wrong category.
        /// </summary>
        public void UnflagWrongCategory()
        {
            timesWrongCategoriesFlagged--;
        }
        
        /// <summary>
        /// Returns the answer at a given index in the list of answers.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PollOption GetPollOption(int index = -1)
        {
            return _pollOptionList[index];
        }

        /// <summary>
        /// Returns the list of answers.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<PollOption> GetPollOptions()
        {
            return _pollOptionList;
        }

        ///// <summary>
        ///// Set the text of an already existing answer to an inputted string.
        ///// </summary>
        ///// <param name="index"></param>
        ///// <param name="answerText"></param>
        //public bool SetExistingAnswer(int index, string ansText)
        //{
        //    if (index < pollOptionList.Count)
        //    {
        //        pollOptionList[index].text = ansText;
        //        return true;
        //    }
        //    else return false;
        //}

        ///// <summary>
        ///// Adds an answer to the list of answers to a PollQuestion.
        ///// </summary>
        ///// <param name="answerText"></param>
        //public void Add(string ansText)
        //{
        //    PollOption ans = new PollOption();
        //    ans.text = ansText;
        //    //ans.numberVotedFor = 0;
        //    pollOptionList.Add(ans);
        //}

        public enum CensorshipLevel
        {
            Kids,
            Teens,
            Adults,
            None
        }
        /// <summary>
        /// Gets the censorship level of the question.
        /// </summary>
        /// <returns></returns>
        public CensorshipLevel GetCensorshipLevel()
        {
            return _censorshipLevel;
        }

        /// <summary>
        /// Sets the censorship level of the question.
        /// </summary>
        /// <param name="CensorshipLevel"></param>
        public void SetCensorshipLevel(CensorshipLevel level)
        {
            _censorshipLevel = level;
        }

        //public static int numOfCategories = 13;

        public enum TypeOfQuestion
        {
            PollQuestion,
            ResponseQuestion,
            Both,
            None
        }

        public TypeOfQuestion QuestionType
        {
            get
            {
                return _questionType;
            }
            set
            {
                _questionType = value;
            }

        }
        
        public enum QuestionCategory
        {
            Adult,
            Books,
            Business,
            Education,
            Lifestyles,
            Movies,
            Pets,
            Politics,
            Sports,
            Technology,
            Television,
            VideoGames,
            WorldNews,
            None
        }

        /// <summary>
        /// Returns the category of the question.
        /// </summary>
        /// <returns></returns>
        public QuestionCategory GetQuestionCategory(int index)
        {
            return questionCategoriesList[index];
        }

        public List<QuestionCategory> CategoryList
        {
            get { return questionCategoriesList; }
            set { questionCategoriesList = value; }
        }

        /// <summary>
        /// Sets the category of the question.
        /// </summary>
        /// <param name="category"></param>
        public void SetQuestionCategory(int index, QuestionCategory category)
        {
            questionCategoriesList[index] = category;
        }

        public string QuestionText
        {
            get
            {
                return questionText;
            }
            set
            {
                questionText = value;
            }
        }

        /// <summary>
        /// Returns the percentage of people who voted for a certain answer to a specified degree of precision.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public double GetVotedPercentage(int index, int precision)
        {
            double percentage;
            if (PollOptions[index].VoteCount == 0)
                percentage = 0.0;
            else percentage = Math.Round(100.0 * ((double)PollOptions[index].VoteCount / TotalVotes), precision);
            return percentage;
        }

        /// <summary>
        /// Casts a vote for an answer by incrementing the numberVotedFor in that answer.
        /// </summary>
        /// <param name="index"></param>
        //public void CastVote(int index)
        //{
        //    SelectedPollIndex = index;
        //}

        /// <summary>
        /// Switches a vote from a previous answer to the currently selected answer.
        /// </summary>
        /// <param name="prevIndex"></param>
        /// <param name="currIndex"></param>
        //public void SwitchVote(int newIndex)
        //{
        //    //This might be better to do as an event (when unselected)
        //    if (SelectedPollIndex == null)
        //    {
        //        _pollOptionList[newIndex].IsSelected = true;
        //        //TotalVotes++;
        //    }
        //    else
        //    {
        //        _pollOptionList[(int)SelectedPollIndex].IsSelected = false;
        //        _pollOptionList[(int)newIndex].IsSelected = true;

        //    }
        //    //This might be better to do as an event (when selected)
        //}

    }
}