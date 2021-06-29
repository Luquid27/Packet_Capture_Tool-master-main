using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpPcap;
using PacketDotNet;
using Packet_Capture_Tool;
using System.Net;
using SnmpSharpNet;

namespace Packet_Capture_Tool
{
    public partial class Form1 : Form
    {
        List<string> captureDeviceList;
        List<string> protocolsList;
        int deviceIndex;
        bool isPromisc;
        CaptureDeviceList devices;
        int typeOfDecode = 0;
        string writeLine;
        bool stopCapture = false;
        bool decodeMode;

        List<PackageDetail> packageDetailList;

        public Form1()
        {
            InitializeComponent();
            FillProtocolsList();
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
            button3.Click += new EventHandler(button3_Click);
            button4.Click += new EventHandler(button4_Click);
            string displayText = get_Device_List();
            textBox1.Text = displayText;
            comboBox1.DataSource = captureDeviceList;
            comboBox2.DataSource = protocolsList;
            button3.Enabled = false;
            button4.Enabled = false;
            packageDetailList = new List<PackageDetail>();
            new PackageDetail();
        }

        private void FillProtocolsList()
        {
            protocolsList = new List<string>();
            protocolsList.Add("All");
            protocolsList.Add("TCP");
            protocolsList.Add("UDP");
            protocolsList.Add("Ethernet");
            protocolsList.Add("ICMPv4");
            protocolsList.Add("ICMPv6");
            protocolsList.Add("IGMP");
        }

        private string get_Device_List()
        {
            captureDeviceList = new List<string>();
            var ver = SharpPcap.Version.VersionString;
            var stringDevices = "SharpPcap {0}, Example1.IfList.cs" + ver;
            devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                stringDevices = stringDevices + Environment.NewLine + "No devices were found on this machine";
                return stringDevices;
            }
            stringDevices = stringDevices + Environment.NewLine + "The following devices are available on this machine: ";
            int count = 0;
            foreach (ICaptureDevice dev in devices)
            {
                var device = count.ToString();
                stringDevices = stringDevices + Environment.NewLine + "----------------------------------------------------------------------" + Environment.NewLine + "Device #" + device + ": " + dev.ToString();
                captureDeviceList.Add(device);
                count++;
            }

            return stringDevices;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            packageDetailList.Clear();
            deviceIndex = int.Parse(comboBox1.Text);
            isPromisc = checkBox1.Checked;
            decodeMode = false;
            Thread l = new Thread(new ThreadStart(listen_Start))
            {
                IsBackground = true
            };

            l.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stopCapture = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deviceIndex = comboBox1.SelectedIndex;
            isPromisc = checkBox1.Checked;
            decodeMode = true;
            Thread l = new Thread(new ThreadStart(listen_Start))
            {
                IsBackground = true
            };

            l.Start();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            stopCapture = true;
        }

        private void updateLog()
        {
            textBox1.AppendText(Environment.NewLine + writeLine);
        }

        private void listen_Start()
        {
            ICaptureDevice device = devices[deviceIndex];

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            if (isPromisc == true)
            {
                device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            }
            else
            {
                device.Open(DeviceMode.Normal, readTimeoutMilliseconds);
            }

            string filter;

            if (decodeMode == false)
            {
                switch (typeOfDecode)
                {
                    case 0:
                        break;

                    case 1:
                        filter = "ip and udp";
                        device.Filter = filter;
                        break;

                    case 2:
                        filter = "ip and tcp";
                        device.Filter = filter;
                        break;
                    case 3:
                        filter = "ip and icmp";
                        device.Filter = filter;
                        break;
                    case 4:
                        filter = "icmp6";
                        device.Filter = filter;
                        break;
                    case 5:
                        filter = "ip and igmp";
                        device.Filter = filter;
                        break;
                    case 6:
                        filter = "arp";
                        device.Filter = filter;
                        break;
                }
            }
            //else
            //{
            //    filter = "udp port 161 or udp port 162";
            //    device.Filter = filter;
            //}

            device.StartCapture();
            writeLine = "--- Listening For Packets ---";
            Invoke(new MethodInvoker(updateLog));
            while (!stopCapture)
            {

            }

            device.StopCapture();
            device.Close();

            writeLine = " -- Capture stopped, device closed. --";
            Invoke(new MethodInvoker(updateLog));
            stopCapture = false;
        }

