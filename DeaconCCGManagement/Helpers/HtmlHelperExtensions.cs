using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DeaconCCGManagement.Helpers
{
    /// <summary>
    /// Html helper extension methods.
    /// </summary>
    public static class HtmlHelperExtensions
    {        
        /// <summary>
        /// Extension Html helper method for a submit button.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="value">Display text on the button.</param>
        /// <param name="classAttr">Class</param>
        /// <param name="id">Element's id</param>
        /// <returns></returns>
        public static MvcHtmlString Submit(this HtmlHelper helper, string value = "Save", string id = "Submit", string classAttr="btn btn-default")
        {
            return new MvcHtmlString("<input id=\"" + id + "\" type=\"submit\" class=\"" + classAttr + "\" value =\"" + value + "\"  />");
        }
        /// <summary>
        /// Returns a url for the items in the 'Items Per Page'
        /// drop down menu for the pagination.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="id"></param>
        /// <param name="memberId"></param>
        /// <param name="ccgId"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="listAll"></param>
        /// <param name="getAll"></param>
        /// <param name="query"></param>
        /// <param name="selectAll"></param>
        /// <param name="allAccess"></param>
        /// <param name="archive"></param>
        /// <param name="dateFilter"></param>
        /// <param name="appUserFilter"></param>
        /// <param name="sortOption"></param>
        /// <param name="memberFilter"></param>
        /// <returns></returns>
        public static MvcHtmlString GetUrlForItemsPerPage(this HtmlHelper helper, 
            string action, string controller, int? itemsPerPage=null, 
            int? id=null, int? memberId=null, int? ccgId=null, bool listAll=false, bool getAll=false, 
            string query=null, bool selectAll=false, bool allAccess=false, bool archive=false, int? dateFilter=null,
            int? appUserFilter=null, int? sortOption=0, int? memberFilter=null)
        {
            var sb = new StringBuilder();
            sb.Append("/" + controller + "/" + action);
            
            // Need one of these
            if (itemsPerPage == null && !listAll)
            {
                return new MvcHtmlString(sb.ToString());
            }

            // Example url: 
            // mydomain.com/Members/Index?itemsPerPage=25&id=3&getAll=true
            char prefix = '?';
            if (itemsPerPage != null)
            {
                sb.Append(prefix + "itemsPerPage=" + itemsPerPage);
                prefix = '&';
            }
            if (listAll)
            {
                sb.Append(prefix + "listAll=" + true);
                prefix = '&';
            }
            if (getAll)
            {
                sb.Append(prefix + "getAll=" + true);
            }
            if (id != null)
            {
                sb.Append(prefix + "id=" + id);
            }
            if (memberId != null)
            {
                sb.Append(prefix + "memberId=" + memberId);
            }
            if (ccgId != null)
            {
                sb.Append(prefix + "ccgId=" + ccgId);
            }
            if (query != null)
            {
                sb.Append(prefix + "query=" + query);
            }
            if (selectAll)
            {
                sb.Append(prefix + "selectAll=" + true);
            }
            if (allAccess)
            {
                sb.Append(prefix + "allAccess=" + true);
            }
            if (archive)
            {
                sb.Append(prefix + "isArchive=" + true);
            }
            if (dateFilter != null)
            {
                sb.Append(prefix + "DateRangeFilter=" + dateFilter);
            }
            if (appUserFilter != null)
            {
                sb.Append(prefix + "AppUserFilter=" + appUserFilter);
            }
            if (sortOption != null)
            {
                sb.Append(prefix + "sortOption=" + sortOption);
            }
            if (memberFilter != null)
            {
                sb.Append(prefix + "memberFilter=" + memberFilter);
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString MyCheckBox(this HtmlHelper helper, string id=null, bool isChecked=false, string labelId=null, string labelTextId = null, string labelClass=null, string labelText=null)
        {
            var sb = new StringBuilder();
            sb.Append("<label ");
            if (labelId != null)
                sb.Append("id=\"" + labelId + "\" ");
            if (labelClass != null)
                sb.Append("class=\"" + labelClass + "\" ");
            sb.Append("/>");
            sb.Append("<input ");
            sb.Append("type=\"checkbox\" ");
            if (id != null)
                sb.Append("id=" + id + " ");
            if (isChecked)
                sb.Append("checked=\"checked\"" + " ");
            sb.Append("/>");
            if (labelTextId != null)
                sb.Append("<span id=\"" + labelTextId + "\">");
            sb.Append(labelText);
            if (labelTextId != null)
                sb.Append("</span>");
            sb.Append("</label>");

            return new MvcHtmlString(sb.ToString());
        }
     
    }
}
