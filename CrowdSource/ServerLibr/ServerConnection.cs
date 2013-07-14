/*
 * Server Library
 * Copyright by 
 * Author: Sam Moore
 * Revision: 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ServerLibr;

namespace ServerLibr
{
    /// <summary>
    /// Helper struct to make code cleaner
    /// </summary>
    public struct XmlStreamAtt
    {
        public XmlReader documentReader;
        public XmlRootAttribute xroot;
    }

    /// <summary>
    /// Class that is used to connect to the server.
    /// </summary>
    public class ServerConnection
    {
        public delegate void Del(XmlAuthor author);

        // change to hash later
        //private static string auth_user = "AKazyaka";
        //private static string auth_pass = "heavensgate"; 

        private static string auth_user = "smoore";
        private static string auth_pass = "phyisalis"; 

        // change once we have domain + static IP
        //private static string service_url_root = "http://72.44.101.239/CrowdSpark/";
        private static string service_url_root = @"http://127.0.0.1/Data/";
        //private static string service_url_root = "http://google.com";

        static WebRequest request;
        //static ASCIIEncoding objEncoding = new ASCIIEncoding();
        static WebRequest stream = null;
        static Stream reqStream;

        static string rootElementName = "";

        static Del callBackDelegate;

        /// <summary>
        /// Repeated GET Request code.
        /// </summary>
        /// <param name="service_url"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private static void GetRequest(string service_url, string elementName)
        {
            rootElementName = elementName;
            
            var webRequest = (HttpWebRequest)HttpWebRequest.Create(service_url);
            //webRequest.
            webRequest.Credentials = new NetworkCredential(auth_user, auth_pass);
            webRequest.BeginGetResponse(new AsyncCallback(registerReturn), webRequest);

            //StreamReader webResponseStreamReader = new StreamReader(reqStream);

            
        }

        static void registerReturn(IAsyncResult result)
        {
            var webRequest = result.AsyncState as HttpWebRequest;
            // e.Result is the effective string downloaded
            var response = (HttpWebResponse)webRequest.EndGetResponse(result);
            // convert to xml, then de-serialize into 
            XmlReader docReader = XmlReader.Create(response.GetResponseStream());
            XmlRootAttribute xRoot = new XmlRootAttribute();


            xRoot.ElementName = rootElementName;
            xRoot.IsNullable = true;

            XmlStreamAtt XmlData = new XmlStreamAtt() { documentReader = docReader, xroot = xRoot };

            XmlSerializer deserializer = new XmlSerializer(typeof(XmlAuthor), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            XmlAuthor author = (XmlAuthor)deserializer.Deserialize(XmlData.documentReader);

            // store Author
            callBackDelegate(author);
        }

        /*/// <summary>
        /// Repeated POST request code.
        /// </summary>
        /// <param name="service_url"></param>
        /// <param name="elementName"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static XmlStreamAtt PostRequest(string service_url, string elementName)
        {
            

            //setup request
            stream = (WebRequest)WebRequest.Create(service_url);
            stream.Method = "POST";
            stream.Credentials = new NetworkCredential(auth_user, auth_pass);

            

            //encode and send post data
            //System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(postData.GetType());
            //MemoryStream PostStream = new MemoryStream();
            //x.Serialize(PostStream, postData);
            byte[] postDataBytes = new byte[1000];
            //PostStream.Read(postDataBytes, 0, 1000);

            //stream.ContentType = postDataBytes.Length;
            stream.ContentType = "application/x-www-form-urlencoded";
            reqStream = WebRequestExtensions.GetRequestStream(stream);
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);

            WebResponse response = stream.GetResponse();

            //reqStream = stream.GetResponse().GetResponseStream();
            //
            reqStream = response.GetResponseStream();
           

            StreamReader webResponseStreamReader = new StreamReader(reqStream);

            // convert to xml, then de-serialize into model
            XmlReader docReader = XmlReader.Create(reqStream);
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = elementName;
            xRoot.IsNullable = true;

            XmlStreamAtt XmlData = new XmlStreamAtt() { documentReader = docReader, xroot = xRoot };

            return XmlData;
        }*/


        /// <summary>
        /// Registers a new author via GET request.
        /// </summary>
        /// <returns></returns>
        public static int RegisterAuthor(string username, string birthYear, string gender, string ethnicity, string filterLevel, Del callbackMethod)
        {
            callBackDelegate = callbackMethod;
            
            string service_url;
            //service_url = service_url_root;
            service_url = service_url_root + @"NewAuthor";

            service_url += @"?username="+username+@"&birthYear="+birthYear+@"&gender="+gender+@"&ethnicity="+ethnicity+@"&filterLevel="+filterLevel;

           
            
            try
            {
                GetRequest(service_url, "XmlAuthor");
                
            }
            catch (Exception e)
            {
                return -1;
            }

            return 1;
        }

        /// <summary>
        /// Submits a question via POST request.
        /// </summary>
        /// <param name="question"></param>
        /// <returns>the id of the question</returns>
        public int SubmitQuestion(XmlQuestion question, int authorId, string categories, string censorship, List<String> pollOptions)
        {

            string service_url;
            service_url = service_url_root + "submitQuestion";
            service_url += "?questionText=" + question.QuestionText + "&authorId=" + authorId + "&questionType=" + question.QuestionType + "&numberOfOptions=" + pollOptions.Count + "&categories=" + categories + "&censorship=" + censorship;

            int i=0;
            foreach (string option in pollOptions)
            {
                service_url += "&pollOption" + i + "=" + option.Replace(' ', '+');
                i++;
            }
/*

            XmlStreamAtt XmlData = GetRequest(service_url, "XmlQuestion");
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(XmlQuestion), XmlData.xroot);

                XmlData.documentReader.Read();
                XmlData.documentReader.Read();
                string inn = XmlData.documentReader.ReadOuterXml();
                XmlData.documentReader = XmlReader.Create(new StringReader(inn));

                XmlQuestion success = (XmlQuestion)deserializer.Deserialize(XmlData.documentReader);
                return success.Id;
            }
            catch (Exception e)
            {
                return -1;
            }
 * */
            return 0;
        }

        /// <summary>
        /// Grabs a specific question from the database.
        /// </summary>
        /// <param name="pollID"></param>
        /// <returns></returns>
        /*public static XmlQuestion GetQuestion(int QuestionID)
        {

            string service_url;
            service_url = service_url_root + "GetPoll/"+QuestionID;

            XmlStreamAtt XmlData = GetRequest(service_url_root, "XmlQuestion");

            XmlSerializer deserializer = new XmlSerializer(typeof(XmlQuestion), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            XmlQuestion question = (XmlQuestion)deserializer.Deserialize(XmlData.documentReader);
            return question;
        }*/


        /// <summary>
        /// Grabs author data from the database.
        /// </summary>
        /// <param name="authorID"></param>
        /// <returns></returns>
        /*public static XmlAuthor GetAuthorData(int authorID)
        {
            string service_url;
            service_url = service_url_root + "GetAuthorData/" + authorID;

            XmlStreamAtt XmlData = GetRequest(service_url_root, "XmlAuthor");

            XmlSerializer deserializer = new XmlSerializer(typeof(XmlAuthor), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            XmlAuthor author = (XmlAuthor)deserializer.Deserialize(XmlData.documentReader);
            return author;
        }*/


        /// <summary>
        /// Submits an answer for a specific Question
        /// </summary>
        /// <returns></returns>
        /*public static bool SubmitAnswer(string responseType, int authorId, int questionId, int answerId = -1, string freeResponse = "")
        {

            string service_url;
            service_url = service_url_root + "SubmitResponse";

            service_url += "?questionId=" + questionId + "&authorId=" + authorId + "&responseType=" + responseType+"&answerId="+answerId+"&freeResponse="+freeResponse.Replace(' ', '+');

            XmlStreamAtt XmlData = GetRequest(service_url, "boolean");

            XmlSerializer deserializer = new XmlSerializer(typeof(Boolean), XmlData.xroot);

            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            Boolean success = (Boolean)deserializer.Deserialize(XmlData.documentReader);
            return success;
        }*/


        // get question responses
        /// <summary>
        /// Grabs a specific question from the database.
        /// </summary>
        /// <param name="pollID"></param>
        /// <returns></returns>
        /*public static List<XmlFreeResponses> GetResponses(int QuestionID)
        {

            string service_url;
            service_url = service_url_root + "GetFreeResponses/" + QuestionID;

            XmlStreamAtt XmlData = GetRequest(service_url_root, "ArrayOfXmlFreeResponses");

            XmlSerializer deserializer = new XmlSerializer(typeof(List<XmlFreeResponses>), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            List<XmlFreeResponses> question = (List<XmlFreeResponses>)deserializer.Deserialize(XmlData.documentReader);
            return question;
        }*/

        // get statistics
        /// <summary>
        /// Grabs a questions stat data from the database.
        /// </summary>
        /// <param name="QuestionID"></param>
        /// <returns></returns>
        /*public static List<XmlQuestionMetaValues> GetStatistics(int QuestionID)
        {

            string service_url;
            service_url = service_url_root + "GetStatistics/" + QuestionID;

            XmlStreamAtt XmlData = GetRequest(service_url_root, "ArrayOfXmlMetaValues");

            XmlSerializer deserializer = new XmlSerializer(typeof(List<XmlQuestionMetaValues>), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            List<XmlQuestionMetaValues> stats = (List<XmlQuestionMetaValues>)deserializer.Deserialize(XmlData.documentReader);
            return stats;
        }*/

        // get stat options
        // get statistics
        /// <summary>
        /// Grabs question stat options from the database.
        /// </summary>
        /// <param name="pollID"></param>
        /// <returns></returns>
        /*public static List<XmlQuestionMetaOptions> GetStatOptions()
        {

            string service_url;
            service_url = service_url_root + "GetStatOptions";

            XmlStreamAtt XmlData = GetRequest(service_url_root, "ArrayOfXmlMetaOptions");

            XmlSerializer deserializer = new XmlSerializer(typeof(List<XmlQuestionMetaOptions>), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            List<XmlQuestionMetaOptions> stats = (List<XmlQuestionMetaOptions>)deserializer.Deserialize(XmlData.documentReader);
            return stats;
        }*/

        
        /// <summary>
        /// Get Author's Poll Data
        /// </summary>
        /// <param name="authorId">Id of the author</param>
        /// <returns></returns>
        /*public static List<XmlQuestion> GetAuthorsPollData(int authorId)
        {

            string service_url;
            service_url = service_url_root + "GetAuthorsPollData/"+authorId;

            XmlStreamAtt XmlData = GetRequest(service_url_root, "ArrayOfXmlQuestion");

            XmlSerializer deserializer = new XmlSerializer(typeof(List<XmlQuestion>), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            List<XmlQuestion> stats = (List<XmlQuestion>)deserializer.Deserialize(XmlData.documentReader);
            return stats;
        }*/

    }
}