        private void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {
            Packet pack = Packet.ParsePacket(packet.Packet.LinkLayerType, packet.Packet.Data);
            DateTime time = packet.Packet.Timeval.Date;
            int len = packet.Packet.Data.Length;

            if (IsTCPPacket(pack))
            {
                ShowTCPPacket(pack, time, len);
            }
            else if (IsUDPPacket(pack))
            {
                ShowUDPPacket(pack, time, len);
            }
            else if (IsICMPv4Packet(pack))
            {
                ShowICMPv4Packet(pack, time, len);
            }
            else if (IsICMPv6Packet(pack))
            {
                ShowICMPv6Packet(pack, time, len);
            }
            else if(IsIGMPv2Packet(pack))
            {
                ShowIGMPv2Packet(pack, time, len);
            }
            else if (IsEthernetPacket(pack))
            {
                ShowEthernetPacket(pack, time, len);
            }
        }


        #region TCP
        public bool IsTCPPacket(Packet pack) => (TcpPacket)pack.Extract(typeof(TcpPacket)) != null;
        private void ShowTCPPacket(Packet pack, DateTime time, int len)
        {
            var tcpPacket = (TcpPacket)pack.Extract(typeof(TcpPacket));
            IpPacket ipPacket = (IpPacket)tcpPacket.ParentPacket;

            var packageDetail = new PackageDetail(tcpPacket, null, ipPacket);
            packageDetailList.Add(packageDetail);

            var srcIp = ipPacket.SourceAddress;
            var dstIp = ipPacket.DestinationAddress;
            var srcPort = tcpPacket.SourcePort;
            var dstPort = tcpPacket.DestinationPort;
            writeLine = string.Format("ID: {9} - {0}:{1}:{2},{3} - TCP Packet: {5}:{6}  -> {7}:{8}\n\n",
                                time.Hour, time.Minute, time.Second, time.Millisecond, len,
                                srcIp, srcPort, dstIp, dstPort, packageDetail.Id);
            Invoke(new MethodInvoker(updateLog));
        }
        #endregion

        #region Ethernet
        public bool IsEthernetPacket(Packet pack) => (EthernetPacket)pack.Extract(typeof(EthernetPacket)) != null;
        private void ShowEthernetPacket(Packet pack, DateTime time, int len)
        {
            var ethernetPacket = (EthernetPacket)pack.Extract(typeof(EthernetPacket));
            

            var packageDetail = new PackageDetail(ethernetPacket);
            packageDetailList.Add(packageDetail);

            var srcIp = ethernetPacket.SourceHwAddress;
            var dstIp = ethernetPacket.DestinationHwAddress;
            writeLine = string.Format("ID: {7} - {0}:{1}:{2},{3} - Ethernet Packet: {5} -> {6}\n\n",
                                time.Hour, time.Minute, time.Second, time.Millisecond, len,
                                srcIp, dstIp, packageDetail.Id);
            Invoke(new MethodInvoker(updateLog));
        }
        #endregion

        #region UDP
        public bool IsUDPPacket(Packet pack) => (UdpPacket)pack.Extract(typeof(UdpPacket)) != null;
        private void ShowUDPPacket(Packet pack, DateTime time, int len)
        {
            UdpPacket udpPacket = (UdpPacket)pack.Extract(typeof(UdpPacket));
            IpPacket ipPacket = (IpPacket)udpPacket.ParentPacket;

            var packageDetail = new PackageDetail(null, udpPacket, ipPacket);
            packageDetailList.Add(packageDetail);

            IPAddress srcIp = ipPacket.SourceAddress;
            IPAddress dstIp = ipPacket.DestinationAddress;
            ushort srcPort = udpPacket.SourcePort;
            ushort dstPort = udpPacket.DestinationPort;
            writeLine = (string.Format("ID: {9} - {0}:{1}:{2},{3} - UDP Packet: {5}:{6} -> {7}:{8}\n",
                            time.Hour, time.Minute, time.Second, time.Millisecond, len,
                            srcIp, srcPort, dstIp, dstPort, packageDetail.Id));
            Invoke(new MethodInvoker(updateLog));
            if (decodeMode == true)
            {
                byte[] packetBytes = udpPacket.PayloadData;
                int version = SnmpPacket.GetProtocolVersion(packetBytes, packetBytes.Length);
                switch (version)
                {
                    case (int)SnmpVersion.Ver1:
                        SnmpV1Packet snmpPacket = new SnmpV1Packet();
                        try
                        {
                            snmpPacket.decode(packetBytes, packetBytes.Length);
                            writeLine = "SNMP.V1 Packet: " + snmpPacket.ToString();
                        }
                        catch (Exception e)
                        {
                            writeLine = e.ToString();
                        }
                        break;
                    case (int)SnmpVersion.Ver2:
                        SnmpV2Packet snmp2Packet = new SnmpV2Packet();
                        try
                        {
                            snmp2Packet.decode(packetBytes, packetBytes.Length);
                            writeLine = "SNMP.V2 Packet: " + snmp2Packet.ToString();
                        }
                        catch (Exception e)
                        {
                            writeLine = e.ToString();
                        }
                        break;
                    case (int)SnmpVersion.Ver3:
                        SnmpV3Packet snmp3Packet = new SnmpV3Packet();
                        try
                        {
                            snmp3Packet.decode(packetBytes, packetBytes.Length);
                            writeLine = "SNMP.V3 Packet: " + snmp3Packet.ToString();
                        }
                        catch (Exception e)
                        {
                            writeLine = e.ToString();
                        }
                        break;
                }
                Invoke(new MethodInvoker(updateLog));
            }
        }
        #endregion

