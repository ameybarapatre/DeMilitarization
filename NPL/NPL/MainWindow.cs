using System;
using NPL;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using Gtk;


public partial class MainWindow: Gtk.Window
{	 public static int uid;
	public static MySQLCon query ;
	public static int session = 0 ;

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {   query.remove ("DELETE FROM `npl_ip` WHERE `uid` = '" + uid + "'");
        textview1.Buffer.Text="Disconnected \n";
        session=0;
        
        ProcessExecute s = new ProcessExecute(this, "netsh advfirewall firewall delete rule name='HUMM' ");
        ProcessExecute j = new ProcessExecute(this, "netsh advfirewall firewall delete rule name='HUMM1' ");

		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnButton3Clicked (object sender, EventArgs e)
	{ query  = new MySQLCon ("SELECT * FROM `npl_user` WHERE `username` = '"+entry3.Text+"' AND `password`= password('"+entry4.Text+"')",this);
		query.authenticate();


	}
	public void SQLResults()
	{ if (session != 1) {
			if (uid > 0) {

				button3.Label = "Disconnect";
				Console.WriteLine (uid + " is Here");
				textview1.Buffer.Text += "Connected \n";
				session = 1;
				query.GetIP();
				query.remove ("INSERT INTO  `npl_ip` VALUES ('"+GetLocalIPAddress()+"','" + uid + "')");
				  //ProcessExecute s = new ProcessExecute(this,"netsh advfirewall firewall add rule name='HUMM1' dir=in action=block protocol=icmpv4");
                Thread k = new Thread(new ThreadStart(SSLSender.sendmessage));
                k.IsBackground=true;
                Thread m = new Thread(new ThreadStart(listenit));
                m.IsBackground=true;
                k.Start();
                m.Start();

			}
		}
		else {

			button3.Label = "Connect";
			query.remove ("DELETE FROM `npl_ip` WHERE `uid` = '" + uid + "'");
			textview1.Buffer.Text="Disconnected \n";
			session=0;
          
			ProcessExecute s = new ProcessExecute(this, "netsh advfirewall firewall delete rule name='HUMM' ");
            ProcessExecute j = new ProcessExecute(this, "netsh advfirewall firewall delete rule name='HUMM1' ");
           
			//disconnect
			// textview1.Buffer.Text += "Disconnected \n";
		}
	}
	 public void ConnectedIP()
    {       textview1.Buffer.Text = "Connected\n";
		foreach (String i in MySQLCon.ips) {
			textview1.Buffer.Text += i+ " is using DeMilitarize\n";
		    ProcessExecute s = new ProcessExecute(this, "netsh advfirewall firewall add rule name='HUMM' dir=in action=allow remoteip="+i+" protocol=tcp");
		
		}
		
	}
	public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
		return "You are Not Connected ";
    }

	public void Disconnect(){
        Console.WriteLine("okay");
       // textview1.Buffer.Text += ProcessExecute.g+ " \n";
	}
    public  void listenit(){
        try { UdpClient udpServer = new UdpClient(11000);
            
            while (true)
            {  
                var remoteEP = new IPEndPoint(IPAddress.Any, 11000); 
                var data = udpServer.Receive(ref remoteEP);
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(data));// listen on port 11000
                Console.Write("receive data from " + remoteEP.ToString());
                ProcessExecute s = new ProcessExecute(this, "netsh advfirewall firewall add rule name='HUMM' dir=in action=allow remoteip="+System.Text.Encoding.UTF8.GetString(data)+" protocol=icmpv4");
                MySQLCon.ips.Add(System.Text.Encoding.UTF8.GetString(data));
                Gtk.Application.Invoke(delegate {
                    this.ConnectedIP();
                });
                udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
                
            }
            
        }
        catch(Exception e){
            Thread.CurrentThread.Abort();
        } 
    }
}
