namespace VoiceCreator;

public partial class OpenTTS
{
    private byte[] _lastResponse;
    private bool _success;

    partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
    {
        var stream = response.Content.ReadAsStream();

        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        _lastResponse = ms.ToArray();
        _success = true;
    }

    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request,
        string url)
    {
        _success = false;
    }

    public byte[] LastResponse => _lastResponse;
    public bool IsSuccess => _success;

}