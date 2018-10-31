using System;
using System.Text;
using CaseService.Services.DTO;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace HL7Server.Services {
    public class MessagingService {

        private readonly IQueueClient client = new QueueClient(HL7Server.Configuration.ServiceBus.connectionURI, HL7Server.Configuration.ServiceBus.orderQueue);

        public void SendCreateCaseMessage(CreateCaseDTO dto) {
            try {
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dto)));
                client.SendAsync(message);
            } catch (Exception exception) {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

    }
}