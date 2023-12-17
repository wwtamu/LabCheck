using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabCheck
{
    public class Computer : INotifyPropertyChanged
    {
        private string name;
        private string image;
        private bool availability;
        private string status;
        private string use;

        private string ip;
        private string mac;

        public Computer()
        {
            this.name = "";
            this.image = "";
            this.availability = true;
            this.status = "";
            this.use = "";
            this.ip = "";
            this.mac = "";
        }

        public Computer(string n, string i, bool a, string s, string u, string ip, string mac)
        {
            this.name = n;
            this.image = i;
            this.availability = a;
            this.status = s;
            this.use = u;
            this.ip = ip;
            this.mac = mac;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Image
        {
            get { return this.image; }
            set
            {
                if (value != this.image)
                {

                    this.image = value;
                    NotifyPropertyChanged("Image");
                }
            }
        }

        public bool Availability
        {
            get { return this.availability; }
            set
            {
                if (value != this.availability)
                {

                    this.availability = value;
                    NotifyPropertyChanged("Availability");
                }
            }
        }

        public string Status
        {
            get { return this.status; }
            set
            {
                if (value != this.status)
                {

                    this.status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public string Use
        {
            get { return this.use; }
            set
            {
                if (value != this.use)
                {

                    this.use = value;
                    NotifyPropertyChanged("Use");
                }
            }
        }

        public string IP
        {
            get { return this.ip; }
            set
            {
                if (value != this.ip)
                {

                    this.ip = value;
                    NotifyPropertyChanged("IP");
                }
            }
        }

        public string Mac
        {
            get { return this.mac; }
            set
            {
                if (value != this.mac)
                {

                    this.mac = value;
                    NotifyPropertyChanged("Mac");
                }
            }
        }
                
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
