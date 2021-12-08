using Haley.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace UdpChat
{
     
    public class ViewModelUdpData : INotifyPropertyChanged
    {
      
        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }
        private string chat;
        public string Chat
        {
            get => chat;
            set
            {
                chat = value;
                OnPropertyChanged();
            }
        }
        private string nick;
        public string Nick
        {
            get => nick;
            set
            {
                nick = value;
                OnPropertyChanged();
            }
        }
        private int localPort;
        public int LocalPort
        {
            get => localPort;
            set
            {
                localPort = value;
                OnPropertyChanged();
            }
        }
        private int remoPort;
        public int RemoPort
        {
            get => remoPort;
            set
            {
                remoPort = value;
                OnPropertyChanged();
            }
        }
        private IPAddress ip;
        public IPAddress IP
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged();
            }
        }


        TcpClient client;

        StreamReader sr;
        StreamWriter sw;

        private DelegateCommand clearCommand;

        public DelegateCommand ClearCommand
        {
            get
            {
                return clearCommand ?? (clearCommand = new DelegateCommand(obj =>
                { 
                    Chat = "";
                }));
            }
        }

        public ViewModelUdpData()
        {
            IP = (IPAddress)IPAddress.Parse("127.0.0.1");
            LocalPort = 0;
            Nick = "Denis";
            RemoPort = 0;
            Chat += Message;
            Task.Run(() => ReciveMessageUdpProtocol());
            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            if (client?.Connected == true)
            //            {
            //                var line = sr.ReadToEnd();
            //                if (line != null)
            //                {
            //                    Chat += line + "\n";
            //                }
            //                else
            //                {
            //                    client.Close();
            //                    Chat += "ConnctedError" + "\n";
            //                }
            //            }
            //            Thread.Sleep(100);
            //        }
            //        catch (Exception ex) { MessageBox.Show(ex.Message); }
            //    }
            //});
        }



        //private DelegateCommand connectCommand;

        public DelegateCommand ConnectCommand
        {
            get
            {
                //return connectCommand ?? (connectCommand = new DelegateCommand(obj =>
                //{
                //    client = new TcpClient();
                //    client.Connect(udp.IP, udp.Port);
                //    sr = new StreamReader(client.GetStream());
                //    sw = new StreamWriter(client.GetStream());
                //    sw.AutoFlush = true;

                //    sw.WriteLine($"Login: {udp.Nick}");
                //}));, () => client?.Connected == false
                return new DelegateCommand(obj =>
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            client = new TcpClient();
                            client.Connect(IP, LocalPort);
                            sr = new StreamReader(client.GetStream());
                            sw = new StreamWriter(client.GetStream());
                            sw.AutoFlush = true; // автоотправка стриму 
                            sw.WriteLine($"Login: {Nick}"); // сразу принимает строку с авторизированием
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    });
                }, (obj) => client == null || client?.Connected == false);
            }
        }

        //private DelegateCommand sendCommand;

        public DelegateCommand SendCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            sw.WriteLine($"{Nick}: {Message}");
                            Message = "";
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    });
                });
                //return sendCommand ?? (sendCommand = new DelegateCommand(obj =>
                //{
                //    Task.Run(() =>
                //    {
                //        try
                //        {
                //            sw.WriteLine($"{Nick}: {Message}");
                //            Message = "";
                //        }catch(Exception ex){ MessageBox.Show(ex.Message); }
                //    });
                //}));
            }
        }
        
        //private DelegateCommand reciveMessageUdpProtocolCommand;
        public void ReciveMessageUdpProtocol()
        {
            while (true)
            {
                UdpClient client = new UdpClient(LocalPort);
                IPEndPoint ep = null;
                byte[] bytes = client.Receive(ref ep);
                string message;
                message = Nick + ": " + Encoding.Default.GetString(bytes) + "\n";
                Chat = message;
                client.Close();
            }
        }
        //public DelegateCommand ReciveMessageUdpProtocolCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(obj =>
        //        {
        //            UdpClient client = new UdpClient(LocalPort);
        //            IPEndPoint ep = null;
        //            byte[] bytes = client.Receive(ref ep);
        //            Chat = Nick + ": " + Encoding.Default.GetString(bytes);
        //            client.Close();
        //        });
        //    }
        //}
        // UDP realization
        //public DelegateCommand SendMessageUdpProtocolCommand
        //{
        //    get
        //    {
        //        return new DelegateCommand(obj =>
        //        {
        //            Task.Factory.StartNew(() =>
        //            {
        //                try
        //            {
        //                    UdpClient client = new UdpClient();
        //                    int ip = Convert.ToInt32(IP);
        //                    IPEndPoint ep = new IPEndPoint(ip, RemoPort);
        //                    byte[] bytes = Encoding.Default.GetBytes(Message);
        //                    client.Send(bytes, bytes.Length, ep);
        //                    client.Close();
        //                    Message = "";
        //                }
        //                catch { }
        //            });
        //        }, (obj) => Message != null);
        //        //return sendMessageUdpProtocolCommand ?? (sendMessageUdpProtocolCommand = new DelegateCommand(obj =>
        //        //{
        //        //    UdpClient client = new UdpClient();
        //        //    int ip = Convert.ToInt32(IP);
        //        //    IPEndPoint ep = new IPEndPoint(ip, RemoPort);
        //        //    byte[] bytes = Encoding.Default.GetBytes(Message);
        //        //    client.Send(bytes, bytes.Length, ep);
        //        //    client.Close();
        //        //}));
        //    }
        //}

        private DelegateCommand sendMessageUdpProtocolCommand;
        public DelegateCommand SendMessageUdpProtocolCommand
        {
            get
            {
                return sendMessageUdpProtocolCommand ?? (sendMessageUdpProtocolCommand = new DelegateCommand(obj =>
                 {
                     Task.Factory.StartNew(() =>
                     {
                         UdpClient client = new UdpClient();
                         IPEndPoint ep = new IPEndPoint(IP, RemoPort);
                         byte[] bytes = Encoding.Default.GetBytes(Message);
                         client.Send(bytes, bytes.Length, ep);
                         client.Close();
                         Chat += $"{Nick} {Message}\n";
                         Message = "";
                         
                     });
                 }, (obj) => Message != null));
            }
        }

        public DelegateCommand ConnectProtocolUdpCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    try
                    {
                        Chat = Nick + " connected.\n";
                        Task.Run(() =>
                        {
                            Message = "You can starte)";
                            Thread.Sleep(2000);
                            Chat = "";
                            ReciveMessageUdpProtocol();
                        });
                    }
                    catch { }
                }, (obj) => Message != "");
            }
        }

        private bool darkTheme;
        public bool DarkTheme
        {
            get => darkTheme;
            set
            {
                darkTheme = value;
                OnThemeChanged();
            }
        }

        private Brush buttonColor;

        public Brush ButtonColor
        {
            get => buttonColor;
            set
            {
                buttonColor = value;
                OnThemeChanged();
            }
        }
        
        private void OnThemeChanged() // этот вариант не работает 
        {
           
            /*
            Task.Run(() =>
            {
                if(DarkTheme)
                {
                    ButtonColor = Brushes.Black;
                }
                else
                {
                    ButtonColor = Brushes.AliceBlue;
                }
            });
            */
        }

        private DelegateCommand darkThemeCommand;
        public DelegateCommand DarkThemeCommand
        {
            get
            {
                return darkThemeCommand ?? (darkThemeCommand = new DelegateCommand(obj =>
                {
                    DarkTheme = true;
                }));
            }
        }

        private DelegateCommand lightThemeCommand;

        public DelegateCommand LightThemeCommand
        {
            get
            {
                return lightThemeCommand ?? (lightThemeCommand = new DelegateCommand(obj =>
                {
                    DarkTheme = false;
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
