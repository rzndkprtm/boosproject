Imports System.IO
Imports System.Web.Services

Partial Class FileManager
    Inherits Page

    <WebMethod()>
    Public Shared Function GetFolders() As Object
        Dim rootPath As String = HttpContext.Current.Server.MapPath("~/File/Order")
        Dim result As New List(Of Object)

        If Directory.Exists(rootPath) Then
            For Each dir As String In Directory.GetDirectories(rootPath)
                result.Add(New With {.Name = Path.GetFileName(dir)})
            Next
        End If
        result = result.OrderBy(Function(x) x.Name).ToList()
        Return result
    End Function

    <WebMethod()>
    Public Shared Function GetFiles(folderName As String) As Object
        Dim folderPath As String = HttpContext.Current.Server.MapPath("~/File/Order/" & folderName)
        Dim result As New List(Of Object)
        If Directory.Exists(folderPath) Then
            For Each file As String In Directory.GetFiles(folderPath)
                Dim fileName As String = Path.GetFileName(file)
                result.Add(New With {.Name = fileName, .Url = "/File/Order/" & folderName & "/" & fileName})

            Next
        End If
        Return result
    End Function
End Class
