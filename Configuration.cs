namespace HL7Server.Configuration {

    public class Swagger {
        public static string version = "v1";
        public static string title = "HL7 Server";
    }

    public class HL7Server {
        public static byte[] ip = { 127, 0, 0, 1 };
        public static int port = 2222;
    }

    public class ServiceBus {
        public static string connectionURI = "Endpoint=sb://caseorders.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UfUyg49Eic+4MHu5fvw6XRXqXnutPPZk5NDYzy2EUFs=";
        public static string orderQueue = "neworder";

    }
}