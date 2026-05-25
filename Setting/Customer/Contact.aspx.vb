Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Contact
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerContact")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchCustomerContact") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Contact"

            BindDataCustomer()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Session("SearchCustomerContact") = txtSearch.Text
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Contact"

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerContacts WHERE Id='" & dataId & "'")
                    If thisData Is Nothing Then Exit Sub

                    BindDataCustomer()

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

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError_Process(True, "CUSTOMER ACCOUNT IS REQURIED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtName.Text = "" Then
                MessageError_Process(True, "CONTACT NAME IS REQURIED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
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

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerContacts ORDER BY Id DESC")
                    Dim checkData As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerContacts WHERE CustomerId='" & ddlCustomer.SelectedValue & "'")
                    Dim primaryData As Integer = 0
                    If checkData = 0 AndAlso Not String.IsNullOrEmpty(txtEmail.Text) Then
                        thisTags = "Confirming,Invoicing,Quoting,Newsletter"
                        primaryData = 1
                    End If

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerContacts VALUES (@Id, @CustomerId, @Name, @Email, @Phone, @Tags, @Note, @Primary)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtNote.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Primary", primaryData)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"CustomerContacts", thisId, Session("LoginId").ToString(), "Customer Contact Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerContact") = txtSearch.Text
                    Response.Redirect("~/setting/customer/contact", False)
                End If

                If lblAction.Text = "Edit" Then
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

                    dataLog = {"CustomerContacts", lblId.Text, Session("LoginId"), "Customer Contact Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerContact") = txtSearch.Text
                    Response.Redirect("~/setting/customer/contact", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnPrimary_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdPrimary.Text

            Dim customerId As String = settingClass.GetItemData("SELECT CustomerId FROM CustomerContacts WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET [Primary]=0 WHERE CustomerId=@CustomerId; UPDATE CustomerContacts SET Tags='Confirming,Invoicing,Quoting,Newsletter', [Primary]=1 WHERE Id=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@CustomerId", customerId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"CustomerContacts", thisId, Session("LoginId"), "Set As Primary"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerContact") = txtSearch.Text
            Response.Redirect("~/setting/customer/contact", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Dim fullContact As String = settingClass.GetItemData("SELECT CONCAT('Contact Name: ', ISNULL(Name, ''), ', ', 'Email: ', ISNULL(Email, ''), ', ', 'Tags: ', ISNULL(Tags, '')) AS ThisContact FROM CustomerContacts WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerContacts WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerContacts' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim stringLog As String = String.Format("Customer Contact Deleted | {0}", fullContact)
            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), stringLog}
            settingClass.Logs(dataLog)

            Session("SearchCustomerContact") = txtSearch.Text
            Response.Redirect("~/setting/customer/contact", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerContact") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_GetCustomerContacts", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Customers ORDER BY Name ASC"
            If Session("RoleName") = "Account" Then
                thisString = "SELECT * FROM Customers WHERE CompanyId='" & Session("CompanyId").ToString() & "' ORDER BY Name ASC"
            End If
            If Session("RoleName") = "Sales" Then
                thisString = "SELECT * FROM Customers WHERE CompanyId='" & Session("CompanyId").ToString() & "' ORDER BY Name ASC"
                If Session("LevelName") = "Member" Then
                    thisString = "SELECT * FROM Customers CROSS APPLY STRING_SPLIT(Operator, ',') AS operatorArray WHERE CompanyId='" & Session("CompanyId").ToString() & "' AND operatorArray.VALUE='" & Session("LoginId").ToString() & "' ORDER BY Name ASC"
                End If
            End If

            ddlCustomer.DataSource = settingClass.GetDataTable(thisString)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Function VisiblePrimary(primary As Boolean) As Boolean
        If primary = False Then Return True
        Return False
    End Function

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