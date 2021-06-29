# Packet Capture Tool

## Objective
- Software for capturing TCP and UDP packets created by JamesE-J and Bruno Caregnato. 
- This project added the capture of ICMPv4, ICMPv6 and IGMPv2 packets<br/>
![Tool](Images/home_screen.png?raw=true "Tool")

## Usage 
- Remove the currently installed npcap driver
- Install version 0.991 (https://nmap.org/npcap/dist/npcap-0.991.exe)
- Add wpcap.dll (Packet_Capture_Tool\dll) in Packet_Capture_Tool\bin\Debug
- Run project

## How to Use
- It is possible to carry out the capture of UDP, TCP, ICMPv4, ICMPv6 and IGMPv2 packets, the screen on the side will show the captured packets and an id.<br/>
![Capturing Packages](Images/capturing_packages.png?raw=true "Capturing Packages")

- In the button of detailing package, it is possible to put the requested package ID, showing its details:<br/>
### TCP PACKAGE DETAIL EXAMPLE	
![Tcp detail](Images/tcp_detail.png?raw=true "Tcp detail")<br/>
### UDP PACKAGE DETAIL EXAMPLE
![Udp detail](Images/udp_detail.png?raw=true "Udp detail")<br/>
### ICMPv4 PACKAGE DETAIL EXAMPLE
![ICMPv4 detail](Images/icmpv4_detail.png?raw=true "ICMPv4 detail")<br/>
### ICMPv6 PACKAGE DETAIL EXAMPLE
![ICMPv6 detail](Images/icmpv6_detail.png?raw=true "ICMPv6 detail")<br/>
### IGMPv2 PACKAGE DETAIL EXAMPLE
![IGMPv2 detail](Images/igmpv2_detail.png?raw=true "IGMPv2 detail")<br/>

## Code Changes

### Capture of icmpv4, icmpv6 e igmpv2
![Capture detail](Images/classe_captura_pacotes.png?raw=true "Capture detail")<br/>
### ICMPv4
![ICMPv4 detail](Images/classe_icmpv4.png?raw=true "icmpv4")<br/>
### ICMPv6
![ICMPv6 detail](Images/classe_icmpv6.png?raw=true "icmpv6")<br/>
### IGMPv2
![IGMPv2 detail](Images/classe_igmpv2.png?raw=true "igmpv2")<br/>

### Filter
![Filter detail](Images/classe_filtros.png?raw=true "filter")<br/>

### Details
![Filter detail](Images/classe_detalhes.png?raw=true "filter")<br/>

