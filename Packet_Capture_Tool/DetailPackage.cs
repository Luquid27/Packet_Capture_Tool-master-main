using PacketDotNet;
using System.Threading;

namespace Packet_Capture_Tool
{
    public class PackageDetail
    {
        public int Id { get; private set; }

        public UdpPacket UdpPacket { get; private set; }

        public TcpPacket TcpPacket { get; private set; }
        public ICMPv4Packet ICMPv4Packet { get; private set; }
        public ICMPv6Packet ICMPv6Packet { get; private set; }
        public IGMPv2Packet IGMPv2Packet { get; private set; }
        public EthernetPacket EthernetPacket { get; private set; }

        public IpPacket IpPacket { get; private set; }

        private static int _newId;

        public PackageDetail()
        {
            _newId = 0;
        }

        public PackageDetail(TcpPacket tcpPacket, UdpPacket udpPacket, IpPacket ipPacket)
        {
            Id = Interlocked.Increment(ref _newId);
            TcpPacket = tcpPacket;
            UdpPacket = udpPacket;
            IpPacket = ipPacket;
        }

        public PackageDetail(ICMPv4Packet iCMPv4Packet, IpPacket ipPacket)
        {
            Id = Interlocked.Increment(ref _newId);
            ICMPv4Packet = iCMPv4Packet;
            IpPacket = ipPacket;
        }
        public PackageDetail(ICMPv6Packet iCMPv6Packet, IpPacket ipPacket)
        {
            Id = Interlocked.Increment(ref _newId);
            ICMPv6Packet = iCMPv6Packet;
            IpPacket = ipPacket;
        }
        public PackageDetail(IGMPv2Packet iGMPv2Packet, IpPacket ipPacket)
        {
            Id = Interlocked.Increment(ref _newId);
            IGMPv2Packet = iGMPv2Packet;
            IpPacket = ipPacket;
        }
        public PackageDetail(EthernetPacket ethernetPacket)
        {
            Id = Interlocked.Increment(ref _newId);
            EthernetPacket = ethernetPacket;
        }


    }
}
