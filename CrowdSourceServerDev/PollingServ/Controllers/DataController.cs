/*
 *  CrowdSpark Server 
 *  Data Controller
 * 
 *  Copyright <>.
 *  Author: Sam Moore
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PollingServ.Models;
using System.Web.Util;
using MvcContrib.ActionResults;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;


namespace PollingServ.Controllers
{
    /// <summary>
    /// Controller for controlling data between the database and view
    /// This is where most of the server functionality is.
    /// Might move some to a business logic model.
    /// </summary>
    [Authorize]
    public class DataController : Controller
    {
        // The database
        private PollEntities db = new PollEntities();

        #region index and error

        [Authorize]
        public ActionResult populateMetaDB()
        {
            

            List<string> statsPerOption = new List<string>();
            statsPerOption.Add("");
            statsPerOption.Add("Frequency");
            statsPerOption.Add("MaleFrequency");
            statsPerOption.Add("FemaleFrequency");
            statsPerOption.Add("MinorFrequency");
            statsPerOption.Add("YoungFrequency");
            statsPerOption.Add("MiddleAgedFrequency");
            statsPerOption.Add("OlderFrequency");
            statsPerOption.Add("OldestFrequency");
            statsPerOption.Add("CaucasianFrequency");
            statsPerOption.Add("AfricanAmericanFrequency");
            statsPerOption.Add("HispanicFrequency");
            statsPerOption.Add("AsianFrequency");
            statsPerOption.Add("MiddleEasternFrequency");
            statsPerOption.Add("PacificIslanderFrequency");
            statsPerOption.Add("NativeAmericanFrequency");
            statsPerOption.Add("OtherFrequency");

            List<string> statsPerQuestion = new List<string>();
            statsPerQuestion.Add("TotalFrequency");
            statsPerQuestion.Add("TotalMaleFrequency");
            statsPerQuestion.Add("TotalFemaleFrequency");
            statsPerQuestion.Add("TotalMinorFrequency");
            statsPerQuestion.Add("TotalYoungFrequency");
            statsPerQuestion.Add("TotalMiddleAgedFrequency");
            statsPerQuestion.Add("TotalOlderFrequency");
            statsPerQuestion.Add("TotalOldestFrequency");
            statsPerQuestion.Add("TotalCaucasianFrequency");
            statsPerQuestion.Add("TotalAfricanAmericanFrequency");
            statsPerQuestion.Add("TotalHispanicFrequency");
            statsPerQuestion.Add("TotalAsianFrequency");
            statsPerQuestion.Add("TotalMiddleEasternFrequency");
            statsPerQuestion.Add("TotalPacificIslanderFrequency");
            statsPerQuestion.Add("TotalNativeAmericanFrequency");
            statsPerQuestion.Add("TotalOtherFrequency");
            statsPerQuestion.Add("Categories");
            statsPerQuestion.Add("Censorship");
            statsPerQuestion.Add("NumberOfPollOptions");

            foreach (string stat in statsPerOption)
            {
                for (int i = 0; i < 10; i++)
                {
                    QuestionMetaOptions option = new QuestionMetaOptions();
                    option.Option = ("PollOption"+i)+stat;
                    db.QuestionMetaOptions.Add(option);
                }
            }

            foreach (string stat in statsPerQuestion)
            {
                QuestionMetaOptions option = new QuestionMetaOptions();
                option.Option = stat;
                db.QuestionMetaOptions.Add(option);
            }

            db.SaveChanges();
            return new XmlResult(true);
            
        }

        /// <summary>
        /// Here for consistincies sake, will likely never be hit.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("error");
        }

        /// <summary>
        /// Cause every good view needs a error screen
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Error()
        {
            // initializes database if it doesn't exist
            if(!db.Database.Exists())
                db.Database.Initialize(true);
            return new XmlResult(false);

        }

        #endregion

        #region Database logic

        /// <summary>
        /// POST
        /// Registers a new Author with the database
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewAuthor()
        {
            // get POST variables
            string username = Request["username"];
            string birthYear = Request["birthYear"];
            string gender = Request["gender"];
            string ethnicity = Request["ethnicity"];
            string filterLevel = Request["filterLevel"];

            // It's an MVC thing.
            if (ModelState.IsValid)
            {
                Author newAuthor = new Author();

                newAuthor.Active = true;
                newAuthor.BirthYear = Int32.Parse(birthYear);
                newAuthor.FilterLevel = filterLevel;
                newAuthor.Gender = gender;
                newAuthor.Ethnicity = ethnicity;
                newAuthor.Username = username;
                
                db.Authors.Add(newAuthor);


                db.SaveChanges();

                XmlAuthor author = new XmlAuthor();
                author.Active = newAuthor.Active;
                author.BirthYear = newAuthor.BirthYear;
                author.Ethnicity = newAuthor.Ethnicity;
                author.FilterLevel = newAuthor.FilterLevel;
                author.Gender = newAuthor.Gender;
                author.Id = newAuthor.Id;
                author.Username = newAuthor.Username;

                // return the Author
                return new XmlResult(author);
            }
            else
            {
                return new XmlResult(false);
            }
        }

        /// <summary>
        /// POST
        /// Submits Poll to DB
        /// </summary>
        /// <param name="poll"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitQuestion()
        {
            string questionText = Request["questionText"];
            int authorId = Int32.Parse(Request["authorId"]);
            int flagLevel = 0;
            double rank = 0;
            DateTime dateCreated = DateTime.Now;
            string questionType = Request["questionType"];
            List<QuestionMetaValues> metaValues = new List<QuestionMetaValues>();
             
            Question poll = new Question();

            poll.Author = db.Authors.Where(a => a.Id == authorId).Single();
            poll.DateCreated = dateCreated;
            poll.FlagLevel = flagLevel;
            poll.QuestionText = questionText;
            poll.QuestionType = questionType;
            poll.Rank = rank;
            db.Question.Add(poll);
            db.SaveChanges();
            if(questionType == "Poll" || questionType == "PollResponse")
            {
                int numberOfOptions = Int32.Parse(Request["numberOfOptions"]);

                for (int i = 0; i < numberOfOptions; i++)
                {
                    QuestionMetaValues metaValue = new QuestionMetaValues();
                    metaValue.Value = Request["pollOption" + i];
                    string PollOptionName = "PollOption" + i;
                    metaValue.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName).Single();
                    poll.MetaValues.Add(metaValue);
                    metaValue.Question = poll;

                    // main frequency
                    QuestionMetaValues metaValueFrequency = new QuestionMetaValues();
                    metaValueFrequency.Value = "0";
                    metaValueFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "Frequency").Single();
                    poll.MetaValues.Add(metaValueFrequency);
                    metaValueFrequency.Question = db.Question.Where(q => q.Id == poll.Id).Single();
                    

                    // male
                    QuestionMetaValues metaValueMaleFrequency = new QuestionMetaValues();
                    metaValueMaleFrequency.Value = "0";
                    metaValueMaleFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "MaleFrequency").Single();
                    metaValueMaleFrequency.Question = poll;

                    // female
                    QuestionMetaValues metaValueFemaleFrequency = new QuestionMetaValues();
                    metaValueFemaleFrequency.Value = "0";
                    metaValueFemaleFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "FemaleFrequency").Single();
                    metaValueFemaleFrequency.Question = poll;

                    // age 18<
                    QuestionMetaValues metaValueMinorFrequency = new QuestionMetaValues();
                    metaValueMinorFrequency.Value = "0";
                    metaValueMinorFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "MinorFrequency").Single();
                    metaValueMinorFrequency.Question = poll;

                    // age 18-39
                    QuestionMetaValues metaValueYoungFrequency = new QuestionMetaValues();
                    metaValueYoungFrequency.Value = "0";
                    metaValueYoungFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "YoungFrequency").Single();
                    metaValueYoungFrequency.Question = poll;

                    // age 40-59
                    QuestionMetaValues metaValueMiddleAgedFrequency = new QuestionMetaValues();
                    metaValueMiddleAgedFrequency.Value = "0";
                    metaValueMiddleAgedFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "MiddleAgedFrequency").Single();
                    metaValueMiddleAgedFrequency.Question = poll;

                    // age 60-74
                    QuestionMetaValues metaValueOlderFrequency = new QuestionMetaValues();
                    metaValueOlderFrequency.Value = "0";
                    metaValueOlderFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "OlderFrequency").Single();
                    metaValueOlderFrequency.Question = poll;

                    // age 75+
                    QuestionMetaValues metaValueOldestFrequency = new QuestionMetaValues();
                    metaValueOldestFrequency.Value = "0";
                    metaValueOldestFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "OldestFrequency").Single();
                    metaValueOldestFrequency.Question = poll;

                    // Caucasian
                    QuestionMetaValues metaValueCaucasianFrequency = new QuestionMetaValues();
                    metaValueCaucasianFrequency.Value = "0";
                    metaValueCaucasianFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "CaucasianFrequency").Single();
                    metaValueCaucasianFrequency.Question = poll;

                    // African American / Black
                    QuestionMetaValues metaValueAfricanAmericanFrequency = new QuestionMetaValues();
                    metaValueAfricanAmericanFrequency.Value = "0";
                    metaValueAfricanAmericanFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "AfricanAmericanFrequency").Single();
                    metaValueAfricanAmericanFrequency.Question = poll;

                    // Hispanic
                    QuestionMetaValues metaValueHispanicFrequency = new QuestionMetaValues();
                    metaValueHispanicFrequency.Value = "0";
                    metaValueHispanicFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "HispanicFrequency").Single();
                    metaValueHispanicFrequency.Question = poll;

                    // Asian
                    QuestionMetaValues metaValueAsianFrequency = new QuestionMetaValues();
                    metaValueAsianFrequency.Value = "0";
                    metaValueAsianFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "AsianFrequency").Single();
                    metaValueAsianFrequency.Question = poll;

                    // Middle Eastern
                    QuestionMetaValues metaValueMiddleEasternFrequency = new QuestionMetaValues();
                    metaValueMiddleEasternFrequency.Value = "0";
                    metaValueMiddleEasternFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "MiddleEasternFrequency").Single();
                    metaValueMiddleEasternFrequency.Question = poll;

                    // Pacific Islander
                    QuestionMetaValues metaValuePacificIslanderFrequency = new QuestionMetaValues();
                    metaValuePacificIslanderFrequency.Value = "0";
                    metaValuePacificIslanderFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "PacificIslanderFrequency").Single();
                    metaValuePacificIslanderFrequency.Question = poll;

                    // Native American/Alaskan
                    QuestionMetaValues metaValueNativeAmericanFrequency = new QuestionMetaValues();
                    metaValueNativeAmericanFrequency.Value = "0";
                    metaValueNativeAmericanFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "NativeAmericanFrequency").Single();
                    metaValueNativeAmericanFrequency.Question = poll;

                    // Other
                    QuestionMetaValues metaValueOtherFrequency = new QuestionMetaValues();
                    metaValueOtherFrequency.Value = "0";
                    metaValueOtherFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == PollOptionName + "OtherFrequency").Single();
                    metaValueOtherFrequency.Question = poll;

                    db.QuestionMetaValues.Add(metaValue);
                    db.QuestionMetaValues.Add(metaValueFrequency);
                    db.QuestionMetaValues.Add(metaValueMaleFrequency);
                    db.QuestionMetaValues.Add(metaValueFemaleFrequency);
                    db.QuestionMetaValues.Add(metaValueMinorFrequency);
                    db.QuestionMetaValues.Add(metaValueYoungFrequency);
                    db.QuestionMetaValues.Add(metaValueMiddleAgedFrequency);
                    db.QuestionMetaValues.Add(metaValueOlderFrequency);
                    db.QuestionMetaValues.Add(metaValueOldestFrequency);
                    db.QuestionMetaValues.Add(metaValueCaucasianFrequency);
                    db.QuestionMetaValues.Add(metaValueAfricanAmericanFrequency);
                    db.QuestionMetaValues.Add(metaValueAsianFrequency);
                    db.QuestionMetaValues.Add(metaValueMiddleEasternFrequency);
                    db.QuestionMetaValues.Add(metaValuePacificIslanderFrequency);
                    db.QuestionMetaValues.Add(metaValueNativeAmericanFrequency);
                    db.QuestionMetaValues.Add(metaValueOtherFrequency);
                    db.QuestionMetaValues.Add(metaValueHispanicFrequency);
                    
                }

                // number of poll options
                QuestionMetaValues metaValueNumberOfOptions = new QuestionMetaValues();
                metaValueNumberOfOptions.Value = numberOfOptions.ToString();
                metaValueNumberOfOptions.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "NumberOfPollOptions").Single();
                metaValueNumberOfOptions.Question = poll;

                db.QuestionMetaValues.Add(metaValueNumberOfOptions);


                

            }

            // totals

            // main frequency
            QuestionMetaValues metaValueTotalFrequency = new QuestionMetaValues();
            metaValueTotalFrequency.Value = "0";
            metaValueTotalFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalFrequency").Single();
            metaValueTotalFrequency.Question = poll;

            // male
            QuestionMetaValues metaValueTotalMaleFrequency = new QuestionMetaValues();
            metaValueTotalMaleFrequency.Value = "0";
            metaValueTotalMaleFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalMaleFrequency").Single();
            metaValueTotalMaleFrequency.Question = poll;

            // female
            QuestionMetaValues metaValueTotalFemaleFrequency = new QuestionMetaValues();
            metaValueTotalFemaleFrequency.Value = "0";
            metaValueTotalFemaleFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalFemaleFrequency").Single();
            metaValueTotalFemaleFrequency.Question = poll;

            // age 18<
            QuestionMetaValues metaValueTotalMinorFrequency = new QuestionMetaValues();
            metaValueTotalMinorFrequency.Value = "0";
            metaValueTotalMinorFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalMinorFrequency").Single();
            metaValueTotalMinorFrequency.Question = poll;

            // age 18-39
            QuestionMetaValues metaValueTotalYoungFrequency = new QuestionMetaValues();
            metaValueTotalYoungFrequency.Value = "0";
            metaValueTotalYoungFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalYoungFrequency").Single();
            metaValueTotalYoungFrequency.Question = poll;

            // age 40-59
            QuestionMetaValues metaValueTotalMiddleAgedFrequency = new QuestionMetaValues();
            metaValueTotalMiddleAgedFrequency.Value = "0";
            metaValueTotalMiddleAgedFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalMiddleAgedFrequency").Single();
            metaValueTotalMiddleAgedFrequency.Question = poll;

            // age 60-74
            QuestionMetaValues metaValueTotalOlderFrequency = new QuestionMetaValues();
            metaValueTotalOlderFrequency.Value = "0";
            metaValueTotalOlderFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalOlderFrequency").Single();
            metaValueTotalOlderFrequency.Question = poll;

            // age 75+
            QuestionMetaValues metaValueTotalOldestFrequency = new QuestionMetaValues();
            metaValueTotalOldestFrequency.Value = "0";
            metaValueTotalOldestFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalOldestFrequency").Single();
            metaValueTotalOldestFrequency.Question = poll;

            // Caucasian
            QuestionMetaValues metaValueTotalCaucasianFrequency = new QuestionMetaValues();
            metaValueTotalCaucasianFrequency.Value = "0";
            metaValueTotalCaucasianFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalCaucasianFrequency").Single();
            metaValueTotalCaucasianFrequency.Question = poll;

            // African American / Black
            QuestionMetaValues metaValueTotalAfricanAmericanFrequency = new QuestionMetaValues();
            metaValueTotalAfricanAmericanFrequency.Value = "0";
            metaValueTotalAfricanAmericanFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalAfricanAmericanFrequency").Single();
            metaValueTotalAfricanAmericanFrequency.Question = poll;

            // Hispanic
            QuestionMetaValues metaValueTotalHispanicFrequency = new QuestionMetaValues();
            metaValueTotalHispanicFrequency.Value = "0";
            metaValueTotalHispanicFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalHispanicFrequency").Single();
            metaValueTotalHispanicFrequency.Question = poll;

            // Asian
            QuestionMetaValues metaValueTotalAsianFrequency = new QuestionMetaValues();
            metaValueTotalAsianFrequency.Value = "0";
            metaValueTotalAsianFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalAsianFrequency").Single();
            metaValueTotalAsianFrequency.Question = poll;

            // Middle Eastern
            QuestionMetaValues metaValueTotalMiddleEasternFrequency = new QuestionMetaValues();
            metaValueTotalMiddleEasternFrequency.Value = "0";
            metaValueTotalMiddleEasternFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalMiddleEasternFrequency").Single();
            metaValueTotalMiddleEasternFrequency.Question = poll;

            // Pacific Islander
            QuestionMetaValues metaValueTotalPacificIslanderFrequency = new QuestionMetaValues();
            metaValueTotalPacificIslanderFrequency.Value = "0";
            metaValueTotalPacificIslanderFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalPacificIslanderFrequency").Single();
            metaValueTotalPacificIslanderFrequency.Question = poll;

            // Native American/Alaskan
            QuestionMetaValues metaValueTotalNativeAmericanFrequency = new QuestionMetaValues();
            metaValueTotalNativeAmericanFrequency.Value = "0";
            metaValueTotalNativeAmericanFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalNativeAmericanFrequency").Single();
            metaValueTotalNativeAmericanFrequency.Question = poll;

            // Other
            QuestionMetaValues metaValueTotalOtherFrequency = new QuestionMetaValues();
            metaValueTotalOtherFrequency.Value = "0";
            metaValueTotalOtherFrequency.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "TotalOtherFrequency").Single();
            metaValueTotalOtherFrequency.Question = poll;


            string categories = Request["categories"];
            string censorship = Request["censorship"];


            // Category
            QuestionMetaValues metaValueCategory = new QuestionMetaValues();
            metaValueCategory.Value = categories;
            metaValueCategory.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "Categories").Single();
            metaValueCategory.Question = poll;

            // Censorship Level
            QuestionMetaValues metaValueCensorship = new QuestionMetaValues();
            metaValueCensorship.Value = censorship;
            metaValueCensorship.MetaOption = db.QuestionMetaOptions.Where(mo => mo.Option == "Censorship").Single();
            metaValueCensorship.Question = poll;

            


            db.QuestionMetaValues.Add(metaValueTotalFrequency);
            db.QuestionMetaValues.Add(metaValueTotalMaleFrequency);
            db.QuestionMetaValues.Add(metaValueTotalFemaleFrequency);
            db.QuestionMetaValues.Add(metaValueTotalMinorFrequency);
            db.QuestionMetaValues.Add(metaValueTotalYoungFrequency);
            db.QuestionMetaValues.Add(metaValueTotalMiddleAgedFrequency);
            db.QuestionMetaValues.Add(metaValueTotalOlderFrequency);
            db.QuestionMetaValues.Add(metaValueTotalOldestFrequency);
            db.QuestionMetaValues.Add(metaValueTotalCaucasianFrequency);
            db.QuestionMetaValues.Add(metaValueTotalAfricanAmericanFrequency);
            db.QuestionMetaValues.Add(metaValueTotalAsianFrequency);
            db.QuestionMetaValues.Add(metaValueTotalMiddleEasternFrequency);
            db.QuestionMetaValues.Add(metaValueTotalPacificIslanderFrequency);
            db.QuestionMetaValues.Add(metaValueTotalNativeAmericanFrequency);
            db.QuestionMetaValues.Add(metaValueTotalOtherFrequency);
            db.QuestionMetaValues.Add(metaValueTotalHispanicFrequency);
            db.QuestionMetaValues.Add(metaValueCensorship);
            db.QuestionMetaValues.Add(metaValueCategory);
            //

            db.Entry(poll).State = EntityState.Modified;
            // If data is valid, insert it into the database, else return error
            if (ModelState.IsValid && poll != null)
            {
               
                db.SaveChanges();
                XmlQuestion response = new XmlQuestion();
                response.DateCreated = poll.DateCreated;
                response.FlagLevel = poll.FlagLevel;
                response.Id = poll.Id;
                response.QuestionText = poll.QuestionText;
                response.QuestionType = poll.QuestionType;
                response.Rank = poll.Rank;
                return new XmlResult(response);
            }
            else
            {
                return RedirectToAction("error");
            }

        }

        /// <summary>
        /// GET
        /// Returns a specified poll
        /// </summary>
        /// <param name="Id">Poll ID</param>
        /// <returns></returns>
        public ActionResult GetPoll(int Id)
        {
            // If ID is valid
            if (ModelState.IsValid && Id != null)
            {
                //Find and return it
                try
                {
                    if (db.Question.Where(q => q.Id == Id).Count() == 1)
                    {
                        Question poll = db.Question.Where(q => q.Id == Id).Single();
                        XmlQuestion response = new XmlQuestion();
                        response.DateCreated = poll.DateCreated;
                        response.FlagLevel = poll.FlagLevel;
                        response.Id = poll.Id;
                        response.QuestionText = poll.QuestionText;
                        response.QuestionType = poll.QuestionType;
                        response.Rank = poll.Rank;
                        
                        return new XmlResult(response);
                    }
                    else
                        return RedirectToAction("error");
                }
                catch (Exception e)
                {
                    return RedirectToAction("error");
                }
                
            }
            else
            {
                return RedirectToAction("error");
            }
        }

        /// <summary>
        /// GET
        /// Grabs Author information
        /// </summary>
        /// <param name="Id">Author ID</param>
        /// <returns></returns>
        public ActionResult GetAuthorData(int Id)
        {
            // If ID is valid
            if (ModelState.IsValid && Id != null)
            {
                // Grab author and return him
                if (db.Authors.Where(a => a.Id == Id).Count() == 1)
                {
                    // eager load to not break XmlResult
                    Author author = db.Authors.Where(a => a.Id == Id).Single();
                    XmlAuthor returnObject = new XmlAuthor();
                    returnObject.Active = author.Active;
                    returnObject.BirthYear = author.BirthYear;
                    returnObject.Ethnicity = author.Ethnicity;
                    returnObject.FilterLevel = author.FilterLevel;
                    returnObject.Gender = author.Gender;
                    returnObject.Id = author.Id;
                    returnObject.Username = author.Username;
                    return new XmlResult(returnObject);
                }
                else
                    return RedirectToAction("error");
            }
            else
            {
                return RedirectToAction("error");
            }
        }

        /// <summary>
        /// GET
        /// Grabs Authors polls (May need to do this when logging in
        /// </summary>
        /// <param name="Id">Author ID</param>
        /// <returns></returns>
        public ActionResult GetAuthorsPollData(int Id)
        {
            // If ID is valid
            if (ModelState.IsValid && Id != null)
            {
                // Grab author and return him
                List<Question> questions = db.Question.Where(q => q.Author.Id == Id).ToList();
                List<XmlQuestion> response = new List<XmlQuestion>();
                foreach (Question poll in questions)
                {
                    XmlQuestion q = new XmlQuestion();
                    q.DateCreated = poll.DateCreated;
                    q.FlagLevel = poll.FlagLevel;
                    q.Id = poll.Id;
                    q.QuestionText = poll.QuestionText;
                    q.QuestionType = poll.QuestionType;
                    q.Rank = poll.Rank;
                    response.Add(q);
                }
                
                return new XmlResult(response);
            }
            else
            {
                return RedirectToAction("error");
            }
        }

        /// <summary>
        /// POST
        /// Submits responses to a question for either a poll or free response
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitResponse()
        {
            /* NOTE to self: Make sure the following is changed in client lib
             * public static bool submitAnswer(int answerID, Guid SenderID, Guid PollID)
             * {
             * string postData = answerID + "," + SenderID + "," + PollID;
             * string service_url;
             * service_url = service_url_root + "submitAnswer/";
             * 
             * XmlStreamAtt XmlData = postRequest(service_url, "boolean", postData);
             * 
             */


            int questionId = Int32.Parse(Request["questionId"]);

            // check if question is valid
            if (db.Question.Where(q => q.Id == questionId).Count() == 0)
            {
                return new XmlResult("Error: That question does not exist.");
            }
            
            string responseType = Request["responseType"];

            int authorId = Int32.Parse(Request["authorId"]);

            if (db.Authors.Where(a => a.Id == authorId).Count() == 0)
            {
                return new XmlResult("Error: That author does not exist."); // should never happen
            }

            Author submitter = db.Authors.Where(a => a.Id == authorId).Single();
            
            int diff = DateTime.Now.Year - submitter.BirthYear;
            string ageGroup = "";
            if (diff < 18)
            {
                ageGroup = "Minor";
            }
            else if (diff > 17 && diff < 40)
            {
                ageGroup = "Young";
            }
            else if (diff > 39 && diff < 60)
            {
                ageGroup = "MiddleAged";
            }
            else if (diff > 59 && diff < 75)
            {
                ageGroup = "Older";
            }
            else if (diff > 74)
            {
                ageGroup = "Oldest";
            }
            if (ModelState.IsValid)
            {
                if (responseType == "Poll")
                {
                    // handle poll

                    string bugFix = "PollOption" + Int32.Parse(Request["answerId"]);
                    // update option specific statistics

                    QuestionMetaValues freq = db.QuestionMetaValues.Include("Question").Where(qm => db.Question.Where(qs => qs.Id == questionId).FirstOrDefault() == qm.Question && db.QuestionMetaOptions.Where(q => q.Option == bugFix + "Frequency").FirstOrDefault() == qm.MetaOption).Single();
                    //QuestionMetaValues freq = db.QuestionMetaValues.Where(q => q.Id == 523).Single();
                    
                    QuestionMetaValues agefreq = db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == bugFix + ageGroup + "Frequency").FirstOrDefault() == qm.MetaOption ).Single();
                    QuestionMetaValues genderfreq = db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == bugFix + submitter.Gender + "Frequency").FirstOrDefault() == qm.MetaOption).Single();
                    QuestionMetaValues ethfreq = db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == bugFix + submitter.Ethnicity + "Frequency").FirstOrDefault() == qm.MetaOption).Single();

                    Question qe = freq.Question;

                    freq.Value = "" + (Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == bugFix + "Frequency").FirstOrDefault() == qm.MetaOption).Single().Value) + 1);
                    db.Entry(freq).State = EntityState.Modified;
                    genderfreq.Value = ""+(Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == bugFix + submitter.Gender + "Frequency").FirstOrDefault() == qm.MetaOption).Single().Value)+1);
                    db.Entry(genderfreq).State = EntityState.Modified;
                    agefreq.Value = ""+(Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == bugFix + ageGroup + "Frequency").FirstOrDefault() == qm.MetaOption ).Single().Value)+1);
                    db.Entry(agefreq).State = EntityState.Modified;
                    ethfreq.Value = "" + (Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == bugFix + submitter.Ethnicity + "Frequency").FirstOrDefault() == qm.MetaOption).Single().Value) + 1);
                    db.Entry(ethfreq).State = EntityState.Modified;

                    // handle admin poll response
                    PollResponses pollResponse = new PollResponses();
                    pollResponse.Author = db.Authors.Where(a => a.Id == authorId).Single();
                    pollResponse.Question = db.Question.Where(q => q.Id == questionId).Single();
                    pollResponse.AnswerId = Int32.Parse(Request["answerId"]);
                    db.PollResponses.Add(pollResponse);
                    db.SaveChanges();
                }
                else if (responseType == "FreeResponse")
                {
                    // handle free response
                    try
                    {
                        FreeResponses freeResponse = new FreeResponses();
                        freeResponse.Author = db.Authors.Where(a => a.Id == authorId).Single();
                        freeResponse.Question = db.Question.Where(q => q.Id == questionId).Single();
                        Regex regex = new Regex(@"<[^>]*(>|$)");
                        string temp = regex.Replace(Request["freeResponse"], "");
                        regex = new Regex(@"[\s\r\n]+");
                        freeResponse.Response = regex.Replace(temp, " ");
                        db.FreeResponses.Add(freeResponse);

                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return new XmlResult(false);
                    }
                }
                else
                {
                    // error
                    return RedirectToAction("error");
                }


                QuestionMetaValues totFreq = db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId &&  db.QuestionMetaOptions.Where(q => q.Option == "TotalFrequency").FirstOrDefault() == qm.MetaOption).Single();
                QuestionMetaValues totGenderFreq = db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId &&  db.QuestionMetaOptions.Where(q => q.Option == "Total" + submitter.Gender + "Frequency").FirstOrDefault() == qm.MetaOption).Single();
                QuestionMetaValues totAgeGroupFreq = db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId &&  db.QuestionMetaOptions.Where(q => q.Option == "Total" + ageGroup + "Frequency").FirstOrDefault() == qm.MetaOption).Single();
                QuestionMetaValues totEthnicityFreq = db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == "Total" + submitter.Ethnicity + "Frequency").FirstOrDefault() == qm.MetaOption).Single();

                // update total statistics
                totFreq.Value = "" + (Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == "TotalFrequency").FirstOrDefault() == qm.MetaOption).Single().Value) + 1);
                db.Entry(totFreq).State = EntityState.Modified;
                totGenderFreq.Value = "" + (Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == "Total" + submitter.Gender + "Frequency").FirstOrDefault() == qm.MetaOption).Single().Value) + 1);
                db.Entry(totGenderFreq).State = EntityState.Modified;
                totAgeGroupFreq.Value = "" + (Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == "Total" + ageGroup + "Frequency").FirstOrDefault() == qm.MetaOption).Single().Value) + 1);
                db.Entry(totAgeGroupFreq).State = EntityState.Modified;
                totEthnicityFreq.Value = "" + (Int32.Parse(db.QuestionMetaValues.Where(qm => qm.Question.Id == questionId && db.QuestionMetaOptions.Where(q => q.Option == "Total" + submitter.Ethnicity + "Frequency").FirstOrDefault() == qm.MetaOption).Single().Value) + 1);
                db.Entry(totEthnicityFreq).State = EntityState.Modified;

                db.SaveChanges();

                // recalculate rank
                Question question = db.Question.Where(a => a.Id == questionId).Single();
                Ranking.Rank(ref question);
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                
                //db.Entry(db.Answers.Where(a => a.PollId == Answer.PollId).Single()).State = System.Data.EntityState.Modified;
                //db.Entry(db.Question.Where(a => a.PollId == Answer.PollId).Single()).State = System.Data.EntityState.Modified;
                return new XmlResult(true);
            }
            else
            {
                return RedirectToAction("error");
            }
        }

        /// <summary>
        /// Grabs 10(?) random polls from the db under category. If category is empty, gives back a single random result.
        /// </summary>
        /// <param name="Id">The category to grab by</param>
        /// <returns></returns>
        public ActionResult GetRandomQuestionsByCategory(string Id = "")
        {
            // If category is set, grab 10 random questions - might need to be redone if performance is an issue
            if (Id != "")
            {
                List<Question> questions = new List<Question>();
                List<Question> questionsTotal = db.Question.Where(q => q.MetaValues.Where(mv=>mv.Value.Contains(Id)).Count() == 1).ToList();
                // select 10 randpom items
                var rand = new Random();
                int val = 0;
                int valTemp = -1;

                int total = questionsTotal.Count>=10 ? 10 : questionsTotal.Count; 

                for (int i = 0; i < total; i++)
                {
                    
                    val = rand.Next(questionsTotal.Count);
                    if (val == valTemp)
                    {
                        // ensure no duplicates
                        i--;
                        continue;
                    }
                    valTemp = val;
                    Question question = questionsTotal[val];
                    questions.Add(question);
                }

                List<XmlQuestion> response = new List<XmlQuestion>();
                foreach (Question poll in questions)
                {
                    XmlQuestion q = new XmlQuestion();
                    q.DateCreated = poll.DateCreated;
                    q.FlagLevel = poll.FlagLevel;
                    q.Id = poll.Id;
                    q.QuestionText = poll.QuestionText;
                    q.QuestionType = poll.QuestionType;
                    q.Rank = poll.Rank;
                    response.Add(q);
                }

                return new XmlResult(response);
            }
            else // else return a single random question - this can be optimized by sending a whole bunch and letting the 
            {   // client run through a queue before another request.
                List<Question> questionsTotal = db.Question.ToList();
                var rand = new Random();
                Question question = questionsTotal[rand.Next(questionsTotal.Count)];
                XmlQuestion q = new XmlQuestion();
                q.DateCreated = question.DateCreated;
                q.FlagLevel = question.FlagLevel;
                q.Id = question.Id;
                q.QuestionText = question.QuestionText;
                q.QuestionType = question.QuestionType;
                q.Rank = question.Rank;
                
                return new XmlResult(q);
            }
        }

        /// <summary>
        /// Grabs the top ten most answered questions by frequency
        /// </summary>
        public ActionResult GetTopTenQuestions()
        {
            // TODO: check this
            // might take smallest answered - double check when testing
            List<Question> questions = (from t in db.Question orderby (t.MetaValues.Where(mv => mv.MetaOption.Option == "TotalFrequency").FirstOrDefault().Value) descending select t).Take(10).ToList();
            List<XmlQuestion> response = new List<XmlQuestion>();
            foreach (Question poll in questions)
            {
                XmlQuestion q = new XmlQuestion();
                q.DateCreated = poll.DateCreated;
                q.FlagLevel = poll.FlagLevel;
                q.Id = poll.Id;
                q.QuestionText = poll.QuestionText;
                q.QuestionType = poll.QuestionType;
                q.Rank = poll.Rank;
                response.Add(q);
            }
            return new XmlResult(response);
        }

        /// <summary>
        /// Grabs the top ten most answered questions by popularity rank
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMostPopularQuestions()
        {
            List<Question> questions = (from t in db.Question orderby (t.Rank) descending select t).Take(10).ToList();
            List<XmlQuestion> response = new List<XmlQuestion>();
            foreach (Question poll in questions)
            {
                XmlQuestion q = new XmlQuestion();
                q.DateCreated = poll.DateCreated;
                q.FlagLevel = poll.FlagLevel;
                q.Id = poll.Id;
                q.QuestionText = poll.QuestionText;
                q.QuestionType = poll.QuestionType;
                q.Rank = poll.Rank;
                response.Add(q);
            }
            return new XmlResult(response);
        }

        /// <summary>
        /// Balances creation date with answer frequency to retrieve relavent questions for the week
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTopTenQuestionsForWeek()
        {
            DateTime currentDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));
            List<Question> questions = (from t in db.Question orderby (t.Rank) descending select t).Where(y => 1 == (y.DateCreated.CompareTo(currentDate))).Take(10).ToList();
            List<XmlQuestion> response = new List<XmlQuestion>();
            foreach (Question poll in questions)
            {
                XmlQuestion q = new XmlQuestion();
                q.DateCreated = poll.DateCreated;
                q.FlagLevel = poll.FlagLevel;
                q.Id = poll.Id;
                q.QuestionText = poll.QuestionText;
                q.QuestionType = poll.QuestionType;
                q.Rank = poll.Rank;
                response.Add(q);
            }
            return new XmlResult(response);
        }

        /// <summary>
        /// Gets ten random questions considered dull
        /// Current algorithm for dullness: it has been 25 days
        /// Should add frequency to it? rank?
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTenDullQuestions()
        {
            DateTime currentDate = DateTime.Now.Subtract(new TimeSpan(25, 0, 0, 0, 0));
            List<Question> questions = new List<Question>();
            List<Question> questionsTotal = (from t in db.Question orderby (t.Rank) select t).Where(y => -1 == (y.DateCreated.CompareTo(currentDate))).ToList();
            // select 10 randpom items
            int valTemp = -1;
            int val = 0;
            var rand = new Random();
            int total = questionsTotal.Count >= 10 ? 10 : questionsTotal.Count; 
            for (int i = 0; i < total; i++)
            {
                val = rand.Next(questionsTotal.Count);
                if (val == valTemp)
                {
                    // ensure no duplicates
                    i--;
                    continue;
                }
                valTemp = val;
                Question question = questionsTotal[val];
                questions.Add(question);
            }
            List<XmlQuestion> response = new List<XmlQuestion>();
            foreach (Question poll in questions)
            {
                XmlQuestion q = new XmlQuestion();
                q.DateCreated = poll.DateCreated;
                q.FlagLevel = poll.FlagLevel;
                q.Id = poll.Id;
                q.QuestionText = poll.QuestionText;
                q.QuestionType = poll.QuestionType;
                q.Rank = poll.Rank;
                response.Add(q);
            }
            return new XmlResult(response);
        }

        /// <summary>
        /// Get a question's free response
        /// </summary>
        /// <param name="Id">The question Id</param>
        /// <returns></returns>
        public ActionResult GetFreeResponses(int Id)
        {
            // If ID is valid
            if (ModelState.IsValid && Id != null)
            {
                // Grab author and return him
                List<FreeResponses> freeResponses = db.FreeResponses.Where(q => q.Question.Id == Id).ToList();
                List<XmlFreeResponses> response = new List<XmlFreeResponses>();
                foreach (FreeResponses freeResponse in freeResponses)
                {
                    XmlFreeResponses fr = new XmlFreeResponses();
                    fr.Id = freeResponse.Id;
                    fr.Response = freeResponse.Response;
                    fr.AuthorId = freeResponse.Author.Id;
                    response.Add(fr);
                }

                return new XmlResult(response);
            }
            else
            {
                return RedirectToAction("error");
            }
        }


        /// <summary>
        /// Get Question Stat Options
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStatOptions()
        {
            if (ModelState.IsValid)
            {
                // Get Stat Options 
                List<QuestionMetaOptions> questionOptions = db.QuestionMetaOptions.ToList();
                List<XmlQuestionMetaOptions> response = new List<XmlQuestionMetaOptions>();
                foreach (QuestionMetaOptions option in questionOptions)
                {
                    XmlQuestionMetaOptions q = new XmlQuestionMetaOptions();
                    q.Description = option.Description;
                    q.Id = option.Id;
                    q.Option = option.Option;
                    response.Add(q);
                }

                return new XmlResult(response);
            }
            else
            {
                return RedirectToAction("error");
            }
        }

        /// <summary>
        /// Get Question Stats
        /// </summary>
        /// <param name="id">Question Id</param>
        /// <returns></returns>
        public ActionResult GetStatistics(int Id)
        {
            // If ID is valid
            if (ModelState.IsValid && Id != null)
            {
                // Get 
                List<QuestionMetaValues> questionMetaValues = db.QuestionMetaValues.Where(q => q.Question.Id == Id).ToList();
                List<XmlQuestionMetaValues> response = new List<XmlQuestionMetaValues>();
                foreach (QuestionMetaValues pollValue in questionMetaValues)
                {
                    XmlQuestionMetaValues q = new XmlQuestionMetaValues();
                    q.Id = pollValue.Id;
                    q.OptionId = pollValue.MetaOption.Id;
                    q.Value = pollValue.Value;
                    response.Add(q);
                }

                return new XmlResult(response);
            }
            else
            {
                return RedirectToAction("error");
            }
        }





        #endregion

        /*
         * EOF comments:
         * Should we add a search function? Is that a good idea?
         * 
         */
        

    }
}
