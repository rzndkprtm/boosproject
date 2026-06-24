Imports System.Data
Imports System.Data.SqlClient

Partial Class Account_Forgot
    Inherits Page    

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSend_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If String.IsNullOrEmpty(txtUserLogin.Text) Then
                MessageError(True, "USERNAME IS REQUIRED !")
                Exit Sub
            End If
            If String.IsNullOrEmpty(txtEmail.Text) Then
                MessageError(True, "EMAIL ADDRESS IS REQUIRED !")
                Exit Sub
            End If

            Dim checkEmail As String = IsValidEmailAddress(txtEmail.Text.Trim())

            If checkEmail = False Then
                MessageError(True, "INVALID EMAIL ADDRESS !")
                Exit Sub
            End If

            Dim checkData As DataRow = settingClass.GetDataRow("SELECT * FROM Logins WHERE UserName='" & txtUserLogin.Text.Trim() & "' AND Email='" & txtEmail.Text.Trim() & "'")
            If checkData Is Nothing Then
                MessageError(True, "SYSTEM ERROR: NO DATA RETURNED.")
                Exit Sub
            End If

            Dim loginId As String = checkData("Id").ToString()

            Dim newPassword As String = settingClass.GenerateNewPassword(15)
            Dim encryptPassword As String = settingClass.Encrypt(newPassword)

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password, ResetLogin=1, FailedCount=0, Active=1 WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", loginId)
                    thisCmd.Parameters.AddWithValue("@Password", encryptPassword)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"Logins", loginId, loginId, "Customer Forgot Password"}
            settingClass.Logs(dataLog)

            mailingClass.ResetPassword(loginId, newPassword)

            Response.Redirect("~/", False)
        Catch ex As Exception
            MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
        End Try
    End Sub

    Protected Function IsValidEmailAddress(email As String) As Boolean
        Try
            Dim m As New Net.Mail.MailAddress(email)
            Return m.Address = email
        Catch ex As Exception
            Return False
        End Try
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub
End Class
