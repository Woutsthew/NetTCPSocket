
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
}
