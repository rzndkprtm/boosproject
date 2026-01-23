Imports System.Data
Imports System.IO

Partial Class Order_File
    Inherits Page

    Dim mailingClass As New MailingClass
    Dim dataMailing As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            LoadRootFolders()
        End If
    End Sub

    Protected Sub LoadRootFolders(Optional keyword As String = "")
        Dim dt As New DataTable
        dt.Columns.Add("FolderName")
        dt.Columns.Add("FullPath")

        Dim rootPath As String = Server.MapPath("~/File/Order")

        For Each dir As String In Directory.GetDirectories(rootPath)
            Dim folderName As String = Path.GetFileName(dir)

            If folderName.Equals("bin", StringComparison.OrdinalIgnoreCase) _
               OrElse folderName.Equals("app_data", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            If keyword <> "" AndAlso
               Not folderName.ToLower().Contains(keyword.ToLower()) Then
                Continue For
            End If

            dt.Rows.Add(folderName, dir)
        Next
        gvList.DataSource = dt
        gvList.DataBind()
    End Sub

    Protected Sub LoadFolderContent(folderName As String)
        Dim dt As New DataTable
        dt.Columns.Add("Type")
        dt.Columns.Add("Name")
        dt.Columns.Add("FullPath")
        dt.Columns.Add("Command")

        Dim folderPath As String = Server.MapPath(String.Format("~/File/Order/{0}", folderName))

        For Each file As String In Directory.GetFiles(folderPath)
            dt.Rows.Add("File", Path.GetFileName(file), file, "OpenFile")
        Next

        gvListDetail.DataSource = dt
        gvListDetail.DataBind()
    End Sub


    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        LoadRootFolders(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim folderName As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showDetailFile(); };"
                Try
                    LoadFolderContent(folderName)

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_DetailFile(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_DetailFile(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        If Session("RoleName") = "Customer" Then
                            MessageError_DetailFile(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                        End If
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetail_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_DetailFile(visible As Boolean, message As String)
        divErrorDetailFile.Visible = visible : msgErrorDetailFile.InnerText = message
    End Sub

    Protected Sub gvListDetail_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "OpenFile" Then
            Dim filePath As String = e.CommandArgument.ToString()
            OpenOrDownloadFile(filePath)
        End If
    End Sub

    Protected Sub OpenOrDownloadFile(filePath As String)
        If Not File.Exists(filePath) Then Exit Sub

        Dim ext As String = Path.GetExtension(filePath).ToLower()
        Dim fileName As String = Path.GetFileName(filePath)

        Dim previewExtensions As String() = {".pdf", ".jpg", ".jpeg", ".png", ".gif"}

        Response.Clear()
        Response.ContentType = GetContentType(ext)

        If previewExtensions.Contains(ext) Then
            ' Preview (open di tab baru)
            Response.AddHeader("Content-Disposition", "inline; filename=" & fileName)
        Else
            ' Download
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName)
        End If

        Response.TransmitFile(filePath)
        Response.End()
    End Sub

    Private Function GetContentType(ext As String) As String
        Select Case ext
            Case ".pdf" : Return "application/pdf"
            Case ".jpg", ".jpeg" : Return "image/jpeg"
            Case ".png" : Return "image/png"
            Case ".gif" : Return "image/gif"
            Case ".txt" : Return "text/plain"
            Case ".zip" : Return "application/zip"
            Case Else : Return "application/octet-stream"
        End Select
    End Function
End Class
