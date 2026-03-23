Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Contact
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
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
                    ddlSalutation.SelectedValue = thisData("Salutation").ToString()
                    txtRole.Text = thisData("Role").ToString()
                    txtEmail.Text = thisData("Email").ToString()
                    txtPhone.Text = thisData("Phone").ToString()
                    txtMobile.Text = thisData("Mobile").ToString()
                    txtFax.Text = thisData("Fax").ToString()
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
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerContacts VALUES (@Id, @CustomerId, @Name, @Salutation, @Role, @Email, @Phone, @Mobile, @Fax, @Tags, @Note, 0)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Salutation", ddlSalutation.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Role", txtRole.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Fax", txtFax.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtNote.Text.Trim())

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
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET CustomerId=@CustomerId, Name=@Name, Salutation=@Salutation, Role=@Role, Email=@Email, Phone=@Phone, Mobile=@Mobile, Fax=@Fax, Tags=@Tags, Note=@Note WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Salutation", ddlSalutation.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Role", txtRole.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Fax", txtFax.Text.Trim())
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
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnPrimary_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdPrimary.Text

            Dim customerId As String = settingClass.GetItemData("SELECT CustomerId FROM CustomerContacts WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET [Primary]=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", customerId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET Tags='Confirming,Invoicing,Quoting,Newsletter', [Primary]=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"CustomerContacts", thisId, Session("LoginId"), "Set As Primary"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerContact") = txtSearch.Text
            Response.Redirect("~/setting/customer/contact", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerContacts WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerContacts' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("SearchCustomerContact") = txtSearch.Text
            Response.Redirect("~/setting/customer/contact", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerContact") = String.Empty
        Try
            Dim search As String = String.Empty

            If Not String.IsNullOrEmpty(searchText) Then
                search = "WHERE Customers.Id LIKE '%" & searchText & "%' OR Customers.Name LIKE '%" & searchText & "%' OR Customers.DebtorCode LIKE '%" & searchText & "%' OR CustomerContacts.Name LIKE '%" & searchText & "%' OR CustomerContacts.Email LIKE '%" & searchText & "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT CustomerContacts.*, Customers.Name AS CustomerName, CONVERT(VARCHAR, CustomerContacts.Salutation) + ' ' + CONVERT(VARCHAR, CustomerContacts.Name) AS ContactName, CASE WHEN CustomerContacts.[Primary]=1 THEN 'Yes' WHEN CustomerContacts.[Primary]=0 THEN 'No' ELSE 'Error' END AS DataPrimary FROM CustomerContacts LEFT JOIN Customers ON CustomerContacts.CustomerId=Customers.Id {0} ORDER BY Customers.Id, CustomerContacts.Id ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindDataCustomer()
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