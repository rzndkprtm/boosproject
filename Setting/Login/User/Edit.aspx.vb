Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Login_User_Edit
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/login/user", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("loginid")) Then
            Response.Redirect("~/setting/login/user", False)
            Exit Sub
        End If

        If Session("LoginId") = Request.QueryString("loginid").ToString() Then
            Response.Redirect("~/setting/login/user", False)
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
            If ddlRole.SelectedValue = "" Then
                MessageError(True, "ROLE IS REQUIRED !")
                Exit Sub
            End If
            If ddlLevel.SelectedValue = "" Then
                MessageError(True, "LEVEL IS REQUIRED !")
                Exit Sub
            End If

            If Session("RoleName") = "IT" Then
                If ddlRole.SelectedValue = "1" Then
                    MessageError(True, "YOU DO NOT HAVE PERMISSION TO CREATE A LOGIN WITH THIS ROLE !")
                    Exit Sub
                End If

                If Session("LevelName") = "Member" AndAlso ddlRole.SelectedValue = "2" Then
                    MessageError(True, "YOU DO NOT HAVE PERMISSION TO CREATE A LOGIN WITH THIS ROLE !")
                    Exit Sub
                End If
            End If

            If Session("RoleName") = "Factory Office" Then
                If ddlRole.SelectedValue = "1" OrElse ddlRole.SelectedValue = "2" Then
                    MessageError(True, "YOU DO NOT HAVE PERMISSION TO CREATE A LOGIN WITH THIS ROLE !")
                    Exit Sub
                End If

                If Session("LevelName") = "Member" AndAlso ddlRole.SelectedValue = "3" Then
                    MessageError(True, "YOU DO NOT HAVE PERMISSION TO CREATE A LOGIN WITH THIS ROLE !")
                    Exit Sub
                End If
            End If

            If txtEditUserName.Text = "" Then
                MessageError(True, "USERNAME IS REQUIRED !")
                Exit Sub
            End If
            If Not Regex.IsMatch(txtEditUserName.Text, "^[a-zA-Z0-9._-]+$") Then
                MessageError(True, "INVALID USERNAME. ONLY LETTERS, NUMBERS, DOT (.), UNDERSCRORE (_) & HYPHEN (-) ARE ALLOWED !")
                Exit Sub
            End If
            Dim checkUsername As String = settingClass.GetItemData("SELECT UserName FROM Logins WHERE UserName='" + txtEditUserName.Text + "'")
            If txtEditUserName.Text = checkUsername Then
                MessageError(True, "USERNAME ALREADY EXIST !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET CustomerId=@CustomerId, RoleId=@RoleId, LevelId=@LevelId, UserName=@UserName, FullName=@FullName, Email=@Email WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@CustomerId", If(String.IsNullOrEmpty(ddlCustomer.SelectedValue), CType(DBNull.Value, Object), ddlCustomer.SelectedValue))
                        myCmd.Parameters.AddWithValue("@RoleId", ddlRole.SelectedValue)
                        myCmd.Parameters.AddWithValue("@LevelId", ddlLevel.SelectedValue)
                        myCmd.Parameters.AddWithValue("@UserName", txtEditUserName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Pricing", ddlPricing.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Logins", lblId.Text, Session("LoginId").ToString(), "Login Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/login/user", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/login/user", False)
    End Sub

    Protected Sub BindData(loginId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Logins WHERE Id='" & loginId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/login/user", False)
                Exit Sub
            End If

            Dim roleId As String = myData("RoleId").ToString()
            If Session("RoleName") = "IT" Then
                If roleId = "1" Then
                    Response.Redirect("~/setting/login/user", False)
                    Exit Sub
                End If

                If Session("LevelName") = "Member" AndAlso (roleId = "1" OrElse roleId = "2") Then
                    Response.Redirect("~/setting/login/user", False)
                    Exit Sub
                End If
            End If

            If Session("RoleName") = "Factory Office" Then
                If roleId = "1" OrElse roleId = "2" Then
                    Response.Redirect("~/setting/login/user", False)
                    Exit Sub
                End If

                If Session("LevelName") = "Member" AndAlso (roleId = "1" OrElse roleId = "2" OrElse roleId = "3") Then
                    Response.Redirect("~/setting/login/user", False)
                    Exit Sub
                End If
            End If

            BindRole()
            BindLeve()
            BindCustomer()

            ddlRole.SelectedValue = myData("RoleId").ToString()
            ddlLevel.SelectedValue = myData("LevelId").ToString()
            ddlCustomer.SelectedValue = myData("CustomerId").ToString()
            txtEditUserName.Text = myData("UserName").ToString()
            lblUserName.Text = myData("UserName").ToString()
            txtFullName.Text = myData("FullName").ToString()
            txtEmail.Text = myData("Email").ToString()
            ddlPricing.SelectedValue = Convert.ToInt32(myData("Pricing"))
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindRole()
        ddlCustomer.Items.Clear()
        Try
            ddlRole.DataSource = settingClass.GetDataTable("SELECT * FROM LoginRoles WHERE Active=1 ORDER BY Name ASC")
            ddlRole.DataTextField = "Name"
            ddlRole.DataValueField = "Id"
            ddlRole.DataBind()

            If ddlRole.Items.Count > 1 Then
                ddlRole.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlRole.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindLeve()
        ddlLevel.Items.Clear()
        Try
            ddlLevel.DataSource = settingClass.GetDataTable("SELECT * FROM LoginLevels WHERE Active=1 ORDER BY Name ASC")
            ddlLevel.DataTextField = "Name"
            ddlLevel.DataValueField = "Id"
            ddlLevel.DataBind()

            If ddlLevel.Items.Count > 1 Then
                ddlLevel.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlLevel.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindCustomer()
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT * FROM Customers WHERE Active=1 ORDER BY Name ASC")
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
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
