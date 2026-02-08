Imports System.Data.SqlClient

Partial Class Account_Password
    Inherits Page    

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BackColor()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
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
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Password=@Password, ResetLogin=0 WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", loginId)
                        myCmd.Parameters.AddWithValue("@Password", newPassword)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerLogins", loginId, Session("LoginId").ToString(), "Customer Change Password"}
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "Surcharge", "btnSubmit_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BackColor()
        MessageError(False, String.Empty)

        txtPassword.BackColor = Drawing.Color.Empty
        txtConfirmPassword.BackColor = Drawing.Color.Empty
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub
End Class
