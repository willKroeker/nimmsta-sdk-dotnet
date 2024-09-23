namespace Nimmsta.Net.CoreApi.Request;

public class NimmstaDeviceRequest : NimmstaRequestMessage
{
    public NimmstaDeviceRequest(int id, string deviceAddress,
        NimmstaRequestAction action)
        : base("DEVICE_REQUEST", id, action)
    {
        DeviceAddress = deviceAddress;
    }
}
