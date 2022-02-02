
namespace NetTCPSocket
{
    public class CommandType
    {
        public const string AUTHORIZATION = "authorization";
        public const string TABLE = "table";
        public const string SWITCHES = "switches";
        public const string MYSQL = "mysql";
        public const string USERSMYSQL = "usersmysql";
        public const string ACCESS = "access";

        public const string ACCEPT = "accept";
        public const string ACCEPTED = "accepted";
        public const string DENIED = "denied";
        public const string REJECT = "reject";
        public const string REJECTED = "rejected";

        public const string CONNECT = "connect";
        public const string CONNECTED = "connected";
        public const string DISCONNECT = "disconnect";
        public const string DISCONNECTED = "disconnected";

        public const string SUCCESS = "success";
        public const string SUCCESSED = "successed";

        public const string DONE = "done";
        public const string DONED = "doned";

        public const string ERROR = "error";
        public const string WRONG = "wrong";
    }

    public class RequestType
    {
        public const string GET = "get";
        public const string CREATE = "create";
        public const string UPDATE = "update";
        public const string UPLOAD = "upload";
        public const string DELETE = "delete";
    }

    internal class CommandMessage
    {
        protected internal const string EndMessage = "<EOF>";
        protected internal const string DisconnectMessage = "<DCT>";
    }
}
