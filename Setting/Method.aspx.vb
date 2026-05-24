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

    <WebMethod()>
    Public Shared Function GetReads(dataId As String) As List(Of ReadDto)
        Dim settingClass As New SettingClass
        Dim dt As DataTable = settingClass.GetDataTable("SELECT Logins.FullName, Logins.UserName, NotificationLogs.ShowDate FROM NotificationLogs LEFT JOIN Logins ON NotificationLogs.LoginId=Logins.Id WHERE NotificationLogs.NotificationId = '" & dataId & "' ORDER BY NotificationLogs.ShowDate DESC")

        Dim result As New List(Of ReadDto)

        For Each r As DataRow In dt.Rows
            Dim fullName As String = r("FullName").ToString()
            Dim userName As String = r("UserName").ToString()
            Dim showDate As String = Convert.ToDateTime(r("ShowDate")).ToString("dd MMM yyyy")

            result.Add(New ReadDto With {
                .TextRead = "<b>" & userName & "</b> (" & fullName & ") on " & showDate
            })
        Next

        Return result
    End Function
End Class

Public Class LogDto
    Public Property TextLog As String
End Class

Public Class ReadDto
    Public Property TextRead As String
End Class