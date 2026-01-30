Imports System.Data
Imports System.Web.Services

Partial Class Setting_Method
    Inherits Page

    <WebMethod()>
    Public Shared Function GetLogs(type As String, dataId As String) As List(Of LogDto)
        Dim settingClass As New SettingClass
        Dim dt As DataTable = settingClass.GetDataTable("SELECT * FROM Logs WHERE Type='" & type & "' AND DataId='" & dataId & "' ORDER BY ActionDate ASC")

        Dim result As New List(Of LogDto)

        For Each r As DataRow In dt.Rows
            result.Add(New LogDto With {
            .TextLog = settingClass.GetTextLog(r("Id").ToString())
        })
        Next

        Return result
    End Function
End Class

Public Class LogDto
    Public Property TextLog As String
End Class