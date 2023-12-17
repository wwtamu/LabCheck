using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LabCheck
{
    class Program
    {
        public class Inside
        {
            public ObservableCollection<Lab> labs = new ObservableCollection<Lab>();

            public event PropertyChangedEventHandler PropertyChanged;


            public ObservableCollection<Lab> Labs
            {
                get { return labs; }
                set
                {
                    if (value != labs)
                    {
                        labs = value;
                        NotifyPropertyChanged("Labs");
                    }
                }
            }

            public void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public string ConvertTime(String military_time)
            {
                string[] time_split = military_time.Split(new char[] { ':' }, StringSplitOptions.None);

                if (Convert.ToInt32(time_split[0]) < 12)
                {
                    return Convert.ToInt32(time_split[0]) + ":" + time_split[1] + "am";
                }
                else
                {
                    return (Convert.ToInt32(time_split[0]) - 12) + ":" + time_split[1] + "pm";
                }
            }
        }

        private static MachineAvailabilityServiceReference.IService1 AvailabilityService;
        private static bool retrieving;

        static void Main(string[] args)
        {
            AvailabilityService = new MachineAvailabilityServiceReference.Service1Client();

            Inside inside = new Inside();
            
            Init(inside);
            retrieving = true;

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Ping pingSender = new Ping();


            while (retrieving) { System.Threading.Thread.Sleep(500); }

            foreach (Lab l in inside.labs)
            {
                Console.WriteLine(l.Name);
                foreach (Computer c in l.Computers)
                {
                    Console.WriteLine(c.Name + " " + c.IP);
                    /*
                    IPAddress address = IPAddress.Parse(c.IP);
                    PingReply reply = pingSender.Send(address);

                    if (reply.Status == IPStatus.Success)
                    {
                        Console.WriteLine("Address: {0}", reply.Address.ToString());
                        Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                        Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                        Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                        Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                    }
                    else
                    {
                        Console.WriteLine(reply.Status);
                    }
                    */ 
                }
                System.Threading.Thread.Sleep(10000);
            }
            Console.WriteLine("DONE");
            System.Threading.Thread.Sleep(10000);
        }

        public static async void Init(Inside inside)
        {
            await GetLabs(inside);
            retrieving = false;
        }

        public static async Task<bool> GetLabs(Inside inside)
        {
            bool exception_found = false;
            try
            {
                var labs = await AvailabilityService.GetAllLabsAsync();
                Console.WriteLine(labs.Count() + " labs");
                foreach (MachineAvailabilityServiceReference.LabObject l in labs)
                {
                    Lab newLab = new Lab();
                    newLab.Name = l.LabName;
                    LabInfo newLabInfo = new LabInfo(l.LabName, l.LabBuilding, l.LabFloor, l.LabRoom, l.LabMachineCount, 0, inside.ConvertTime(l.LabOpen), inside.ConvertTime(l.LabClose), "Unknown"); //l.LabStatus

                    try
                    {
                        var computers = await AvailabilityService.GetComputersInLabAsync(l.LabName);
                        var availability = await AvailabilityService.GetComputersAvailabilityAsync(l.LabName);

                        Console.WriteLine(computers.Count() + " computers in " + l.LabName);

                        int i = 0;
                        int a = 0;
                        foreach (MachineAvailabilityServiceReference.MachineObject m in computers)
                        {
                            if (availability.ElementAt(i) == true)
                            {
                                newLab.Computers.Add(new Computer(m.MachineName, Constants.available_image, availability.ElementAt(i), "ON", "Available", m.MachineIP, m.MachineMac));
                                a++;
                            }
                            else
                            {
                                newLab.Computers.Add(new Computer(m.MachineName, Constants.inuse_image, availability.ElementAt(i), "ON", "InUse", m.MachineIP, m.MachineMac));
                            }
                            i++;
                        }
                        newLabInfo.Available = a;
                        newLab.Info.Add(newLabInfo);
                        inside.labs.Add(newLab);
                    }
                    catch (Exception)
                    {
                        exception_found = true;
                    }

                    if (exception_found)
                    {
                        Console.WriteLine("ERROR");
                    }
                }
            }
            catch (Exception)
            {
                exception_found = true;
            }

            if (exception_found) // allows awaitable method before exit
            {
                Console.WriteLine("ERROR");
            }
            return !exception_found;
        }
    }
}
