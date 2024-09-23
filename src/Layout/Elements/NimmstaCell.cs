using Nimmsta.Net.Layout.Helpers;
using System.Xml.Linq;

namespace Nimmsta.Net.Layout.Elements;

public class NimmstaCell(string name) : NimmstaElement(name)
{
    protected override string LayoutXmlElementName => "cell";

    /// <summary>
    /// (Unit: pt) Default: 17pt
    /// </summary>
    public NimmstaFontSize FontSize { get; set; } = NimmstaFontSize.Font17pt;

    /// <summary>
    /// (Unit: pt) Default: LiberationMono
    /// </summary>
    public NimmstaFontFamily FontFamily { get; set; } = NimmstaFontFamily.LiberationMono;

    /// <summary>
    /// Default: BLACK
    /// </summary>
    public NimmstaTextColor TextColor { get; set; } = NimmstaTextColor.BLACK;

    /// <summary>
    /// Changes the background color Default: WHITE (Since 5.3)
    /// </summary>
    public NimmstaBackgroundColor BackgroundColor { get; set; } = NimmstaBackgroundColor.WHITE;

    /// <summary>
    /// (Unit: px) Default: 1px
    /// </summary>
    public int CharacterSpacing { get; set; } = 1;

    /// <summary>
    /// (Unit: pt) Adds padding around the text Default: 0px (Since 5.3)
    /// </summary>
    public int TextPadding { get; set; } = 0;

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

        if (FontSize != NimmstaFontSize.Font17pt)
        {
            xmlElement.Add(new XAttribute("fontSize",
                FontSize switch
                {
                    NimmstaFontSize.AutoSize => "auto",
                    NimmstaFontSize.Font11pt => "11pt",
                    NimmstaFontSize.Font22pt => "22pt",
                    NimmstaFontSize.Font26pt => "26pt",
                    NimmstaFontSize.Font30pt => "30pt",
                    NimmstaFontSize.Font34pt => "34pt",
                    NimmstaFontSize.Font40pt => "40pt",
                    NimmstaFontSize.Font52pt => "52pt",
                    _ => "17pt"

                }));
        }

        if (FontFamily != NimmstaFontFamily.LiberationMono)
            xmlElement.Add(new XAttribute("fontFamily", FontFamily.ToString()));

        if (CharacterSpacing != 1)
            xmlElement.Add(new XAttribute("characterSpacing", CharacterSpacing));

        if (TextColor != NimmstaTextColor.BLACK)
            xmlElement.Add(new XAttribute("textColor", TextColor.ToString()));

        if (TextPadding > 0)
            xmlElement.Add(new XAttribute("padding", TextPadding));

        if (BackgroundColor != NimmstaBackgroundColor.WHITE)
            xmlElement.Add(new XAttribute("backgroundColor", BackgroundColor.ToString()));

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
