Imports System.Data
Imports System.IO
Imports System.IO.Compression

Partial Class Order_File
    Inherits Page

    Dim mailingClass As New MailingClass
    Dim dataMailing As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindFolder()
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        BindFolder(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindFolder(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)

                Dim folderName As String = e.CommandArgument.ToString()
                Dim thisScript As String = "window.onload = function() { showDetailFile(); };"
                Try
                    BindFiles(folderName)

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
            ElseIf e.CommandName = "Zip" Then
                Dim folderName As String = e.CommandArgument.ToString()
                DownloadFolderAsZip(folderName)
            End If
        End If
    End Sub

    Protected Sub BindFolder(Optional search As String = "")
        Try
            Dim path As String = Server.MapPath("~/File/Order")
            Dim dt As New DataTable
            dt.Columns.Add("FolderName")
            dt.Columns.Add("FileCount", GetType(Integer))

            If Directory.Exists(path) Then
                For Each dir As String In Directory.GetDirectories(path)
                    Dim folderName As String = New DirectoryInfo(dir).Name

                    If String.IsNullOrEmpty(search) OrElse
                   folderName.ToLower().Contains(search.ToLower()) Then

                        Dim files = Directory.GetFiles(dir)
                        Dim row = dt.NewRow()
                        row("FolderName") = folderName
                        row("FileCount") = files.Length
                        dt.Rows.Add(row)
                    End If
                Next
            End If

            gvList.DataSource = dt
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindFiles(folderName As String)
        MessageError_DetailFile(False, String.Empty)
        Try
            Dim path As String = Server.MapPath("~/File/Order/" & folderName)

            Dim dt As New DataTable
            dt.Columns.Add("FolderName")
            dt.Columns.Add("FileName")
            dt.Columns.Add("FileUrl")

            If Directory.Exists(path) Then
                For Each file As String In Directory.GetFiles(path)
                    Dim fileName = IO.Path.GetFileName(file)
                    Dim row = dt.NewRow()

                    row("FolderName") = folderName
                    row("FileName") = fileName
                    row("FileUrl") = ResolveUrl("~/File/Order/" & folderName & "/" & fileName)
                    dt.Rows.Add(row)
                Next
            End If

            gvListDetail.DataSource = dt
            gvListDetail.DataBind()
        Catch ex As Exception
            MessageError_DetailFile(True, ex.ToString())
        End Try
    End Sub

    Private Sub DownloadFolderAsZip(folderName As String)
        Dim folderPath As String = Server.MapPath("~/File/Order/" & folderName)

        If Not Directory.Exists(folderPath) Then Exit Sub

        Using ms As New MemoryStream()
            Using zip As New ZipArchive(ms, ZipArchiveMode.Create, True)
                For Each filePath In Directory.GetFiles(folderPath)
                    Dim entryName As String = Path.GetFileName(filePath)

                    Dim entry = zip.CreateEntry(entryName, CompressionLevel.Fastest)
                    Using entryStream = entry.Open()
                        Using fileStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                            fileStream.CopyTo(entryStream)
                        End Using
                    End Using
                Next
            End Using

            Dim zipBytes As Byte() = ms.ToArray()

            Response.Clear()
            Response.ContentType = "application/zip"
            Response.AddHeader("Content-Disposition", "attachment; filename=""" & folderName & ".zip""")
            Response.BinaryWrite(zipBytes)
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Using
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_DetailFile(visible As Boolean, message As String)
        divErrorDetailFile.Visible = visible : msgErrorDetailFile.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
