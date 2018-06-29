public struct ConnectionInfo{

        public string ipAddress;
    public string fromAddress;
    //public string data;
    public string name;
    //public interface port;
    public ConnectionInfo(string fromAddress, string data)
    {
        this.fromAddress = fromAddress;
        ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - (fromAddress.LastIndexOf(":") + 1)); //fromAddress.Length - fromAddress.LastIndexOf(":") +2);
        name = data;
    }
}
