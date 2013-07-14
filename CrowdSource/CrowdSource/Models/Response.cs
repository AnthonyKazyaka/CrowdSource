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
    public class Response
    {
        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        User user = new User();

        public User.Gender UserGender
        {
            get { return user.gender; }
        }

        public Response()
        {
        }

        public Response(string responseText, User _u)
        {
            Text = responseText;
            user = _u;
        }
    }
}
