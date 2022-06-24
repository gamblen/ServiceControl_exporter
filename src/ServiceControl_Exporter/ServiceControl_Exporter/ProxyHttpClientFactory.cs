namespace ServiceControl_Exporter;

using System.Net;
using Flurl.Http.Configuration;

public class ProxyHttpClientFactory : DefaultHttpClientFactory
{
    private readonly string _address;

    public ProxyHttpClientFactory(string address)
    {
        _address = address;
    }

    public override HttpMessageHandler CreateMessageHandler()
    {
        return new HttpClientHandler
               {
                   Proxy = _address == null ? null : new WebProxy(_address),
                   UseProxy = _address != null
               };
    }
}