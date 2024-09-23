using Nimmsta.Net.Layout.Helpers;
using System.Xml.Linq;

namespace Nimmsta.Net.Layout.Elements;

public class NimmstaScanResult() : NimmstaElement(string.Empty)
{
    protected override string LayoutXmlElementName => "scanResult";

    /// <summary>
    /// How many seconds the result will be shown.
    /// If null the result will be displayed until it is overwritten.
    /// Default: null
    /// </summary>
    public int? TimeToShowBarcode { get; set; } = null;

    /// <summary>
    /// Defines if the barcode rules should be applied before displaying
    /// the barcode on the screen. If set to false the barcode will
    /// be displayed as scanned, without applying the rules first.
    /// Default: false
    /// </summary>
    public bool ApplyRule { get; set; } = false;

    /// <summary>
    /// (Unit: pt) Default: 17pt; Supported: 11pt - 17pt - 26pt - 30pt - 34pt - 40pt - 52pt
    /// </summary>
    public NimmstaFontSize FontSize { get; set; } = NimmstaFontSize.Font17pt;

    /// <summary>
    /// (Unit: px) Default: 1px
    /// </summary>
    public int CharacterSpacing { get; set; } = 1;

    /// <summary>
    /// Default: BLACK
    /// </summary>
    public NimmstaTextColor TextColor { get; set; } = NimmstaTextColor.BLACK;

    /// <summary>
    /// Default: START
    /// </summary>
    public NimmstaTextAlignment HorizontalAlignment { get; set; } = NimmstaTextAlignment.START;

    /// <summary>
    /// Default: START
    /// </summary>
    public NimmstaTextAlignment VerticalAlignment { get; set; } = NimmstaTextAlignment.START;

    /// <summary>
    /// Defines if lines that are longer than the screen will wrap around to the next line Default: NO_WRAP
    /// </summary>
    public bool WrapLongText { get; set; } = false;

    /// <summary>
    /// (Unit: %) Default: 110%
    /// </summary>
    public int LineHeight { get; set; } = 110;

    /// <summary>
    /// Defines the maximum amount of lines, other lines are cut off
    /// (values less than or equal to 0 mean unlimited lines) Default: 1
    /// </summary>
    public int MaxLines { get; set; } = 1;

    public override XElement CreateLayoutXml()
    {
        var xmlElement = base.CreateLayoutXml();

        if (TimeToShowBarcode != null)
            xmlElement.Add(new XAttribute("timeToShowBarcode", TimeToShowBarcode.Value));

        if (ApplyRule)
            xmlElement.Add(new XAttribute("applyRule", "true"));

        if (FontSize != NimmstaFontSize.Font17pt && FontSize != NimmstaFontSize.AutoSize)
        {
            xmlElement.Add(new XAttribute("fontSize",
                FontSize switch
                {
                    NimmstaFontSize.Font11pt => "11pt",
                    NimmstaFontSize.Font26pt => "26pt",
                    NimmstaFontSize.Font30pt => "30pt",
                    NimmstaFontSize.Font34pt => "34pt",
                    NimmstaFontSize.Font40pt => "40pt",
                    NimmstaFontSize.Font52pt => "52pt",
                    _ => "17pt"
                }));
        }

        if (CharacterSpacing != 1)
            xmlElement.Add(new XAttribute("characterSpacing", CharacterSpacing));

        if (TextColor != NimmstaTextColor.BLACK)
            xmlElement.Add(new XAttribute("textColor", TextColor.ToString()));

        if (HorizontalAlignment != NimmstaTextAlignment.START)
            xmlElement.Add(new XAttribute("horizontalAlignment", HorizontalAlignment.ToString()));
        if (VerticalAlignment != NimmstaTextAlignment.START)
            xmlElement.Add(new XAttribute("verticalAlignment", VerticalAlignment.ToString()));

        if (WrapLongText)
            xmlElement.Add(new XAttribute("wrapMode", "WRAP"));

        if (LineHeight != 110) // Default: 110%
            xmlElement.Add(new XAttribute("lineHeight", LineHeight));
        if (MaxLines != 1) // Default: 1
            xmlElement.Add(new XAttribute("maxLines", MaxLines));

        return xmlElement;
    }
}
