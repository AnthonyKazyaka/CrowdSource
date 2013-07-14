using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CrowdSource
{
    public class User
    {

        public int userID = new int();
        
        public Question.CensorshipLevel accessLevel = Question.CensorshipLevel.Adults;

        public enum Gender
        {
            Male,
            Female,
        }

        public Gender gender;

        public string username;

        public int age;

        public string ethnicity;


        public User()
        {
            accessLevel = Question.CensorshipLevel.Adults;

        }

        public User(Question.CensorshipLevel access, Gender sex)
        {
            accessLevel = access;
            gender = sex;
        }

        public User(int id, Question.CensorshipLevel access, Gender sex, string usn, int _age, string ethnic)
        {
            userID = id;
            accessLevel = access;
            gender = sex;
            username = usn;
            age = _age;
            ethnicity = ethnic;
        }
    }
}
