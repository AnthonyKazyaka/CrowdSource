using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;

namespace PollingServ.Models
{
    #region Database models
    /// <summary>
    /// Represents a question in the db.
    /// Add question type
    /// Refactor client lib
    /// </summary>
    public class Question
    {
        public int Id { get; set; }

        public string QuestionText { get; set; }

        public string QuestionType { get; set; }

        [InverseProperty("Questions")]
        public virtual Author Author { get; set; }

        public virtual ICollection<FreeResponses> FreeResponses { get; set; }

        public virtual ICollection<PollResponses> PollResponses { get; set; }

        public virtual ICollection<QuestionMetaValues> MetaValues { get; set; }

        public int FlagLevel { get; set; }

        // delimited by ,
        //public string Category { get; set; }

        //public int CensorshipLevel { get; set; }

        // Allow questions to persist forever? Database will get LARGE - Get a timeout algorithm to push old entries down the splay tree
        //public DateTime LifeTime { get; set; }
        public Question()
        {
            FreeResponses = new List<FreeResponses>();
            MetaValues = new List<QuestionMetaValues>();
            PollResponses = new List<PollResponses>();
        }

        public DateTime DateCreated { get; set; }
        // add this to the client library

        // Rank to determine popularity (updated after every submitted answer?) - may have to update more often based on age
        public double Rank { get; set; }

    }

    /// <summary>
    /// Represents an Author in the Database
    /// </summary>
    public class Author
    {
        public int Id { get; set; }

        public Author()
        {
            
            Questions = new List<Question>();
            PollResponses = new List<PollResponses>();
            FreeResponses = new List<FreeResponses>();
        }

        public string Username { get; set; }

        public int BirthYear { get; set; }

        public string Gender { get; set; }

        public string Ethnicity { get; set; }

        public string FilterLevel { get; set; }

        public virtual ICollection<PollResponses> PollResponses { get; set; }

        public virtual ICollection<FreeResponses> FreeResponses { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public bool Active { get; set; }

        //handle?

    }

    /// <summary>
    /// Represents the answers for a single poll in the Database
    /// </summary>
    public class QuestionMetaValues
    {
        public int Id { get; set; }

        public string Value { get; set; }

        //[InverseProperty("MetaValues")]
        public virtual Question Question { get; set; }

        //[InverseProperty("MetaValues")]
        public virtual QuestionMetaOptions MetaOption { get; set; }

        public QuestionMetaValues()
        {
            //MetaOption = new QuestionMetaOptions();
        }

    }


    /// <summary>
    /// Represents the answers for a single poll in the Database
    /// </summary>
    public class QuestionMetaOptions
    {
        public int Id { get; set; }

        public string Option { get; set; }

        public string Description { get; set; }

        public virtual ICollection<QuestionMetaValues> MetaValues { get; set; }

        public QuestionMetaOptions()
        {
            MetaValues = new List<QuestionMetaValues>();
        }

    }


    /// <summary>
    /// Represents a free-answer comment for a poll in the db. -- currently anonymous
    /// </summary>
    public class FreeResponses
    {
        // might not need this
        public int Id { get; set; }

        public string Response { get; set; }

        [InverseProperty("FreeResponses")]
        public virtual Author Author { get; set; }

        [InverseProperty("FreeResponses")]
        public virtual Question Question { get; set; } 

    }

    /// <summary>
    /// Used for updating db with new statistics - will be extremely large and should never be used for clent-side processes
    /// </summary>
    public class PollResponses
    {
        public int Id { get; set; }

        public int AnswerId { get; set; }

        [InverseProperty("PollResponses")]
        public virtual Question Question { get; set; }

        [InverseProperty("PollResponses")]
        public virtual Author Author { get; set; }
    }


    #endregion

    #region Helper Classes - For Transfer
 
    public class XmlAuthor
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public int BirthYear { get; set; }

        public string Gender { get; set; }

        public string Ethnicity { get; set; }

        public string FilterLevel { get; set; }

        public bool Active { get; set; }

        //handle?

    }

    /// <summary>
    /// Represents a question in the db.
    /// Add question type
    /// Refactor client lib
    /// </summary>
    public class XmlQuestion
    {
        public int Id { get; set; }

        public string QuestionText { get; set; }

        public string QuestionType { get; set; }

        public int FlagLevel { get; set; }

        public DateTime DateCreated { get; set; }
        // add this to the client library

        // Rank to determine popularity (updated after every submitted answer?) - may have to update more often based on age
        public double Rank { get; set; }

    }

    /// <summary>
    /// Represents the answers for a single poll in the Database
    /// </summary>
    public class XmlQuestionMetaValues
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int OptionId { get; set; }

    }


    /// <summary>
    /// Represents the answers for a single poll in the Database
    /// </summary>
    public class XmlQuestionMetaOptions
    {
        public int Id { get; set; }

        public string Option { get; set; }

        public string Description { get; set; }

    }


    /// <summary>
    /// Represents a free-answer comment for a poll in the db. -- currently anonymous
    /// </summary>
    public class XmlFreeResponses
    {
        // might not need this
        public int Id { get; set; }

        public string Response { get; set; }

        public int AuthorId { get; set; }

    }

    /// <summary>
    /// Used for updating db with new statistics - will be extremely large and should never be used for clent-side processes
    /// </summary>
    public class XmlPollResponses
    {
        public int Id { get; set; }

        public int AnswerId { get; set; }

        public int AuthorId { get; set; }
    }

#endregion

    #region Ranking Algorithm
    class Ranking
    {
        /// <summary>
        /// Will give rank to question based on frequency and creation date
        /// </summary>
        /// <param name="question"></param>
        public static void Rank(ref Question question)
        {
            // using simplified hacker news algorithm for now
            question.Rank = Int32.Parse(question.MetaValues.Where(q => q.MetaOption.Option == "TotalFrequency").Single().Value) / (Math.Pow((DateTime.Now - question.DateCreated).Hours + 2,1.5));
        }
    }
    #endregion
   
}