        #region ICMPv4
        public bool IsICMPv4Packet(Packet pack) => (ICMPv4Packet)pack.Extract(typeof(ICMPv4Packet)) != null;

        private void ShowICMPv4Packet(Packet pack, DateTime time, int len)
        {
            var icmpv4Packet = (ICMPv4Packet)pack.Extract(typeof(ICMPv4Packet));
            IpPacket ipPacket = (IpPacket)icmpv4Packet.ParentPacket;

            var packageDetail = new PackageDetail(icmpv4Packet, ipPacket);
            packageDetailList.Add(packageDetail);

            var srcIp = ipPacket.SourceAddress;
            var dstIp = ipPacket.DestinationAddress;
            writeLine = string.Format("ID: {7} - {0}:{1}:{2},{3} - ICMPv4 Packet: {5} -> {6}\n\n",
                                time.Hour, time.Minute, time.Second, time.Millisecond, len,
                                srcIp, dstIp, packageDetail.Id);
            Invoke(new MethodInvoker(updateLog));
        }
        #endregion

        #region ICMPv6
        public bool IsICMPv6Packet(Packet pack) => (ICMPv6Packet)pack.Extract(typeof(ICMPv6Packet)) != null;

        private void ShowICMPv6Packet(Packet pack, DateTime time, int len)
        {
            var icmpv6Packet = (ICMPv6Packet)pack.Extract(typeof(ICMPv6Packet));
            IpPacket ipPacket = (IpPacket)icmpv6Packet.ParentPacket;

            var packageDetail = new PackageDetail(icmpv6Packet, ipPacket);
            packageDetailList.Add(packageDetail);

            var srcIp = ipPacket.SourceAddress;
            var dstIp = ipPacket.DestinationAddress;
            writeLine = string.Format("ID: {7} - {0}:{1}:{2},{3} - ICMPv6 Packet: {5} -> {6}\n\n",
                                time.Hour, time.Minute, time.Second, time.Millisecond, len,
                                srcIp, dstIp, packageDetail.Id);
            Invoke(new MethodInvoker(updateLog));
        }
        #endregion

        #region IGMP
        public bool IsIGMPv2Packet(Packet pack) => (IGMPv2Packet)pack.Extract(typeof(IGMPv2Packet)) != null;

        private void ShowIGMPv2Packet(Packet pack, DateTime time, int len)
        {
            var igmpv2Packet = (IGMPv2Packet)pack.Extract(typeof(IGMPv2Packet));
            IpPacket ipPacket = (IpPacket)igmpv2Packet.ParentPacket;

            var packageDetail = new PackageDetail(igmpv2Packet, ipPacket);
            packageDetailList.Add(packageDetail);

            var srcIp = ipPacket.SourceAddress;
            var dstIp = ipPacket.DestinationAddress;
            writeLine = string.Format("ID: {7} - {0}:{1}:{2},{3} - IGMPv2 Packet: {5} -> {6}\n\n",
                                time.Hour, time.Minute, time.Second, time.Millisecond, len,
                                srcIp, dstIp, packageDetail.Id);
            Invoke(new MethodInvoker(updateLog));
        }

        #endregion
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            new PackageDetailForm(packageDetailList).Show();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = sender as ComboBox;

            switch (combo.SelectedItem as string)
            {
                case "All":
                    typeOfDecode = 0;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
                case "UDP":
                    typeOfDecode = 1;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    break;
                case "TCP":
                    typeOfDecode = 2;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
                case "ICMPv4":
                    typeOfDecode = 3;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
                case "ICMPv6":
                    typeOfDecode = 4;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
                case "IGMP":
                    typeOfDecode = 5;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
                case "Ethernet":
                    typeOfDecode = 6;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    break;
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
