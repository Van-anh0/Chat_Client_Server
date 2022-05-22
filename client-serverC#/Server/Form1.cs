using System;
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

namespace Server
{
	public partial class ServerForm : Form
	{
		public ServerForm()
		{
			InitializeComponent();
			Connect();
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			foreach(Socket item in clientList)
			{
		
					Send(item);
					
			}
			lsvMessage.Items.Add("Server: " + txbMessage.Text);
			txbMessage.Clear();
		}

		private void ServerForm_Load(object sender, EventArgs e)
		{

		}

		private void ServerForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Close();
		}

		IPEndPoint IP;
		Socket server;
		List<Socket> clientList;
		void Connect()
		{
			clientList = new List<Socket>();
			IP = new IPEndPoint(IPAddress.Any, 9999);
			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

			server.Bind(IP);

			Thread Listen = new Thread(()=> {
				try
				{
					while (true)
					{
						server.Listen(100);
						Socket client = server.Accept();

						clientList.Add(client);

						Thread receive = new Thread(Receive);
						receive.IsBackground = true;
						receive.Start(client);
					}
				}
				catch
				{
					IP = new IPEndPoint(IPAddress.Any, 9999);
					server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
				}
			});
			Listen.IsBackground = true;
			Listen.Start();
		}

		void Close()
		{
			server.Close();
		}

		void Send(Socket client)
		{

			if (client != null && txbMessage.Text != string.Empty)
				client.Send(Serialize(txbMessage.Text));
		}

		//nhận tin
		void Receive(Object obj)
		{
			Socket client = obj as Socket;
			try
			{
				while (true)
				{
					byte[] data = new byte[1024 * 5000];
					client.Receive(data);
					string message = (string)Deseriallize(data);

                    foreach (Socket item in clientList)
                    {
						if(item != null && item != client) //không gửi trả lại tin nhắn cho thằng client gửi tin
							item.Send(Serialize(message));

                    }
					AddMessage(message);
					//sau khi nhận xong, muốn các client khác nhận được thì truyền cái tin nhắn nhận được cho các bé client khác
				}
			}
			catch
			{
				clientList.Remove(client);
				client.Close();
			}


		}

		void AddMessage(string s)
		{
			lsvMessage.Items.Add(new ListViewItem() { Text = s});
		}
		//Phân mảnh
		byte[] Serialize(object obj)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, obj);

			return stream.ToArray();
		}
		//Gom mảnh
		object Deseriallize(byte[] data)
		{
			MemoryStream stream = new MemoryStream(data);
			BinaryFormatter formatter = new BinaryFormatter();

			return formatter.Deserialize(stream);


		}
	}
}
