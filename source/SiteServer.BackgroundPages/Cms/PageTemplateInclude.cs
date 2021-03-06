﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateInclude : BasePageCms
    {
		public DataGrid DgFiles;

        public string PublishmentSystemUrl => PublishmentSystemInfo.Additional.WebUrl;

	    private string _directoryPath;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateInclude), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _directoryPath = PathUtility.MapPath(PublishmentSystemInfo, "@/include");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "包含文件管理", AppManager.Permissions.WebSite.Template);

				if (Body.IsQueryExists("Delete"))
				{
                    var fileName = Body.GetQueryString("FileName");

					try
					{
                        FileUtils.DeleteFileIfExists(PathUtils.Combine(_directoryPath, fileName));
                        Body.AddSiteLog(PublishmentSystemId, "删除包含文件", $"包含文件:{fileName}");
						SuccessDeleteMessage();
					}
					catch(Exception ex)
					{
                        FailDeleteMessage(ex);
					}
				}
			}
			BindGrid();
		}

		public void BindGrid()
		{
            DirectoryUtils.CreateDirectoryIfNotExists(_directoryPath);
            var fileNames = DirectoryUtils.GetFileNames(_directoryPath);
            var fileNameArrayList = new ArrayList();
            foreach (var fileName in fileNames)
            {
                if (EFileSystemTypeUtils.IsTextEditable(EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(fileName))))
                {
                    if (!fileName.Contains("_parsed"))
                    {
                        fileNameArrayList.Add(fileName);
                    }
                }
            }
            DgFiles.DataSource = fileNameArrayList;
            DgFiles.DataBind();
        }

        public string GetCharset(string fileName)
        {
            var charset = FileUtils.GetFileCharset(PathUtils.Combine(_directoryPath, fileName));
            return ECharsetUtils.GetText(charset);
        }

	}
}
