namespace SimpleNetExecutor.Server
{
    public class Endpoint
    {
        public int Id { get; set; }
        public string EndpointId { get; set; }
        public DateTime LastEndpointHeartbeat { get; set; }
    }

    public class Module
    {
        public int Id { get; set; }
        public string ModuleMd5 { get; set; }
        public byte[] ModuleDll { get; set; }
    }
}