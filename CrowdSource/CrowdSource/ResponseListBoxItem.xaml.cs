using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CrowdSource
{
    public partial class ResponseListBoxItem : UserControl
    {

        private Response _response;

        public Response Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
            }
        }

        public ResponseListBoxItem()
        {
            InitializeComponent();
        }

        public ResponseListBoxItem(Response response)
        {
            InitializeComponent();
            Response = response;
            ResponseTextBlock.Text = Response.Text;
        }


    }
}
