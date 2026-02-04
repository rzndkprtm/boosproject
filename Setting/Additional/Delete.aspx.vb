Imports System.IO

Partial Class Setting_Additional_Delete
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/additional", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            DeleteAction()
        End If
    End Sub

    Protected Sub DeleteAction()
        Try
            Dim directoryArchive As New DirectoryInfo(Server.MapPath("~/File/Archive/"))
            EmptyFolder(directoryArchive)

            Dim directoryCws As New DirectoryInfo(Server.MapPath("~/File/CWS/"))
            EmptyFolder(directoryCws)

            Dim directoryInv As New DirectoryInfo(Server.MapPath("~/File/Invoice/"))
            EmptyFolder(directoryInv)

            Dim directoryOrder As New DirectoryInfo(Server.MapPath("~/File/Preview/"))
            EmptyFolder(directoryOrder)

            Dim directoryQuote As New DirectoryInfo(Server.MapPath("~/File/Quote/"))
            EmptyFolder(directoryQuote)

            Dim directoryReport As New DirectoryInfo(Server.MapPath("~/File/Report/"))
            EmptyFolder(directoryReport)

            Dim directorySuratJalan As New DirectoryInfo(Server.MapPath("~/File/SuratJalan/"))
            EmptyFolder(directorySuratJalan)

            Dim directoryRework As New DirectoryInfo(Server.MapPath("~/File/Rework/"))
            EmptyFolder_Rework(directoryRework)

            Response.Redirect("~/", False)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub EmptyFolder(directory As DirectoryInfo)
        For Each file As FileInfo In directory.GetFiles()
            If Not file.Name.Equals("README.txt", StringComparison.OrdinalIgnoreCase) Then
                file.Delete()
            End If
        Next

        For Each subDir As DirectoryInfo In directory.GetDirectories()
            subDir.Delete(True)
        Next
    End Sub

    Protected Sub EmptyFolder_Rework(directory As DirectoryInfo)
        For Each file As FileInfo In directory.GetFiles()
            If file.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase) Then
                file.Delete()
            End If
        Next

        For Each subDir As DirectoryInfo In directory.GetDirectories()
            subDir.Delete(True)
        Next
    End Sub

    Protected Function PageAction(Action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, Action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
