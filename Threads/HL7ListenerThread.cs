using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CaseService.Services.DTO;
using HL7Server.Messages;
using HL7Server.Services;
using HL7Server.Utils;

namespace HL7Server.Threads {
    public class HL7ListenerThread {

        private static readonly MessagingService messagingService = new MessagingService();

        public static void Thread() {
            IPAddress address = new IPAddress(HL7Server.Configuration.HL7Server.ip ); 
            var endPoint = new IPEndPoint(address, HL7Server.Configuration.HL7Server.port); 
 
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
            try  { 
                listener.Bind(endPoint); 
                listener.Listen(3); 
 
                String data = ""; 
 
                while (true) { 
                    Console.WriteLine("Listening on port {0}", endPoint); 
                     
                    byte[] buffer = new byte[4096]; 
                     
                    // handle incoming connection ... 
                    var handler = listener.Accept(); 
                    Console.WriteLine("Handling incoming connection ..."); 
                    while (true) { 
                        int count = handler.Receive(buffer); 
                        data += Encoding.UTF8.GetString(buffer, 0, count); 
 
                        // Find start of MLLP frame, a VT character ... 
                        int start = data.IndexOf((char) 0x0B); 
                        if (start >= 0) { 
                            // Now look for the end of the frame, a FS character 
                            int end = data.IndexOf((char) 0x1C); 
                            if (end > start) { 
                                string temp = data.Substring(start + 1, end - start); 
 
                                // handle message 
                                string response = HandleMessage(temp); 
 
                                // Send response 
                                handler.Send(Encoding.UTF8.GetBytes(response)); 
                                break;  
                            } 
                        } 
                    } 
 
                    // close connection 
                    handler.Shutdown( SocketShutdown.Both); 
                    handler.Close(); 
 
                    Console.WriteLine("Connection closed."); 
 
                } 
 
            } catch (Exception e) { 
                Console.WriteLine("Exception caught: {0}", e.Message); 
            } 
            Console.WriteLine("Terminating - press ENTER"); 
            Console.ReadLine(); 
        }

        private static string HandleMessage(string data) { 
            Console.WriteLine("Received message"); 
 
            var msg = new HL7Message(); 
            msg.Parse(data); 
 
            Console.WriteLine("Parsed message     : {0}", msg.MessageType() ); 
            Console.WriteLine("Message timestamp  : {0}", msg.MessageDateTime() ); 
            Console.WriteLine("Message control id : {0}", msg.MessageControlId());
            Console.WriteLine("MSG: " + msg.Serialize()); 
            Console.WriteLine("____________________________________");

            // Segment firstSegment = msg.FindSegment("OBR");
            // Segment secondSegment = msg.FindNextSegment("OBR", firstSegment);
            // Segment thirdSegment = msg.FindNextSegment("OBR", secondSegment);

            // Console.WriteLine("FIRST: " + firstSegment.Serialize());
            // Console.WriteLine("SECOND: " + secondSegment.Serialize());
            // Console.WriteLine("THIRD: " + thirdSegment.Serialize());

            CreatePatientDTO patientDTO = DTOUtils.CreatePatientDTO(msg);
            CreateRequestorDTO requestorDTO = DTOUtils.CreateRequestorDTO(msg);
            CreateSpecimenDTO specimenDTO = DTOUtils.CreateSpecimenDTO(msg);
            List<CreateSpecimenDTO> specimens = new List<CreateSpecimenDTO>();
            
            CreateCaseDTO caseDTO = new CreateCaseDTO();
            caseDTO.Patient = patientDTO;
            caseDTO.Requestor = requestorDTO;
            caseDTO.Specimens = specimens;
            caseDTO.Type = "Histology";


            messagingService.SendCreateCaseMessage(caseDTO);
            
            // for(int i = 0; i <= 25; i++) {
            //     Console.WriteLine(i + ": " + msg.FindSegment("OBR").Field(i));
            // }
 
            // ********************************************************************* 
            // Here you could do something usefull with the received message ;-) 
            // ********************************************************************* 
 
 
            // todo  
 
 
            // Create a response message 
            // 
            var response = new HL7Message(); 
 
            var msh = new Segment("MSH"); 
            msh.Field(2, "^~\\&"); 
            msh.Field(7, DateTime.Now.ToString("yyyyMMddhhmmsszzz")); 
            msh.Field(9, "ACK"); 
            msh.Field(10, Guid.NewGuid().ToString() ); 
            msh.Field(11, "P"); 
            msh.Field(12, "2.5.1"); 
            response.Add(msh); 
 
            var msa = new Segment("MSA"); 
            msa.Field(1, "AA"); 
            msa.Field(2, msg.MessageControlId()); 
            response.Add(msa); 
 
 
            // Put response message into an MLLP frame ( <VT> data <FS><CR> ) 
            // 
            var frame = new StringBuilder(); 
            frame.Append((char) 0x0B); 
            frame.Append(response.Serialize()); 
            frame.Append( (char) 0x1C); 
            frame.Append( (char) 0x0D); 
 
            return frame.ToString(); 
        } 
    }
}