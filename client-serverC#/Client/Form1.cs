﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
	public partial class Client_form : Form
	{
		public Client_form()
		{
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
			Connect();
		
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			Send();
			AddMessage(txbMessage.Text);
		}

		IPEndPoint IP;
		Socket client;
		void Connect()
		{
			IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
			client = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.IP);
			

			try {
				client.Connect(IP);
			} catch {
				MessageBox.Show("Khong the ket noi server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}


			Thread listen = new Thread(Receive);
			listen.IsBackground = true;
			listen.Start();
		}

		void Close()
		{
			client.Close();
		}

		void Send()
		{
			if(txbMessage.Text != string.Empty)
			client.Send(Serialize(txbMessage.Text));
		}

		void Receive()
		{
			try
			{
				while (true)
				{
					byte[] data = new byte[1024 * 5000];
					client.Receive(data);
					string message = (string)Deseriallize(data);

					AddMessage(message);
				}
			}
			catch
			{
				Close();
			}
			
			
		}

		void AddMessage(string s)
		{
			lsvMessage.Items.Add(new ListViewItem(){ Text = s});
			txbMessage.Clear();

		}
		//Phân mảnh
		byte[] Serialize(object obj)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream,obj);

			return stream.ToArray();
		}
		//Gom mảnh
		object Deseriallize(byte[] data)
		{
			MemoryStream stream = new MemoryStream(data);
			BinaryFormatter formatter = new BinaryFormatter();

			return formatter.Deserialize(stream);

			
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}


		
		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			
		}
		//Đóng kết nối khi đóng form
		private void Client_form_FormClosed(object sender, FormClosedEventArgs e)
		{
			Close();
		}

        private void lsvMessage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
