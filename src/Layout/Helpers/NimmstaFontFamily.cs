namespace Nimmsta.Net.Layout.Helpers;

/// <summary>
/// The fonts support all printable ASCII characters.
/// Additionally, the following characters are available:
/// <br/>Ä, ä, Ö, ö, Ü, ü, ß, €, £, ¥
/// <br/>NIMMSTA docs: <see href="https://docs.nimmsta.com/layout/1Layouts/#fonts"/>
/// </summary>
/// <remarks>
/// Please be aware that since the device manages spacing internally, the font is not monospace anymore.
/// </remarks>
public enum NimmstaFontFamily
{
    LiberationMono,
    Bahnschrift_SemiBold,
    Bahnschrift_SemiLight
}
