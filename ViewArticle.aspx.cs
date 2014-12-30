using System;
using System.Collections.Generic; 
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ViewArticle : System.Web.UI.Page
{
    public System.Text.StringBuilder imageContent = new System.Text.StringBuilder();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["ID"] == null) return;

        int ID = Convert.ToInt32(Request.QueryString["ID"]);
        News article = new News();
        NewsInfo articleInfo = article.getArticle(ID);

        labHeadline.InnerText = articleInfo.Headline;
        labSummary.InnerText = articleInfo.Summary;
        labContent.InnerHtml = articleInfo.Content;
          
        //show the image in the gallery
        List<ImageInfo> articleImages = articleInfo.getImageList();
        if(articleImages.Count >= 3)
			imgSummary.ImageUrl = "Service/ImageService.aspx?ID=" + articleImages[2].ID.ToString();
 
        ImageInfo imageinfo;
        for (int i = 3; i < articleImages.Count; i++)
        {
            imageinfo = articleImages[i]; 

            imageContent.Append("<div><a href='Service/ImageService.aspx?ID=");
            imageContent.Append(imageinfo.ID.ToString());
            imageContent.Append("' title='");
            imageContent.Append(imageinfo.Description);
            imageContent.Append("'><img src='Service/PreviewImageService.aspx?maxLength=199&ID=");
            imageContent.Append(imageinfo.ID.ToString());
            imageContent.Append("'");
            imageContent.Append(" /></a><p>");
            imageContent.Append(imageinfo.Description);
            imageContent.Append("</p></div>");
             
        }

        //show the attachment in the news details page
        List<FileInfo> articleAttachment = articleInfo.getFileList();
        System.Text.StringBuilder attachmentContent = new System.Text.StringBuilder();

        FileInfo fileInfo;

        for (int i = 0; i < articleAttachment.Count; i++)
        {
            if (i == 0)
            {
                attachmentContent.Append("<br/><p><u><b>Attachment: </b></u></p>");
            }
            fileInfo = articleAttachment[i];
            attachmentContent.Append("<a href='Service/FileService.aspx?ID=");
            attachmentContent.Append(fileInfo.ID);
            attachmentContent.Append("'>");
            attachmentContent.Append(fileInfo.Description);
            attachmentContent.Append("</a><br/>");
        }
        labAttachment.InnerHtml = attachmentContent.ToString();

        File file = new File(); 
        System.Text.StringBuilder newsLetters = file.getQuickLinkList(2);
        divNewsLetters.InnerHtml = newsLetters.ToString();



        this.ControlDataBind();
                
    }


    protected void ControlDataBind()
    {
        System.Data.DataTable EventType = SystemPara.getSystemPara("SuggestionType");

        foreach (System.Data.DataRow row in EventType.Rows)
        {
            this.comSuggestionType.Items.Add(new ListItem(row["Description"].ToString(), row["ID"].ToString()));
        }

    }

}
