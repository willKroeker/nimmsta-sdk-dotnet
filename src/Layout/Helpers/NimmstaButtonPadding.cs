using System.Xml.Linq;

namespace Nimmsta.Net.Layout.Helpers;

public class NimmstaButtonPadding
{
    public NimmstaButtonPadding()
        : this(5.0f) { }

    public NimmstaButtonPadding(float padding)
        : this(padding, padding, padding, padding) { }

    public NimmstaButtonPadding(
        float paddingLeft, float paddingRight,
        float paddingTop, float paddingBottom)
    {
        PaddingLeft = paddingLeft;
        PaddingRight = paddingRight;
        PaddingTop = paddingTop;
        PaddingBottom = paddingBottom;
    }

    public float PaddingLeft { get; }
    public float PaddingRight { get; }
    public float PaddingTop { get; }
    public float PaddingBottom { get; }

    internal void ApplyPaddingToLayoutXml(XElement buttonXmlElement)
    {
        var allSamePadding = PaddingLeft == PaddingRight
                          && PaddingRight == PaddingTop
                          && PaddingTop == PaddingBottom;

        if (allSamePadding)
        {
            if (PaddingLeft != 5.0f)
                buttonXmlElement.Add(new XAttribute("padding", PaddingLeft));
            return;
        }

        if (PaddingLeft != 5.0f)
            buttonXmlElement.Add(new XAttribute("paddingLeft", PaddingLeft));
        if (PaddingRight != 5.0f)
            buttonXmlElement.Add(new XAttribute("paddingRight ", PaddingRight));
        if (PaddingTop != 5.0f)
            buttonXmlElement.Add(new XAttribute("paddingTop ", PaddingTop));
        if (PaddingBottom != 5.0f)
            buttonXmlElement.Add(new XAttribute("paddingBottom ", PaddingBottom));
    }
}
