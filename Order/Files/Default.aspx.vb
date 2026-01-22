Imports System.Data
Imports System.IO

Partial Class Order_File_Default
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadRootFolders()
        End If
    End Sub

    Private Sub LoadRootFolders(Optional keyword As String = "")
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


    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        LoadRootFolders(txtSearch.Text)
    End Sub
End Class
