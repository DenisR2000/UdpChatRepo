using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UdpChat
{
    public class UdpData : INotifyPropertyChanged
    {
        private string ip;

        private int port;

        private string nick;
        private string chat;
        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyhanged();
            }
        }
        public string Chat
        {
            get => chat;
            set
            {
                chat = value;
                OnPropertyhanged();
            }
        }
        public string Nick
        {
            get => nick;
            set
            {
                nick = value;
                OnPropertyhanged();
            }
        }
        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyhanged();
            }
        }
        public string IP
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyhanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyhanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
