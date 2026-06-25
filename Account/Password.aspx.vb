Imports System.Data.SqlClient

Partial Class Account_Password
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtPassword.Text = "" Then
                MessageError(True, "PASSWORD IS REQUIRED !")
                txtPassword.BackColor = Drawing.Color.Red
                txtPassword.Focus()
                Exit Sub
            End If
            If txtConfirmPassword.Text = "" Then
                MessageError(True, "CONFIRM PASSWORD IS REQUIRED !")
                txtConfirmPassword.BackColor = Drawing.Color.Red
                txtConfirmPassword.Focus()
                Exit Sub
            End If
            If Not txtConfirmPassword.Text = txtPassword.Text Then
                txtConfirmPassword.BackColor = Drawing.Color.Red
                txtConfirmPassword.Focus()
                MessageError(True, "PASSWORD DO NOT MATCH !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim loginId As String = Session("LoginId").ToString()
                Dim newPassword As String = settingClass.Encrypt(txtPassword.Text)
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password, ResetLogin=0 WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", loginId)
                        thisCmd.Parameters.AddWithValue("@Password", newPassword)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Logins", loginId, Session("LoginId").ToString(), "Customer Change Password"}
                settingClass.Logs(dataLog)

                Dim thisScript As String = "window.onload = function() { showSuccess(); };"
                ClientScript.RegisterStartupScript(Me.GetType(), "showSuccess", thisScript, True)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub
End Class
