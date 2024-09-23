using Nimmsta.Net.Layout.Helpers;
using System.Xml.Linq;

namespace Nimmsta.Net.Layout.Elements;

/// <summary>
/// Buttons represent a touchable button on the screen.
/// Each Button will automatically trigger the vibrator 
/// of the device once recognized and will fire a ButtonClickEvent,
/// which will automatically call the didClickButton method (in Kotlin context).
/// </summary>
public class NimmstaButton(string name) : NimmstaElement(name)
{
    protected override string LayoutXmlElementName => "button";

    /// <summary>
    /// Defines if the vibrator should be triggered when the button is clicked.
    /// Default: true
    /// </summary>
    public bool ShouldTriggerVibratorOnClick { get; set; } = true;

    /// <summary>
    /// Padding to be applied to the button.
    /// Will set paddingLeft, paddingRight, paddingTop and paddingBottom.
    /// Default: 5.0
    /// </summary>
    public NimmstaButtonPadding Padding { get; set; } = new NimmstaButtonPadding(5.0f);

    /// <summary>
    /// Defines the primary style of the button including the fill color,
    /// text color, 3D and rounding.
    /// Default: INVERTED3D_ROUNDED
    /// </summary>
    public NimmstaButtonType ButtonType { get; set; } = NimmstaButtonType.INVERTED3D_ROUNDED;

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

        if (!ShouldTriggerVibratorOnClick)
            xmlElement.Add(new XAttribute("shouldTriggerVibratorOnClick", "false"));

        Padding.ApplyPaddingToLayoutXml(xmlElement);

        if (ButtonType != NimmstaButtonType.INVERTED3D_ROUNDED)
            xmlElement.Add(new XAttribute("type", ButtonType.ToString()));

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
