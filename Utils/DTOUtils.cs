using System;
using System.Collections.Generic;
using CaseService.Services.DTO;
using HL7Server.Messages;

namespace HL7Server.Utils {
    public class DTOUtils {

        public static CreateSpecimenDTO CreateSpecimenDTO(HL7Message msg) {
            CreateSpecimenDTO result = new CreateSpecimenDTO();
            Segment segment = msg.FindSegment("OBR");

            result.TissueType = segment.Field(15).Split("^")[0];
            result.BlockId = Convert.ToInt64(segment.Field(19).Split(";")[2]);    
            result.Slides = CreateSlideDTOs(msg);
            return result;
        }

        private static List<CreateSlideDTO> CreateSlideDTOs(HL7Message msg) {
            List<CreateSlideDTO> result = new List<CreateSlideDTO>();

            Segment seg = msg.FindSegment("OBR");
            result.Add(CreateSlideDTO(seg));
            while(true) {
                try {
                    seg = msg.FindNextSegment("OBR", seg);
                    result.Add(CreateSlideDTO(seg));
                } catch(Exception e) {
                    Console.WriteLine($"{DateTime.Now} :: Exception: {e.Message}");
                    break;
                }
            }

            return result;
        }

        private static CreateSlideDTO CreateSlideDTO(Segment segment) {
            CreateSlideDTO result = new CreateSlideDTO();

            result.ProtocolName = segment.Field(4).Split("^")[1];
            result.ProtocolDescription = result.ProtocolName + " Initial";

            return result;
        }

        public static CreatePatientDTO CreatePatientDTO(HL7Message msg) {
            CreatePatientDTO result = new CreatePatientDTO();
            Segment patientSegment = msg.FindSegment("PID");

            result.Gender = GetPatientGender(patientSegment);
            result.FirstName = GetPatientFirstName(patientSegment);
            result.LastName = GetPatientLastName(patientSegment);
            
            return result;
        }

        public static CreateRequestorDTO CreateRequestorDTO(HL7Message msg) {
            CreateRequestorDTO result = new CreateRequestorDTO();
            Segment requestorSegment = msg.FindSegment("PV1");
            string[] chunks = requestorSegment.Field(7).Split("^");

            result.FirstName = chunks[2];
            result.LastName = chunks[1];

            return result;
        }

        private static string GetPatientFirstName(Segment segment) {
            string fn = segment.Field(5);

            return fn.Split("^")[1];
        }

        private static string GetPatientLastName(Segment segment) {
            string fn = segment.Field(5);

            return fn.Split("^")[0];
        }

        private static string GetPatientGender(Segment segment) {
            string g = segment.Field(8);
            string result = null;
            switch(g) {
                case "M":
                    result = "Male";
                break;
                case "F":
                    result = "Female";
                break;
                case "U":
                    result = "Undefined";
                break;
            }

            return result;
        }
    }
}