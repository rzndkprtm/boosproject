Imports System.Data
Imports System.Data.SqlClient

Partial Class Account_Login
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            CheckSessionStates()
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtUserLogin.Text = "" Then
                MessageError(True, "USERNAME IS REQUIRED !")
                txtUserLogin.BackColor = Drawing.Color.Empty
                txtUserLogin.Focus()
                Exit Sub
            End If

            If txtPassword.Text = "" Then
                MessageError(True, "PASSWORD IS REQUIRED !")
                txtPassword.BackColor = Drawing.Color.Empty
                txtPassword.Focus()
                Exit Sub
            End If

            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerLogins WHERE UserName='" & txtUserLogin.Text & "'")

            If myData Is Nothing Then
                MessageError(True, "USERNAME NOT FOUND !")
                Exit Sub
            End If

            Dim loginId As String = myData("Id").ToString()
            Dim userName As String = myData("UserName").ToString()
            Dim password As String = myData("Password").ToString()
            Dim failedCount As Integer = myData("FailedCount")
            Dim loginActive As Boolean = myData("Active")

            If loginActive = False Then
                MessageError(True, "YOUR ACCOUNT (LOGIN) IS BEING BLOCKED !")
                Exit Sub
            End If

            If settingClass.Encrypt(txtPassword.Text) <> password Then
                UpdateFailedLogin(loginId)
                MessageError(True, "YOUR PASSWORD IS WRONG !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                CheckSessionStates()
                settingClass.UpdateSession(lblDeviceId.Text, loginId)
                settingClass.UpdateFailedCount(loginId)
                Session.Add("IsLoggedIn", True)
                Session.Add("LoginId", loginId)
                Session.Add("UserName", userName)

                Response.Redirect("~/", False)
            End If
        Catch ex As Exception
            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub UpdateFailedLogin(loginId As String)
        Using thisConn As New SqlConnection(myConn)
            Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET FailedCount=FailedCount+1 WHERE Id=@Id", thisConn)
                myCmd.Parameters.AddWithValue("@Id", loginId)

                thisConn.Open()
                myCmd.ExecuteNonQuery()
            End Using
        End Using

        Dim failedCount As String = settingClass.GetItemData("SELECT FailedCount FROM CustomerLogins WHERE Id='" & loginId & "'")
        If failedCount = "3" Then
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Active=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", loginId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
        End If
    End Sub

    Protected Sub CheckSessionStates()
        If Request.Cookies("deviceId") IsNot Nothing Then
            lblDeviceId.Text = Request.Cookies("deviceId").Value
            Dim checkSession As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM Sessions WHERE Id='" & lblDeviceId.Text & "'")
            If checkSession = 1 Then
                Dim loginId As String = settingClass.GetItemData("SELECT LoginId FROM Sessions WHERE Id='" & UCase(lblDeviceId.Text) & "'")
                If Not loginId = "" Then
                    Dim userName As String = settingClass.GetItemData("SELECT UserName FROM CustomerLogins WHERE Id='" & loginId & "'")

                    Session("IsLoggedIn") = True
                    Session("LoginId") = loginId
                    Session("UserName") = userName

                    Response.Redirect("~/", False)
                    HttpContext.Current.ApplicationInstance.CompleteRequest()
                    Exit Sub
                Else
                    lblDeviceId.Text = settingClass.InsertSession()
                    Dim deviceCookie As New HttpCookie("deviceId", lblDeviceId.Text)
                    deviceCookie.Expires = Now.AddMonths(1)
                    Response.Cookies.Add(deviceCookie)
                    Exit Sub
                End If
            Else
                lblDeviceId.Text = settingClass.InsertSession()
                Dim deviceCookie As New HttpCookie("deviceId", lblDeviceId.Text)
                deviceCookie.Expires = Now.AddMonths(1)
                Response.Cookies.Add(deviceCookie)
                Exit Sub
            End If
        Else
            lblDeviceId.Text = settingClass.InsertSession()
            Dim deviceCookie As New HttpCookie("deviceId", lblDeviceId.Text)
            deviceCookie.Expires = Now.AddMonths(1)
            Response.Cookies.Add(deviceCookie)
            Exit Sub
        End If
    End Sub
End Class
