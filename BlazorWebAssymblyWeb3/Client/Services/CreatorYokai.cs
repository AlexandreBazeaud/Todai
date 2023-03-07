using System.Text;

namespace BlazorWebAssymblyWeb3.Client.Services;

public class CreatorYokai
{
    public Attribute? Body{ get; set; }
    public Attribute? Hair{ get; set; }
    public Attribute? Mouth{ get; set; }
    public Attribute? Nose{ get; set; }
    public Attribute? Eyes{ get; set; }
    public Attribute? Eyebrow{ get; set; }
    public Attribute? Mark{ get; set; }
    public Attribute? Accessory { get; set; }
    public Attribute? Earrings{ get; set; }
    public Attribute? Mask{ get; set; }
    
    
    
    public string TEst()
    {
        StringBuilder str = new("<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" x=\"0px\" y=\"0px\" viewBox=\"0 0 420 420\" style=\"enable-background:new 0 0 420 420;\" xml:space=\"preserve\">");
        if(Body != null)
            str.AppendLine(Body.SvgCode);
        if(Hair != null)
            str.AppendLine(Hair.SvgCode);
        if(Mouth != null)
            str.AppendLine(Mouth.SvgCode);
        if(Nose != null)
            str.AppendLine(Nose.SvgCode);
        if(Eyes != null)
            str.AppendLine(Eyes.SvgCode);
        if(Eyebrow != null)
            str.AppendLine(Eyebrow.SvgCode);
        if(Mark != null)
            str.AppendLine(Mark.SvgCode);
        if(Accessory != null)
            str.AppendLine(Accessory.SvgCode);
        if(Earrings != null)
            str.AppendLine(Earrings.SvgCode);
        if(Mask != null)
            str.AppendLine(Mask.SvgCode);

        str.AppendLine("</svg>");
        return str.ToString();
    }
}

public class Attribute
{
    public string Name;
    public string SvgCode;
    public uint Score;
}

public class ProbaItems
{
    public string name { get; set; }
    public List<ProbaItem> items { get; set; }
}

public class ProbaItem
{
    public string name { get; set; }
    public string proba { get; set; }
    public string svg { get; set; }
    public string score { get; set; }
}