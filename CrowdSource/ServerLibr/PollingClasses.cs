using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerLibr
{

#region Helper Classes
    public class SubmittedAnswer
    {
        public int answer;
        public Guid PollId;
    }

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

   
}