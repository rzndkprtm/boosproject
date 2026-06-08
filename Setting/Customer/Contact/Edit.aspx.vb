Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Contact_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/contact/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("contactid")) Then
            Response.Redirect("~/setting/customer/contact/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("contactid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
            MessageError(False, String.Empty)
            Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOMER IS REQURIED !")
                Exit Sub
            End If
            If txtName.Text = "" Then
                MessageError(True, "NAME IS REQURIED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisTags As String = String.Empty
                Dim selected As String = String.Empty
                For Each item As ListItem In lbTags.Items
                    If item.Selected Then
                        selected += item.Text & ","
                    End If
                Next

                If Not selected = "" Then
                    thisTags = selected.Remove(selected.Length - 1).ToString()
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET CustomerId=@CustomerId, Name=@Name, Email=@Email, Phone=@Phone, Tags=@Tags, Note=@Note WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Tags", thisTags)
                        myCmd.Parameters.AddWithValue("@Note", txtNote.Text.Trim())

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerContacts", lblId.Text, Session("LoginId"), "Customer Contact Updated"}
                settingClass.Logs(dataLog)

                url = "~/setting/customer/contact"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/customer/contact"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(contactId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerContacts WHERE Id='" & contactId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/customer/contact/", False)
                Exit Sub
            End If

            BindCustomer()

            ddlCustomer.SelectedValue = thisData("CustomerId").ToString()
            txtName.Text = thisData("Name").ToString()
            txtEmail.Text = thisData("Email").ToString()
            txtPhone.Text = thisData("Phone").ToString()
            txtNote.Text = thisData("Note").ToString()

            Dim tagsArray() As String = thisData("Tags").ToString().Split(",")
            Dim tagsList As List(Of String) = tagsArray.ToList()

            For Each i In tagsArray
                If Not (i.Equals(String.Empty)) Then
                    lbTags.Items.FindByValue(i).Selected = True
                End If
            Next

            ddlCustomer.Enabled = False
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCustomer()
        ddlCustomer.Items.Clear()
        Try
            Dim role As String = String.Empty
            If Session("RoleName") = "Sales" Then
                role = "AND CompanyId='" & Session("CompanyId").ToString() & "'"
                If Session("LevelName") = "Member" Then
                    role = "AND (Id = '" & Session("CustomerId") & "' OR EXISTS (SELECT 1 FROM STRING_SPLIT(Operator, ',') WHERE value = '" & Session("LoginId") & "'))"
                End If
            End If

            Dim thisQuery As String = String.Format("SELECT * FROM Customers WHERE Active=1 {0} ORDER BY Name ASC", role)

            ddlCustomer.DataSource = settingClass.GetDataTable(thisQuery)
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
