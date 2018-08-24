
using System.Web;

namespace DeaconCCGManagement.ViewModels
{
    public class DescriptionWithLinkPanel
    {
        public HtmlString Title { get; set; }
        public HtmlString Description { get; set; }
        public HtmlString href { get; set; }       
        public HtmlString Glyph { get; set; }
        public HtmlString ImageSrc { get; set; }
        public HtmlString ImageSrcSm { get; set; }
    }
}