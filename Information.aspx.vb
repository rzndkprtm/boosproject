Imports System.Data

Partial Class Information
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim sessionId As String = String.Empty

        If Request.Cookies("deviceId") IsNot Nothing Then
            sessionId = Request.Cookies("deviceId").Value
            settingClass.DeleteSession(sessionId)
        End If

        Session.Clear()

        If String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        lblUid.Text = Request.QueryString("uid").ToString()

        If Not IsPostBack Then
            BindData(lblUid.Text)
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Response.Redirect("~/", False)
    End Sub

    Protected Sub BindData(temporaryId As String)
        Try
            Dim thisData As DataSet = settingClass.GetListData("SELECT * FROM Temporarys WHERE Id='" & temporaryId & "'")
            Dim thisUser As String = thisData.Tables(0).Rows(0).Item("UserName").ToString()

            If thisData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/", False)
                Exit Sub
            End If

            Dim finalData As DataSet = settingClass.GetListData("SELECT * FROM CustomerLogins WHERE UserName='" & thisUser & "'")

            If finalData.Tables(0).Rows.Count = 0 Then
                spanHi.InnerText = thisUser
                spanUser.InnerText = "Please contact the developer"
                spanPassword.InnerText = "Please contact the developer"
            Else
                spanHi.InnerText = finalData.Tables(0).Rows(0).Item("UserName").ToString()
                spanUser.InnerText = finalData.Tables(0).Rows(0).Item("UserName").ToString()
                spanPassword.InnerText = settingClass.Decrypt(finalData.Tables(0).Rows(0).Item("Password").ToString())
            End If

            Dim phone As String = "6285215043355"
            Dim message As String = "Hi Reza," & vbLf & "Saya " & spanHi.InnerText & ","

            Dim encodedMessage As String = Server.UrlEncode(message)

            lnkWhatsapp.HRef = "https://wa.me/" & phone & "?text=" & encodedMessage
        Catch ex As Exception
            Response.Redirect("~/", False)
        End Try
    End Sub
End Class
