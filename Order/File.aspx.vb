Imports System.IO
Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Globalization

Partial Class Order_File
    Inherits Page

    Private Shared ReadOnly RootFolder As String = "~/File/Order/"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            ' Initial page load.
        End If
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function

    Public Class FileItem
        Public Property Name As String
        Public Property RelativePath As String
        Public Property IsDirectory As Boolean
        Public Property Extension As String
        Public Property Size As Long
        Public Property Modified As String
    End Class

    Public Class FileListResult
        Public Property Success As Boolean
        Public Property Message As String
        Public Property CurrentPath As String
        Public Property Files As List(Of FileItem)
    End Class

    Private Shared Function GetRootPhysicalPath() As String
        Return Path.GetFullPath(HttpContext.Current.Server.MapPath(RootFolder))
    End Function

    Private Shared Function ResolveSafePath(relativePath As String) As String
        Dim rootPath As String = GetRootPhysicalPath()
        Dim normalizedRoot As String = Path.GetFullPath(rootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) & Path.DirectorySeparatorChar

        If String.IsNullOrWhiteSpace(relativePath) Then
            Return rootPath
        End If

        Dim normalizedRelative As String = relativePath.Replace("/"c, Path.DirectorySeparatorChar).Replace("\"c, Path.DirectorySeparatorChar)

        If Path.IsPathRooted(normalizedRelative) Then
            Throw New UnauthorizedAccessException("Invalid path.")
        End If

        Dim fullPath As String = Path.GetFullPath(Path.Combine(rootPath, normalizedRelative))

        If Not fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase) AndAlso Not String.Equals(fullPath.TrimEnd(Path.DirectorySeparatorChar), rootPath.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then
            Throw New UnauthorizedAccessException("Access denied.")
        End If

        Return fullPath
    End Function

    Private Shared Function GetRelativePath(fullPath As String) As String
        Dim rootPath As String = GetRootPhysicalPath().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        Dim normalizedFullPath As String = Path.GetFullPath(fullPath)

        If String.Equals(normalizedFullPath, rootPath, StringComparison.OrdinalIgnoreCase) Then
            Return String.Empty
        End If

        Return normalizedFullPath.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace("\"c, "/"c)
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetFiles(path As String) As FileListResult
        Dim result As New FileListResult With {
            .Success = False,
            .Message = String.Empty,
            .CurrentPath = String.Empty,
            .Files = New List(Of FileItem)()
        }

        Try
            Dim physicalPath As String = ResolveSafePath(path)

            If Not Directory.Exists(physicalPath) Then
                result.Message = "The requested folder does not exist."
                Return result
            End If

            result.CurrentPath = GetRelativePath(physicalPath)

            For Each directoryPath As String In Directory.GetDirectories(physicalPath)
                Dim directoryInfo As New DirectoryInfo(directoryPath)

                result.Files.Add(New FileItem With {
                    .Name = directoryInfo.Name,
                    .RelativePath = GetRelativePath(directoryPath),
                    .IsDirectory = True,
                    .Extension = String.Empty,
                    .Size = 0,
                    .Modified = directoryInfo.LastWriteTime.ToString("dd MMM yyyy HH:mm", CultureInfo.InvariantCulture)
                })
            Next

            For Each filePath As String In Directory.GetFiles(physicalPath)
                Dim fileInfo As New FileInfo(filePath)

                result.Files.Add(New FileItem With {
                    .Name = fileInfo.Name,
                    .RelativePath = GetRelativePath(filePath),
                    .IsDirectory = False,
                    .Extension = fileInfo.Extension,
                    .Size = fileInfo.Length,
                    .Modified = fileInfo.LastWriteTime.ToString("dd MMM yyyy HH:mm", CultureInfo.InvariantCulture)
                })
            Next

            result.Success = True

        Catch ex As UnauthorizedAccessException
            result.Message = "You do not have permission to access this folder."
        Catch ex As Exception
            result.Message = "Unable to read the requested folder."
        End Try

        Return result
    End Function

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        ' No additional rendering required.
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim action As String = Convert.ToString(Request.QueryString("action"))

        If String.Equals(action, "download", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(action, "view", StringComparison.OrdinalIgnoreCase) Then
            ReturnFile(action)
            Return
        End If

        MyBase.Render(writer)
    End Sub

    Private Sub ReturnFile(action As String)
        Try
            Dim relativePath As String = Convert.ToString(Request.QueryString("path"))
            Dim physicalPath As String = ResolveSafePath(relativePath)

            If Not File.Exists(physicalPath) Then
                Response.StatusCode = 404
                Response.TrySkipIisCustomErrors = True
                Context.ApplicationInstance.CompleteRequest()
                Return
            End If

            Dim fileInfo As New FileInfo(physicalPath)
            Dim extension As String = fileInfo.Extension.ToLowerInvariant()

            If action = "view" Then
                Dim allowedExtensions As String() = {".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg", ".txt"}

                If Not allowedExtensions.Contains(extension) Then
                    Response.StatusCode = 403
                    Response.TrySkipIisCustomErrors = True
                    Context.ApplicationInstance.CompleteRequest()
                    Return
                End If

                Response.Clear()
                Response.ContentType = GetContentType(extension)
                Response.AddHeader("Content-Disposition", "inline; filename=""" & fileInfo.Name.Replace("""", String.Empty) & """")
                Response.AddHeader("X-Content-Type-Options", "nosniff")
                Response.TransmitFile(physicalPath)
                Context.ApplicationInstance.CompleteRequest()
                Return
            End If

            Response.Clear()
            Response.ContentType = "application/octet-stream"
            Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileInfo.Name.Replace("""", String.Empty) & """")
            Response.AddHeader("Content-Length", fileInfo.Length.ToString(CultureInfo.InvariantCulture))
            Response.TransmitFile(physicalPath)
            Context.ApplicationInstance.CompleteRequest()

        Catch ex As UnauthorizedAccessException
            Response.StatusCode = 403
            Response.TrySkipIisCustomErrors = True
            Context.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            Response.StatusCode = 500
            Response.TrySkipIisCustomErrors = True
            Context.ApplicationInstance.CompleteRequest()
        End Try
    End Sub

    Private Shared Function GetContentType(extension As String) As String
        Select Case extension
            Case ".pdf"
                Return "application/pdf"
            Case ".jpg", ".jpeg"
                Return "image/jpeg"
            Case ".png"
                Return "image/png"
            Case ".gif"
                Return "image/gif"
            Case ".webp"
                Return "image/webp"
            Case ".bmp"
                Return "image/bmp"
            Case ".svg"
                Return "image/svg+xml"
            Case ".txt"
                Return "text/plain; charset=utf-8"
            Case Else
                Return "application/octet-stream"
        End Select
    End Function
End Class
