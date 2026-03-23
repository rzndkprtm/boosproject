Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Address
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
            txtSearch.Text = Session("SearchCustomerAddress")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchCustomerAddress") = txtSearch.Text
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Address"

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
                Session("SearchCustomerAddress") = txtSearch.Text
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Address"

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerAddress WHERE Id='" & dataId & "'")
                    If thisData Is Nothing Then Exit Sub

                    BindDataCustomer()

                    ddlCustomer.SelectedValue = thisData("CustomerId").ToString()
                    txtDescription.Text = thisData("Description").ToString()
                    txtAddress.Text = thisData("Address").ToString()
                    txtSuburb.Text = thisData("Suburb").ToString()
                    txtState.Text = thisData("State").ToString()
                    txtPostCode.Text = thisData("PostCode").ToString()
                    ddlCountry.SelectedValue = thisData("Country").ToString()
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

            If txtAddress.Text = "" Then
                MessageError_Process(True, "ADDRESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtSuburb.Text = "" Then
                MessageError_Process(True, "SUBURB IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtState.Text = "" Then
                MessageError_Process(True, "STATE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtPostCode.Text = "" Then
                MessageError_Process(True, "POST CODE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlCountry.SelectedValue = "" Then
                MessageError_Process(True, "COUNTRY IS REQUIRED !")
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
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerAddress ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerAddress VALUES (@Id, @CustomerId, @Description, @Address, @Suburb, @State, @PostCode, @Country, @Tags, @Note, 0)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                            myCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Suburb", txtSuburb.Text.Trim())
                            myCmd.Parameters.AddWithValue("@State", txtState.Text.Trim())
                            myCmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Country", ddlCountry.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtNote.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"CustomerAddress", thisId, Session("LoginId").ToString(), "Customer Address Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerAddress") = txtSearch.Text
                    Response.Redirect("~/setting/customer/address", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET CustomerId=@CustomerId, Description=@Description, Address=@Address, Suburb=@Suburb, State=@State, PostCode=@PostCode, Country=@Country, Tags=@Tags, Note=@Note WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                            myCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Suburb", txtSuburb.Text.Trim())
                            myCmd.Parameters.AddWithValue("@State", txtState.Text.Trim())
                            myCmd.Parameters.AddWithValue("@PostCode", txtPostCode.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Country", ddlCountry.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtNote.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"CustomerAddress", lblId.Text, Session("LoginId"), "Customer Address Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchCustomerAddress") = txtSearch.Text
                    Response.Redirect("~/setting/customer/address", False)
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

            Dim customerId As String = settingClass.GetItemData("SELECT CustomerId FROM CustomerAddress WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET [Primary]=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", customerId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET Tags='Delivery,Warehouse', [Primary]=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"CustomerAddress", thisId, Session("LoginId"), "Reset As Primary"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerAddress") = txtSearch.Text
            Response.Redirect("~/setting/customer/address", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Dim fullAddress As String = settingClass.GetItemData("SELECT CONCAT('Description: ', ISNULL(Description, ''), ', ', 'Address: ', ISNULL(Address, ''), ', ', 'Suburb: ', ISNULL(Suburb, ''), ', ', 'State: ', ISNULL(State, ''), ', ', 'PostCode: ', ISNULL(PostCode, '')) AS FullDescription FROM CustomerAddress WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerAddress WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerAddress' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Dim stringLog As String = String.Format("Customer Address Deleted | {0}", fullAddress)
            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), stringLog}
            settingClass.Logs(dataLog)

            Session("SearchCustomerAddress") = txtSearch.Text
            Response.Redirect("~/setting/customer/address", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerAddress") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                search = "WHERE Customers.Name LIKE '%" & searchText & "%' OR Customers.DebtorCode LIKE '%" & searchText & "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT CustomerAddress.*, Customers.Name AS CustomerName, CASE WHEN CustomerAddress.[Primary]=1 THEN 'Yes' WHEN CustomerAddress.[Primary]=0 THEN 'No' ELSE 'Error' END AS DataPrimary FROM CustomerAddress LEFT JOIN Customers ON CustomerAddress.CustomerId=Customers.Id {0} ORDER BY Customers.Name, CustomerAddress.Id ASC", search)

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

    Protected Function BindDetailAddress(addressId As String) As String
        Dim result As String = String.Empty

        If Not String.IsNullOrEmpty(addressId) Then
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerAddress WHERE Id='" & addressId & "'")

            If thisData IsNot Nothing Then
                Dim address As String = thisData("Address").ToString()
                Dim suburb As String = thisData("Suburb").ToString()
                Dim state As String = thisData("State").ToString()
                Dim postCode As String = thisData("PostCode").ToString()

                result = address & ", " & suburb & ", " & state & " " & postCode
            End If
        End If
        Return result
    End Function

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
