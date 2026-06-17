Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Login_User_Installer_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim pageAccess As Boolean = LoginAccess("Load")
        'If pageAccess = False Then
        '    Response.Redirect("~/setting/login/user/installer", False)
        '    Exit Sub
        'End If

        If String.IsNullOrEmpty(Request.QueryString("loginid")) Then
            Response.Redirect("~/setting/login/user/installer", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("loginid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlLogin.SelectedValue = "" Then
                MessageError(True, "INSTALLER IS REQUIRED !")
                Exit Sub
            End If

            Dim customerId As String = String.Empty
            If Not lbCustomer.SelectedValue = "" Then
                Dim design As String = String.Empty
                For Each item As ListItem In lbCustomer.Items
                    If item.Selected Then
                        design += item.Value & ","
                    End If
                Next
                customerId = design.Remove(design.Length - 1).ToString()
            End If

            If String.IsNullOrEmpty(customerId) Then
                MessageError(True, "CUSTOMER ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE LoginInstallers SET CustomerId=@CustomerId WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", ddlLogin.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CustomerId", customerId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"LoginInstallers", ddlLogin.SelectedValue, Session("LoginId").ToString(), "Login Installer Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/login/user/installer", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/login/user/installer", False)
    End Sub

    Protected Sub BindData(loginId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM LoginInstallers WHERE Id='" & loginId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/login/user/installer", False)
                Exit Sub
            End If

            BindInstaller()
            BindCustomer()

            ddlLogin.SelectedValue = thisData("Id").ToString()
            If Not thisData("CustomerId").ToString() = "" Then
                Dim customerArray() As String = thisData("CustomerId").ToString().Split(",")
                For Each i In customerArray
                    If Not (i.Equals(String.Empty)) Then
                        lbCustomer.Items.FindByValue(i).Selected = True
                    End If
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindInstaller()
        ddlLogin.Items.Clear()
        Try
            ddlLogin.DataSource = settingClass.GetDataTable("SELECT * FROM Logins WHERE RoleId='10' AND Active=1 ORDER BY FullName ASC")
            ddlLogin.DataTextField = "FullName"
            ddlLogin.DataValueField = "Id"
            ddlLogin.DataBind()

            If ddlLogin.Items.Count > 1 Then
                ddlLogin.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlLogin.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindCustomer()
        lbCustomer.Items.Clear()
        Try
            lbCustomer.DataSource = settingClass.GetDataTable("SELECT * FROM Customers WHERE Active=1 AND CompanyDetailId='3' ORDER BY Name ASC")
            lbCustomer.DataTextField = "Name"
            lbCustomer.DataValueField = "Id"
            lbCustomer.DataBind()

            If lbCustomer.Items.Count > 1 Then
                lbCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
End Class
