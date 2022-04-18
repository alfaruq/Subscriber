using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Subscriber
{
    public partial class Form1 : Form
    {
        private AmazonSQSClient _objClient;
        private DateTime _timeTrigger;

        public Form1()
        {
            InitializeComponent();

            _objClient = new AmazonSQSClient();
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now >= _timeTrigger)
            {
                PullMessages();

                btnPull.Enabled = true;
                timer1.Stop();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region Set Timer

            var temp = DateTime.Now.AddSeconds(30);
            txtTriggerTime.Text = temp.ToString("HH:mm:ss");

            #endregion
        }

        private void btnPull_Click(object sender, EventArgs e)
        {
            try
            {
                btnPull.Enabled = false;

                var dailyTime = txtTriggerTime.Text.Trim();
                var timeParts = dailyTime.Split(new char[] { ':' });
                var dateNow = DateTime.Now;
                _timeTrigger = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
                    int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));

                timer1.Start();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PullMessages()
        {
            try
            {
                var URL = txtQueueURL.Text.Trim();
                ReceiveMessageResponse response = new ReceiveMessageResponse();

                response = _objClient.ReceiveMessage(new ReceiveMessageRequest()
                {
                    QueueUrl = URL,
                    MaxNumberOfMessages = int.Parse(txtMaxMessages.Text.Trim())
                });

                foreach(var message in response.Messages)
                {
                    txtResult.Text += String.Format("{0}" + System.Environment.NewLine, message.Body);
                    _objClient.DeleteMessage(URL, message.ReceiptHandle);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
