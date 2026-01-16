Imports System.Data
Imports System.Data.SqlClient

Partial Class Account_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim loginId As String = String.Empty
    Dim dataMailing As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        loginId = Request.QueryString("uid").ToString()
        If Not Session("RoleName") = "Developer" Then
            If Not Session("LoginId") = loginId Then
                Response.Redirect("~/", False)
                Exit Sub
            End If
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Name(False, String.Empty)
            MessageError_Email(False, String.Empty)

            BindData(loginId)
        End If
    End Sub

    Protected Sub btnName_Click(sender As Object, e As EventArgs)
        MessageError_Name(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showName(); };"
        Try
            If msgErrorName.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET FullName=@FullName WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", loginId)
                        myCmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim url As String = String.Format("~/account?uid={0}", loginId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Name(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Name(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showName", thisScript, True)
        End Try
    End Sub

    Protected Sub btnEmail_Click(sender As Object, e As EventArgs)
        MessageError_Email(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showEmail(); };"
        Try
            If msgErrorEmail.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Email=@Email WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", loginId)
                        myCmd.Parameters.AddWithValue("@Email", txtUserEmail.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim url As String = String.Format("~/account?uid={0}", loginId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Email(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Email(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showEmail", thisScript, True)
        End Try
    End Sub

    Protected Sub BindData(loginId As String)
        Try
            'BIND LOGIN

            Dim loginData As DataSet = settingClass.GetListData("SELECT * FROM CustomerLogins WHERE Id='" & loginId & "'")

            lblUserName.Text = loginData.Tables(0).Rows(0).Item("UserName").ToString()
            lblFullName.Text = loginData.Tables(0).Rows(0).Item("FullName").ToString()
            txtFullName.Text = loginData.Tables(0).Rows(0).Item("FullName").ToString()
            lblUserEmail.Text = loginData.Tables(0).Rows(0).Item("Email").ToString()
            txtUserEmail.Text = loginData.Tables(0).Rows(0).Item("Email").ToString()

            Dim customerId As String = loginData.Tables(0).Rows(0).Item("CustomerId").ToString()

            ' BIND CUSTOMER
            Dim customerData As DataSet = settingClass.GetListData("SELECT * FROM Customers WHERE Id='" & customerId & "'")

            lblCustomerName.Text = customerData.Tables(0).Rows(0).Item("Name").ToString()

            gvContact.DataSource = settingClass.GetListData("SELECT *, CONVERT(VARCHAR, Salutation) + ' ' + CONVERT(VARCHAR, Name) AS ContactName FROM CustomerContacts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
            gvContact.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "PLEASE CONTACT YOUR CUSTOMER SERVICE !")
                End If
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Name(visible As Boolean, message As String)
        divErrorName.Visible = visible : msgErrorName.InnerText = message
    End Sub

    Protected Sub MessageError_Email(visible As Boolean, message As String)
        divErrorEmail.Visible = visible : msgErrorEmail.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
