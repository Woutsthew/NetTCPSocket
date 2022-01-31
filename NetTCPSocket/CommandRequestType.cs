
namespace NetTCPSocket
{
    public class CommandType
    {
        public const string CONNECT = "connect";
        public const string AUTHORIZATION = "authorization";
        public const string TABLE = "table";
        public const string SWITCHES = "switches";
        public const string MYSQL = "mysql";
        public const string USERSMYSQL = "usersmysql";

        public const string ACCEPTED = "accepted";
        public const string DENIED = "denied";
    }

    public class RequestType
    {
        public const string GET = "get";
        public const string CREATE = "create";
        public const string UPDATE = "update";
        public const string UPLOAD = "upload";
        public const string DELETE = "delete";
    }

    public class CommandMessage
    {
        protected internal const string EndMessage = "<EOF>";
        protected internal const string DisconnectMessage = "<DCT>";

        protected internal const string ClientTerminated = "Не удается прочитать данные из транспортного соединения: Удаленный хост принудительно разорвал существующее подключение.";
        protected internal const string HostTerminated = "Не удается прочитать данные из транспортного соединения: Программа на вашем хост-компьютере разорвала установленное подключение.";
        protected internal const string WSATerminated = "Не удается прочитать данные из транспортного соединения: Операция блокирования прервана вызовом WSACancelBlockingCall.";

        //protected internal const string LiquidatedObject = "Доступ к ликвидированному объекту невозможен.\r\nИмя объекта: \"System.Net.Sockets.NetworkStream\".";

        //protected internal const string ThreadedTerminated = "Поток находился в процессе прерывания.";
    }
}
