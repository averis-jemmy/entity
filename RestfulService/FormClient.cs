using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyAverisClientTest
{
    public partial class FormClient : Form
    {
        string strResult = string.Empty;
        string requestType = string.Empty;
        string requestData = string.Empty;
        List<KeyValuePair<string, string>> headers = null;

        public FormClient()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            txtResponse.Text = string.Empty;
            txtResponse.Enabled = false;
            txtRequest.Enabled = false;
            btnGo.Enabled = false;

            headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("UserID", "5FD55244-F824-4531-AAB7-20860AE202ED"));
            headers.Add(new KeyValuePair<string, string>("TokenID", "82FF6E5B-7D57-447E-90C0-9B4BF446C504"));

            //headers.Add(new KeyValuePair<string, string>("UserID", "63D485C8-2C3D-4CE9-A42D-801274A74655"));
            //headers.Add(new KeyValuePair<string, string>("TokenID", "36AC2824-D109-4283-85D0-9B49A07325B5"));

            string[] request = txtRequest.Text.Split(';');
            if (request.Length > 1)
            {
                requestType = request[0];
                requestData = request[1];

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
            }
            else
            {
                txtResponse.Enabled = true;
                txtRequest.Enabled = true;
                btnGo.Enabled = true;
                txtResponse.Text = strResult;
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //RestClient client = new RestClient("https://172.25.67.87/MyAverisServiceHttps/AverisMobile.svc/", HttpVerb.POST, ContentTypeString.JSON, requestData);
            //RestClient client = new RestClient("http://localhost:52860/AverisMobile.svc/", HttpVerb.POST, ContentTypeString.JSON, requestData);
            //strResult = client.ProcessRequest(requestType, headers);
            strResult = HttpGet.HttpSubmit("+601137047578", "test");
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtResponse.Enabled = true;
            txtRequest.Enabled = true;
            btnGo.Enabled = true;
            txtResponse.Text = strResult;
        }
    }
}
