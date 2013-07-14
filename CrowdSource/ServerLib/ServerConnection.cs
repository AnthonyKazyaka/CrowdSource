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

namespace ServerLib
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
    class ServerConnection
    {

        // change to hash later
        private static string auth_user = "AKazyaka";
        private static string auth_pass = "heavensgate";

        // change once we have domain + static IP
        private static string service_url_root = "http://68.61.207.207/CrowdSpark/";

        static WebRequest request = null;
        static ASCIIEncoding objEncoding = new ASCIIEncoding();
        static HttpWebRequest stream = null;
        static Stream reqStream;

        /// <summary>
        /// Repeated GET Request code.
        /// </summary>
        /// <param name="service_url"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        private static XmlStreamAtt getRequest(string service_url, string elementName)
        {
            //setup request
            stream = (HttpWebRequest)WebRequest.Create(service_url);

            stream.Credentials = new NetworkCredential(auth_user, auth_pass);

            reqStream = stream.GetResponse().GetResponseStream();

            StreamReader webResponseStreamReader = new StreamReader(reqStream);

            // convert to xml, then de-serialize into model
            XmlReader docReader = XmlReader.Create(reqStream);
            XmlRootAttribute xRoot = new XmlRootAttribute();


            xRoot.ElementName = elementName;
            xRoot.IsNullable = true;

            XmlStreamAtt XmlData = new XmlStreamAtt() { documentReader = docReader, xroot = xRoot };
            
            return XmlData;
        }

        /// <summary>
        /// Repeated POST request code.
        /// </summary>
        /// <param name="service_url"></param>
        /// <param name="elementName"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private static XmlStreamAtt postRequest(string service_url, string elementName, object postData)
        {
            //setup request
            stream = (HttpWebRequest)WebRequest.Create(service_url);
            request.Method = "POST";
            stream.Credentials = new NetworkCredential(auth_user, auth_pass);

            //encode and send post data
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(postData.GetType());
            MemoryStream PostStream = new MemoryStream();
            x.Serialize(PostStream, postData);
            byte[] postDataBytes = new byte[1000];
            PostStream.Read(postDataBytes, 0, 1000);

            request.ContentLength = postDataBytes.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            reqStream = request.GetRequestStream();
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);

            reqStream = stream.GetResponse().GetResponseStream();

            StreamReader webResponseStreamReader = new StreamReader(reqStream);

            // convert to xml, then de-serialize into model
            XmlReader docReader = XmlReader.Create(reqStream);
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = elementName;
            xRoot.IsNullable = true;

            XmlStreamAtt XmlData = new XmlStreamAtt() { documentReader = docReader, xroot = xRoot };

            return XmlData;
        }


        /// <summary>
        /// Registers a new author via GET request.
        /// </summary>
        /// <returns></returns>
        public static Guid registerAuthor()
        {

            string service_url;
            service_url = service_url_root + "newAuthor/";

            XmlStreamAtt XmlData = getRequest(service_url_root, "author");

            XmlSerializer deserializer = new XmlSerializer(typeof(Author), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            Author author  = (Author)deserializer.Deserialize(XmlData.documentReader);
            return author.AuthorId;
        }

        /// <summary>
        /// Submits a question via POST request.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static bool submitQuestion(Poll question)
        {

            string service_url;
            service_url = service_url_root + "submitQuestion/";


            XmlStreamAtt XmlData = postRequest(service_url, "boolean", question);

            XmlSerializer deserializer = new XmlSerializer(typeof(Boolean), XmlData.xroot);

            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            Boolean success = (Boolean)deserializer.Deserialize(XmlData.documentReader);
            return success;
        }

        /// <summary>
        /// Grabs a specific question from the database.
        /// </summary>
        /// <param name="pollID"></param>
        /// <returns></returns>
        public static Poll getQuestion(Guid QuestionID)
        {

            string service_url;
            service_url = service_url_root + "getPoll/"+QuestionID;

            XmlStreamAtt XmlData = getRequest(service_url_root, "poll");

            XmlSerializer deserializer = new XmlSerializer(typeof(Poll), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            Poll question = (Poll)deserializer.Deserialize(XmlData.documentReader);
            return question;
        }


        /// <summary>
        /// Grabs author data from the database.
        /// </summary>
        /// <param name="authorID"></param>
        /// <returns></returns>
        public static Author getAuthorData(Guid authorID)
        {
            string service_url;
            service_url = service_url_root + "getAuthorData/" + authorID;

            XmlStreamAtt XmlData = getRequest(service_url_root, "author");

            XmlSerializer deserializer = new XmlSerializer(typeof(Author), XmlData.xroot);
            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            Author author = (Author)deserializer.Deserialize(XmlData.documentReader);
            return author;
        }


        /// <summary>
        /// Submits an answer for a specific Question
        /// </summary>
        /// <returns></returns>
        public static bool submitAnswer(Answers answer, Guid AuthorId)
        {

            string service_url;
            service_url = service_url_root + "submitAnswer/";


            XmlStreamAtt XmlData = postRequest(service_url, "boolean", answer);

            XmlSerializer deserializer = new XmlSerializer(typeof(Boolean), XmlData.xroot);

            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            Boolean success = (Boolean)deserializer.Deserialize(XmlData.documentReader);
            return success;
        }


        /// <summary>
        /// Submits a free-answer comment for a question
        /// </summary>
        /// <returns></returns>
        public static bool submitComment(Comment comment)
        {

            string service_url;
            service_url = service_url_root + "submitComment/";


            XmlStreamAtt XmlData = postRequest(service_url, "boolean", comment);

            XmlSerializer deserializer = new XmlSerializer(typeof(Boolean), XmlData.xroot);

            XmlData.documentReader.Read();
            XmlData.documentReader.Read();
            string inn = XmlData.documentReader.ReadOuterXml();
            XmlData.documentReader = XmlReader.Create(new StringReader(inn));

            Boolean success = (Boolean)deserializer.Deserialize(XmlData.documentReader);
            return success;
        }
    }
}
