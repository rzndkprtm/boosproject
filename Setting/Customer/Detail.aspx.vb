Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Web.Services

Partial Class Setting_Customer_Detail
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim url As String = String.Empty

    Dim dataLog As Object() = Nothing

    <WebMethod(EnableSession:=True)>
    Public Shared Sub UpdateSession(value As String)
        HttpContext.Current.Session("selectedTabCustomer") = value
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/list", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("customerid")) Then
            Response.Redirect("~/setting/customer/list", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("customerid").ToString()

        If Not Session("selectedTabCustomer") = "" Then
            selected_tab.Value = Session("selectedTabCustomer").ToString()
        End If

        If Not IsPostBack Then
            AllMessageError(False, String.Empty)
            BindData(lblId.Text)

            BindDataContact(lblId.Text)
            BindDataAddress(lblId.Text)
            BindDataBusiness(lblId.Text)
            BindDataLogin(lblId.Text)
            BindDataDiscount(lblId.Text)
            BindDataPromo(lblId.Text)
            BindDataProduct(lblId.Text)
            BindDataQuote(lblId.Text)

            secDetail.Visible = True
            If Session("CustomerId") = lblId.Text AndAlso (Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account") Then
                secDetail.Visible = False
            End If
        End If
    End Sub

    Protected Sub btnEditCustomer_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/customer/edit?customerid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnCreateOrder_Click(sender As Object, e As EventArgs)
        MessageError_CreateOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showCreateOrder(); };"
        Try
            If txtOrderNumber.Text = "" Then
                MessageError_CreateOrder(True, "ORDER NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If InStr(txtOrderNumber.Text, "\") > 0 Or InStr(txtOrderNumber.Text, "/") > 0 Or InStr(txtOrderNumber.Text, ",") > 0 Or InStr(txtOrderNumber.Text, "&") > 0 Or InStr(txtOrderNumber.Text, ",") > 0 Or InStr(txtOrderNumber.Text, "#") > 0 Or InStr(txtOrderNumber.Text, "'") > 0 Or InStr(txtOrderNumber.Text, ".") > 0 Then
                MessageError_CreateOrder(True, "PLEASE DON'T USE [ / ], [ \ ], [ & ], [ # ], [ ' ], [ . ] AND [ , ]")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If Trim(txtOrderNumber.Text).Length > 20 Then
                MessageError_CreateOrder(True, "MAXIMUM 20 CHARACTERS FOR RETAILER ORDER NUMBER !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If txtOrderName.Text = "" Then
                MessageError_CreateOrder(True, "CUSTOMER NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If InStr(txtOrderName.Text, "\") > 0 Or InStr(txtOrderName.Text, "/") > 0 Or InStr(txtOrderName.Text, ",") > 0 Or InStr(txtOrderName.Text, "&") > 0 Or InStr(txtOrderName.Text, ",") > 0 Or InStr(txtOrderName.Text, "#") > 0 Or InStr(txtOrderName.Text, "'") > 0 Or InStr(txtOrderName.Text, ".") > 0 Then
                MessageError_CreateOrder(True, "PLEASE DON'T USE [ / ], [ \ ], [ & ], [ # ], [ ' ], [ . ] AND [ , ]")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If txtOrderNumber.Text = settingClass.GetItemData("SELECT OrderNumber FROM OrderHeaders WHERE OrderNumber='" & txtOrderNumber.Text & "' AND CustomerId='" & lblId.Text & "' AND Active=1") Then
                MessageError_CreateOrder(True, "RETAILER ORDER NUMBER ALREADY EXISTS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If lblCompanyDetailName.Text = "JPMD BP" AndAlso ddlOrderType.SelectedValue = "" Then
                MessageError_CreateOrder(True, "ORDER TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorCreateOrder.InnerText = "" Then
                If ddlOrderType.SelectedValue = "" Then ddlOrderType.SelectedValue = "Regular"

                Dim orderClass As New OrderClass

                Dim thisId As String = orderClass.GetNewOrderHeaderId()
                Dim companyAlias As String = orderClass.GetCompanyAliasByCustomer(lblId.Text)

                Dim success As Boolean = False
                Dim retry As Integer = 0
                Dim maxRetry As Integer = 100
                Dim orderId As String = String.Empty

                Do While Not success
                    retry += 1
                    If retry > maxRetry Then
                        Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")
                    End If

                    Dim randomCode As String = orderClass.GenerateRandomCode()
                    orderId = companyAlias & randomCode
                    Try
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, DownloadBOE, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, @OrderType, 'Unsubmitted', @CreatedBy, GETDATE(), 0, 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", thisId)
                                myCmd.Parameters.AddWithValue("@OrderId", orderId)
                                myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                                myCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim())
                                myCmd.Parameters.AddWithValue("@OrderName", txtOrderName.Text.Trim())
                                myCmd.Parameters.AddWithValue("@OrderNote", txtOrderNote.Text.Trim())
                                myCmd.Parameters.AddWithValue("@OrderType", ddlOrderType.SelectedValue)
                                myCmd.Parameters.AddWithValue("@CreatedBy", Session("LoginId").ToString())

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using
                        success = True
                    Catch exSql As SqlException
                        If exSql.Number = 2601 OrElse exSql.Number = 2627 Then
                            success = False
                        Else
                            Throw
                        End If
                    End Try
                Loop

                If ddlOrderType.SelectedValue = "Builder" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderBuilders(Id) VALUES (@Id)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using
                End If

                Dim directoryOrder As String = Server.MapPath(String.Format("~/File/Order/{0}/", orderId))
                If Not Directory.Exists(directoryOrder) Then
                    Directory.CreateDirectory(directoryOrder)
                End If

                dataLog = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Created"}
                settingClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", thisId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_CreateOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_CreateOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnRecalculate_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataOrder As DataTable = settingClass.GetDataTable("SELECT * FROM OrderHeaders WHERE CustomerId='" & lblId.Text & "' AND Active=1 AND (Status = 'Unsubmitted' OR Status='Waiting Proforma')")
            If Not dataOrder.Rows.Count = 0 Then
                Dim orderClass As New OrderClass
                For i As Integer = 0 To dataOrder.Rows.Count - 1
                    Dim orderId As String = dataOrder.Rows(i)("Id").ToString()

                    orderClass.CalculatePriceByOrder(orderId)
                Next

                dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), "Customer Re-Calculate Price Order"}
                settingClass.Logs(dataLog)
            End If

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
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
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET Active=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), "Customer Deleted"}
            settingClass.Logs(dataLog)

            Response.Redirect("~/setting/customer/", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnWelcome_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim checkPrimary As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerContacts WHERE CustomerId='" & lblId.Text & "' AND [Primary]=1")
            If checkPrimary = 0 Then
                MessageError(True, "PRIMARY CONTACT IS REQUIRED !")
                Exit Sub
            End If

            Dim mailingClass As New MailingClass
            mailingClass.LoginCredentials(lblId.Text, Session("LoginId").ToString(), "Welcome Customer")

            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), "Send Welcome Email"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(customerId As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CustomerId", customerId),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@CompanyId", Session("CompanyId")),
                New SqlParameter("@LoginId", Session("LoginId")),
                New SqlParameter("@LevelName", Session("LevelName"))
            }
            Dim thisData As DataRow = settingClass.GetDataRowSP("sp_CustomerDetail", params)

            If thisData Is Nothing Then
                Response.Redirect("~/setting/customer", False)
                Exit Sub
            End If

            Dim priceGroupId As String = thisData("PriceGroupId").ToString()
            Dim shutterPriceGroupId As String = thisData("ShutterPriceGroupId").ToString()
            Dim doorPriceGroupId As String = thisData("DoorPriceGroupId").ToString()

            Dim priceGroupName As String = settingClass.GetItemData("SELECT Name FROM PriceGroups WHERE Id='" & priceGroupId & "'")
            Dim shutterPriceGroupName As String = settingClass.GetItemData("SELECT Name FROM PriceGroups WHERE Id='" & shutterPriceGroupId & "'")
            Dim doorPriceGroupName As String = settingClass.GetItemData("SELECT Name FROM PriceGroups WHERE Id='" & doorPriceGroupId & "'")

            Dim sponsorId As String = thisData("SponsorId").ToString()
            Dim sponsorName As String = String.Empty
            If Not String.IsNullOrEmpty(sponsorId) Then
                sponsorName = settingClass.GetItemData("SELECT Name FROM Customers WHERE Id='" & sponsorId & "'")
            End If

            lblDebtorCode.Text = thisData("DebtorCode").ToString()
            lblName.Text = thisData("Name").ToString()
            lblCompanyId.Text = thisData("CompanyId").ToString()
            lblCompanyName.Text = thisData("CompanyName").ToString()
            lblCompanyDetailId.Text = thisData("CompanyDetailId").ToString()
            lblCompanyDetailName.Text = thisData("CompanyDetailName").ToString()
            lblArea.Text = thisData("Area").ToString()
            lblOperator.Text = thisData("OperatorName").ToString()
            lblLevel.Text = thisData("Level").ToString()
            lblSponsor.Text = sponsorName
            lblPriceGroup.Text = priceGroupName
            lblPriceGroupShutter.Text = shutterPriceGroupName
            lblPriceGroupDoor.Text = doorPriceGroupName
            lblOnStop.Text = thisData("CustOnStop").ToString()
            lblCashSale.Text = thisData("CustCashSale").ToString()
            lblNewsletter.Text = thisData("CustNewsletter").ToString()
            lblMinSurcharge.Text = thisData("CustMinSurcharge").ToString()
            lblActive.Text = thisData("CustActive").ToString()

            btnEditCustomer.Visible = LoginAccess("Edit")
            If Session("RoleName") = "Sales" AndAlso Session("CustomerId") = customerId Then
                btnEditCustomer.Visible = False
            End If
            aDelete.Visible = LoginAccess("Delete")
            aCreateOrder.Visible = LoginAccess("Create Order")
            aRecalculate.Visible = LoginAccess("Recalculate Order")
            aWelcome.Visible = LoginAccess("Welcome Customer")

            If customerId = "3" Then aWelcome.Visible = False
            If lblOnStop.Text = "Yes" Then aCreateOrder.Visible = False

            divLevelSponsor.Visible = LoginAccess("Visible Level Sponsor")
            divOrderType.Visible = False
            If lblCompanyDetailName.Text = "JPMD BP" Then divOrderType.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_CreateOrder(visible As Boolean, message As String)
        divErrorCreateOrder.Visible = visible : msgErrorCreateOrder.InnerText = message
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


    ' START CUSTOMER CONTACT

    Protected Sub gvListContact_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-contact"

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                url = String.Format("~/setting/customer/contact/edit?contactid={0}", dataId)
                Response.Redirect(url, False)
                Exit Sub
            End If
        End If
    End Sub

    Protected Sub btnAddContact_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-contact"
        url = String.Format("~/setting/customer/contact/add?custid={0}", lblId.Text)
        Response.Redirect(url, False)
        Exit Sub
    End Sub

    Protected Sub btnDeleteContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "list-contact"
        Try
            Dim thisId As String = txtIdContactDelete.Text

            Dim fullContact As String = settingClass.GetItemData("SELECT CONCAT('Contact Name: ', ISNULL(Name, ''), ', ', 'Email: ', ISNULL(Email, ''), ', ', 'Tags: ', ISNULL(Tags, '')) AS ThisContact FROM CustomerContacts WHERE Id='" & thisId & "'")

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

            Dim stringLog As String = String.Format("Customer Contact Deleted | {0}", fullContact)
            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), stringLog}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnPrimaryContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "list-contact"
        Try
            Dim thisId As String = txtIdPrimaryContact.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET [Primary]=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET Tags='Confirming,Invoicing,Quoting,Newsletter', [Primary]=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"CustomerContacts", lblIdContact.Text, Session("LoginId"), "Set As Primary Contact"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataContact(customerId As String)
        MessageError_Contact(False, String.Empty)
        Try
            gvListContact.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerContacts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
            gvListContact.DataBind()
            gvListContact.Columns(1).Visible = LoginAccess("Visible ID Contact")

            btnAddContact.Visible = LoginAccess("Add Contact")

            Dim primaryContact As String = settingClass.GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId='" & customerId & "' AND [Primary]=1")
            loginContactPrimary.InnerText = primaryContact
            If String.IsNullOrEmpty(primaryContact) Then
                loginContactPrimary.InnerText = "PLEASE ADD THE CONTACT DATA FIRST BEFORE PROCEEDING."
            End If
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError_Contact(visible As Boolean, message As String)
        divErrorContact.Visible = visible : msgErrorContact.InnerText = message
    End Sub

    Protected Function VisibleYesPrimaryContact(primary As Boolean) As Boolean
        If primary = True Then : Return True : End If
        Return False
    End Function

    Protected Function VisibleNoPrimaryContact(primary As Boolean) As Boolean
        If primary = False Then : Return True : End If
        Return False
    End Function

    Protected Function VisiblePrimaryContact(primary As Boolean) As Boolean
        If primary = False Then Return True
        Return False
    End Function

    ' END CUSTOMER CONTACT

    ' START CUSTOMER ADDRESS

    Protected Sub gvListAddress_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-address"

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                url = String.Format("~/setting/customer/address/edit?addressid={0}", dataId)
                Response.Redirect(url, False)
                Exit Sub
            End If
        End If
    End Sub

    Protected Sub btnAddAddress_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-address"
        url = String.Format("~/setting/customer/address/add?custid={0}", lblId.Text)
        Response.Redirect(url, False)
        Exit Sub
    End Sub

    Protected Sub btnDeleteAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Session("selectedTabCustomer") = "list-address"
        Try
            Dim thisId As String = txtIdAddressDelete.Text

            Dim fullDesc As String = settingClass.GetItemData("SELECT CONCAT('Description: ', ISNULL(Description, ''), ', ', 'Address: ', ISNULL(Address, ''), ', ', 'Suburb: ', ISNULL(Suburb, ''), ', ', 'State: ', ISNULL(State, ''), ', ', 'PostCode: ', ISNULL(PostCode, '')) AS FullDescription FROM CustomerAddress WHERE Id='" & thisId & "'")

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

            Dim logDesc As String = String.Format("Customer Address Deleted | {0}", fullDesc)
            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), logDesc}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Address(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnPrimaryAddress_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "list-address"
        Try
            Dim thisId As String = txtIdPrimaryAddress.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET [Primary]=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET [Primary]=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"CustomerAddress", thisId, Session("LoginId"), "Set As Primary Address"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataAddress(customerId As String)
        MessageError_Address(False, String.Empty)
        lblIdAddress.Text = String.Empty
        Try
            gvListAddress.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerAddress WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
            gvListAddress.DataBind()
            gvListAddress.Columns(1).Visible = LoginAccess("Visible ID Address")

            btnAddAddress.Visible = LoginAccess("Add Address")
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Address(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function BindDetailAddress(addressId As String) As String
        Dim result As String = String.Empty
        If Not addressId = "" Then
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

    Protected Function VisibleYesPrimaryAddress(primary As Boolean) As Boolean
        If primary = True Then : Return True : End If
        Return False
    End Function

    Protected Function VisibleNoPrimaryAddress(primary As Boolean) As Boolean
        If primary = False Then : Return True : End If
        Return False
    End Function

    Protected Function VisiblePrimaryAddress(primary As Boolean) As Boolean
        If primary = False Then Return True
        Return False
    End Function

    Protected Sub MessageError_Address(visible As Boolean, message As String)
        divErrorAddress.Visible = visible : msgErrorAddress.InnerText = message
    End Sub

    ' END CUSTOMER ADDRESS

    ' START CUSTOMER BUSINESS

    Protected Sub gvListBusiness_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                Session("selectedTabCustomer") = "list-business"
                url = String.Format("~/setting/customer/business/edit?businessid={0}", dataId)
                Response.Redirect(url, False)
            End If
        End If
    End Sub

    Protected Sub btnAddBusiness_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-business"
        url = String.Format("~/setting/customer/business/add?custid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnDeleteBusiness_Click(sender As Object, e As EventArgs)
        MessageError_Business(False, String.Empty)
        Session("selectedTabCustomer") = "list-business"
        Try
            Dim thisId As String = txtIdBusinessDelete.Text

            Dim fullBusiness As String = settingClass.GetItemData("SELECT CONCAT('ABN Number: ', ISNULL(ABNNumber, ''), ', ', 'Registered Name: ', ISNULL(RegisteredName, '')) AS FullDescription FROM CustomerBusiness WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerBusiness WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerBusiness' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Dim stringLog As String = String.Format("Customer Business Deleted | {0}", fullBusiness)
            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), stringLog}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Business(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Business(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnPrimaryBusiness_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "list-business"
        Try
            Dim thisId As String = txtIdPrimaryBusiness.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerBusiness SET [Primary]=0 WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerBusiness SET [Primary]=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"CustomerBusiness", thisId, Session("LoginId"), "Set As Primary Business"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataBusiness(customerId As String)
        MessageError_Business(False, String.Empty)
        lblIdBusiness.Text = String.Empty
        Try
            gvListBusiness.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerBusiness WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
            gvListBusiness.DataBind()
            gvListBusiness.Columns(1).Visible = LoginAccess("Visible ID Business")

            btnAddBusiness.Visible = LoginAccess("Add Business")
        Catch ex As Exception
            MessageError_Business(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Business(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError_Business(visible As Boolean, message As String)
        divErrorBusiness.Visible = visible : msgErrorBusiness.InnerText = message
    End Sub

    Protected Function VisibleYesPrimaryBusiness(primary As Boolean) As Boolean
        If primary = True Then : Return True : End If
        Return False
    End Function

    Protected Function VisibleNoPrimaryBusiness(primary As Boolean) As Boolean
        If primary = False Then : Return True : End If
        Return False
    End Function

    Protected Function VisiblePrimaryBusiness(primary As Boolean) As Boolean
        If primary = False Then Return True
        Return False
    End Function

    ' START CUSTOMER BUSINESS

    ' START CUSTOMER LOGIN
    Protected Sub gvListLogin_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                Session("selectedTabCustomer") = "list-login"
                url = String.Format("~/setting/customer/login/edit?loginid={0}", dataId)
                Response.Redirect(url, False)
            ElseIf e.CommandName = "InstallerAccess" Then
                MessageError_InstallerAccess(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showInstallerAccess(); };"
                Session("selectedTabCustomer") = "list-login"
                Try
                    lblIdLogin.Text = dataId

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM InstallerAccess WHERE Id='" & dataId & "'")

                    Dim areaArray() As String = thisData("Area").ToString().Split(",")
                    Dim tagsList As List(Of String) = areaArray.ToList()

                    For Each i In areaArray
                        If Not (i.Equals(String.Empty)) Then
                            lbInstallerAccess.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showInstallerAccess", thisScript, True)
                Catch ex As Exception
                    MessageError_InstallerAccess(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_InstallerAccess(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showInstallerAccess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddLogin_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-login"
        url = String.Format("~/setting/customer/login/add?custid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnInstallerAccess_Click(sender As Object, e As EventArgs)
        MessageError_InstallerAccess(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showInstallerAccess(); };"
        Try
            Dim thisArea As String = String.Empty
            Dim selected As String = String.Empty
            For Each item As ListItem In lbInstallerAccess.Items
                If item.Selected Then
                    selected += item.Text & ","
                End If
            Next

            If Not selected = "" Then
                thisArea = selected.Remove(selected.Length - 1).ToString()
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE InstallerAccess SET Area=@Area WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblIdLogin.Text)
                    myCmd.Parameters.AddWithValue("@Area", thisArea)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"CustomerBusiness", lblIdBusiness.Text, Session("LoginId"), "Business Updated"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_InstallerAccess(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_InstallerAccess(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showInstallerAccess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnActiveLogin_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "list-login"
        Try
            Dim thisId As String = txtIdActiveLogin.Text

            Dim active As Integer = 1
            If txtActiveLogin.Text = "1" Then : active = 0 : End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Active=@Active, FailedCount=0 WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Active", active)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Customer Login Has Been Activated"
            If active = 0 Then activeDesc = "Customer Login Has Been Deactivated"

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnChangePasswordLogin_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "list-login"
        Try
            If Not String.IsNullOrEmpty(txtChangePassword.Text) Then
                Dim thisId As String = txtIdChangePassword.Text
                Dim newPassword As String = settingClass.Encrypt(txtChangePassword.Text)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Password", newPassword)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Change Password Login"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnResetPass_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "list-login"
        Try
            Dim thisId As String = txtIdResetPass.Text
            Dim newPassword As String = settingClass.Encrypt(txtNewResetPass.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Logins SET Password=@Password, FailedCount=0, ResetLogin=1, Active=1 WHERE Id=@Id; DELETE FROM Sessions WHERE LoginId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Password", newPassword)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Logins", thisId, Session("LoginId").ToString(), "Customer Login Reset Password"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnLoginCredentials_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "list-login"
        Try
            Dim checkPrimary As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerContacts WHERE CustomerId='" & lblId.Text & "' AND [Primary]=1")
            If checkPrimary = 0 Then
                MessageError_Login(True, "PRIMARY CONTACT EMAIL IS REQUIRED !")
                Exit Sub
            End If

            Dim mailingClass As New MailingClass
            mailingClass.LoginCredentials(lblId.Text, Session("LoginId").ToString(), "Login Credentials")

            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), "Sent Login Credentials"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataLogin(customerId As String)
        MessageError_Login(False, String.Empty)
        Try
            Dim thisQuery As String = "SELECT Logins.*, LoginRoles.Name AS RoleName, LoginLevels.Name AS LevelName, CASE WHEN Logins.Pricing=1 THEN 'Yes' WHEN Logins.Pricing=0 THEN 'No' ELSE 'Error' END AS DataPricing, CASE WHEN Logins.Active=1 THEN 'Yes' WHEN Logins.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Logins LEFT JOIN LoginRoles ON Logins.RoleId=LoginRoles.Id LEFT JOIN LoginLevels ON Logins.LevelId=LoginLevels.Id WHERE Logins.CustomerId='" & customerId & "' ORDER BY Logins.RoleId, Logins.Id ASC"

            gvListLogin.DataSource = settingClass.GetDataTable(thisQuery)
            gvListLogin.DataBind()
            gvListLogin.Columns(1).Visible = LoginAccess("Visible ID Login")
            gvListLogin.Columns(5).Visible = LoginAccess("Visible Email Login")

            btnAddLogin.Visible = LoginAccess("Add Login")
            aCredentialsLogin.Visible = LoginAccess("Login Credentials")
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function DencryptPassword(password As String) As String
        Return settingClass.Decrypt(password)
    End Function

    Protected Function TextActive_Login(active As Boolean) As String
        If active = True Then Return "Disable"
        Return "Enable"
    End Function

    Protected Sub MessageError_Login(visible As Boolean, message As String)
        divErrorLogin.Visible = visible : msgErrorLogin.InnerText = message
    End Sub

    Protected Sub MessageError_InstallerAccess(visible As Boolean, message As String)
        divErrorInstallerAccess.Visible = visible : msgErrorInstallerAccess.InnerText = message
    End Sub

    Protected Function VisibleInstallerAccess(roleId As String) As Boolean
        If Not String.IsNullOrEmpty(roleId) Then
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                Dim roleName As String = settingClass.GetItemData("SELECT Name FROM LoginRoles WHERE Id='" & roleId & "'")
                If roleName = "Installer" Then Return True
            End If
            Return False
        End If
        Return False
    End Function

    ' END CUSTOMER LOGIN

    ' CUSTOMER DISCOUNT

    Protected Sub gvListDiscount_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            Session("selectedTabCustomer") = "list-discount"
            url = String.Format("~/setting/customer/discount/edit?discountid={0}", dataId)
            Response.Redirect(url, False)
        End If
    End Sub

    Protected Sub btnAddDiscountA_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-discount"
        url = String.Format("~/setting/customer/discount/add?custid={0}&type={1}", lblId.Text, "product")
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnAddDiscountB_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-discount"
        url = String.Format("~/setting/customer/discount/add?custid={0}&type={1}", lblId.Text, "productgroup")
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnProcessDiscount_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDiscount(False, String.Empty)
        Session("selectedTabCustomer") = "list-discount"
        Dim thisScript As String = "window.onload = function() { showProcessDiscount(); };"
        Try
            If msgErrorProcessDiscount.InnerText = "" Then
                If lblActionDiscount.Text = "Add" Then
                    Dim designData As DataTable = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE applyArray.VALUE='Discounts' AND companyArray.VALUE='" & lblCompanyId.Text & "' ORDER BY Id ASC")

                    For i As Integer = 0 To designData.Rows.Count - 1
                        Dim designId As String = designData.Rows(i)("Id").ToString()

                        Dim checkData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & lblId.Text & "' AND Type='" & ddlDiscountType.SelectedValue & "' AND DataId='" & designId & "'")

                        If checkData IsNot Nothing Then
                            UpdateDiscount(checkData)
                        Else
                            InsertDiscount(designId)
                        End If
                    Next

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblActionDiscount.Text = "Add Custom" Then
                    Dim discounDataId As String = ddlDiscountDataId.SelectedValue
                    If ddlDiscountType.SelectedValue = "PriceProductGroups" Then
                        discounDataId = ddlDiscountDataIdB.SelectedValue
                    End If

                    Dim thisQuery As String = String.Format("SELECT * FROM CustomerDiscounts WHERE CustomerId='{0}' AND Type='{1}' AND DataId='{2}'", lblId.Text, ddlDiscountType.SelectedValue, discounDataId)
                    Dim checkData As DataRow = settingClass.GetDataRow(thisQuery)

                    If checkData IsNot Nothing Then
                        UpdateDiscount(checkData)
                    Else
                        InsertDiscount(discounDataId)
                    End If

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblActionDiscount.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        thisConn.Open()

                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerDiscounts SET Discount=@Discount WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdDiscount.Text)
                            myCmd.Parameters.AddWithValue("@Discount", txtDiscountValue.Text)

                            myCmd.ExecuteNonQuery()
                        End Using

                        thisConn.Close()
                    End Using

                    dataLog = {"CustomerDiscounts", lblIdDiscount.Text, Session("LoginId").ToString(), "Customer Discount Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessDiscount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDiscount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteDiscount_Click(sender As Object, e As EventArgs)
        MessageError_Discount(False, String.Empty)
        Session("selectedTabCustomer") = "list-discount"
        Try
            Dim thisId As String = txtIdDiscountDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerDiscounts WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerDiscounts' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnResetDiscount_Click(sender As Object, e As EventArgs)
        MessageError_Discount(False, String.Empty)
        Session("selectedTabCustomer") = "list-discount"
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim discountData As DataTable = settingClass.GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & lblId.Text & "'")
                For i As Integer = 0 To discountData.Rows.Count - 1
                    Dim id As String = discountData.Rows(i)("Id").ToString()

                    Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerDiscounts' AND DataId=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", id)
                        myCmd.ExecuteNonQuery()
                    End Using
                Next

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerDiscounts WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            dataLog = {"Customers", lblId.Text, Session("LoginId").ToString(), "Customer discount has been reset."}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Discount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataDiscount(customerId As String)
        MessageError_Discount(False, String.Empty)
        Try
            gvListDiscount.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END, DataId ASC")
            gvListDiscount.DataBind()
            gvListDiscount.Columns(1).Visible = LoginAccess("Visible ID Discount")
            gvListDiscount.Columns(2).Visible = LoginAccess("Visible Type Discount")

            btnAddDiscount.Visible = LoginAccess("Add Discount")
            aResetDiscount.Visible = LoginAccess("Reset Discount")
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Discount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDiscountData()
        ddlDiscountDataId.Items.Clear()
        Try
            ddlDiscountDataId.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE companyArray.VALUE='" & lblCompanyId.Text & "' AND applyArray.VALUE='Discounts' ORDER BY Name ASC")
            ddlDiscountDataId.DataTextField = "Name"
            ddlDiscountDataId.DataValueField = "Id"
            ddlDiscountDataId.DataBind()

            If ddlDiscountDataId.Items.Count > 1 Then
                ddlDiscountDataId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDiscountDataId.Items.Clear()
        End Try
    End Sub

    Protected Sub BindDiscountDataB()
        ddlDiscountDataIdB.Items.Clear()
        Try
            ddlDiscountDataIdB.DataSource = settingClass.GetDataTable("SELECT PriceProductGroups.* FROM PriceProductGroups LEFT JOIN Designs ON PriceProductGroups.DesignId=Designs.Id CROSS APPLY STRING_SPLIT(PriceProductGroups.CompanyDetailId, ',') AS companyArray WHERE companyArray.VALUE='" & lblCompanyDetailId.Text & "' AND PriceProductGroups.Active=1 AND Designs.Type='Blinds' AND PriceProductGroups.Name NOT LIKE '%Panel Glide - Panel Only%' AND PriceProductGroups.Name NOT LIKE '%Panel Glide - Track Only%' ORDER BY PriceProductGroups.Name ASC")
            ddlDiscountDataIdB.DataTextField = "Name"
            ddlDiscountDataIdB.DataValueField = "Id"
            ddlDiscountDataIdB.DataBind()

            If ddlDiscountDataIdB.Items.Count > 1 Then
                ddlDiscountDataIdB.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDiscountDataIdB.Items.Clear()
        End Try
    End Sub

    Protected Sub InsertDiscount(dataId As String)
        Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerDiscounts ORDER BY Id DESC")

        Using conn As New SqlConnection(myConn)
            Using cmd As New SqlCommand("INSERT INTO CustomerDiscounts VALUES (@Id, @CustomerId, @Type, @DataId, @Discount, NULL)", conn)
                cmd.Parameters.AddWithValue("@Id", thisId)
                cmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                cmd.Parameters.AddWithValue("@Type", ddlDiscountType.SelectedValue)
                cmd.Parameters.AddWithValue("@DataId", dataId)
                cmd.Parameters.AddWithValue("@Discount", txtDiscountValue.Text)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        settingClass.Logs({"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Created"})
    End Sub

    Protected Sub UpdateDiscount(checkData As DataRow)
        Dim thisId As String = checkData("Id").ToString()
        Dim thisDiscount As Decimal = CDec(checkData("Discount"))
        Dim newDisc As Decimal = settingClass.GetTotalDiscount(thisDiscount, txtDiscountValue.Text)

        Using conn As New SqlConnection(myConn)
            Using cmd As New SqlCommand("UPDATE CustomerDiscounts SET Discount=@Discount WHERE Id=@Id", conn)
                cmd.Parameters.AddWithValue("@Id", thisId)
                cmd.Parameters.AddWithValue("@Discount", newDisc)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        settingClass.Logs({"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Added"})
    End Sub

    Protected Function DiscountTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Function DiscountValue(data As Decimal) As String
        If data > 0 Then Return data.ToString("G29", enUS) & "%"
        Return "ERROR"
    End Function

    Protected Sub MessageError_Discount(visible As Boolean, message As String)
        divErrorDiscount.Visible = visible : msgErrorDiscount.InnerText = message
    End Sub

    Protected Sub MessageError_ProcessDiscount(visible As Boolean, message As String)
        divErrorProcessDiscount.Visible = visible : msgErrorProcessDiscount.InnerText = message
    End Sub

    ' END CUSTOMER DISCOUNT

    ' START CUSTOMER PROMO
    Protected Sub gvListPromo_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-promo"

            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_DetailPromo(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showDetailPromo(); };"
                Try
                    Dim promoId As String = settingClass.GetItemData("SELECT PromoId FROM CustomerPromos WHERE Id='" & dataId & "'")
                    gvListDetailPromo.DataSource = settingClass.GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")
                    gvListDetailPromo.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailPromo", thisScript, True)
                Catch ex As Exception
                    MessageError_DetailPromo(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_DetailPromo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailPromo", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddPromo_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-promo"
        url = String.Format("~/setting/customer/promo/add?custid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnDeletePromo_Click(sender As Object, e As EventArgs)
        MessageError_Promo(False, String.Empty)
        Session("selectedTabCustomer") = "list-promo"
        Try
            Dim thisId As String = txtIdPromoDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Promo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnResetPromo_Click(sender As Object, e As EventArgs)
        MessageError_Promo(False, String.Empty)
        Session("selectedTabCustomer") = "list-promo"
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim discountData As DataTable = settingClass.GetDataTable("SELECT * FROM CustomerPromos WHERE CustomerId='" & lblId.Text & "'")
                For i As Integer = 0 To discountData.Rows.Count - 1
                    Dim id As String = discountData.Rows(i)("Id").ToString()

                    Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", id)
                        myCmd.ExecuteNonQuery()
                    End Using
                Next

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE CustomerId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Promo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Promo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataPromo(customerId As String)
        MessageError_Promo(False, String.Empty)
        Try
            gvListPromo.DataSource = settingClass.GetDataTable("SELECT CustomerPromos.*, Promos.Name AS PromoName, Promos.StartDate AS StartDate, Promos.EndDate AS EndDate FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "'")
            gvListPromo.DataBind()
            gvListPromo.Columns(1).Visible = LoginAccess("Visible ID Promo")

            btnAddPromo.Visible = LoginAccess("Add Promo")
            aResetPromo.Visible = LoginAccess("Reset Promo")
        Catch ex As Exception
            MessageError_Promo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Promo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function PromoValue(data As Decimal) As String
        If data > 0 Then Return data.ToString("G29", enUS) & "%"
        Return "ERROR"
    End Function

    Protected Function PromoTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        If type = "FrameColours" Then Return dataId
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Sub MessageError_Promo(visible As Boolean, message As String)
        divErrorPromo.Visible = visible : msgErrorPromo.InnerText = message
    End Sub

    Protected Sub MessageError_DetailPromo(visible As Boolean, message As String)
        divErrorDetailPromo.Visible = visible : msgErrorDetailPromo.InnerText = message
    End Sub

    ' END CUSTOMER PROMO

    ' START CUSTOMER PRODUCT ACCESS
    Protected Sub gvListProduct_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-product"
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageErrorProcess_Product(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessProduct(); };"
                Try
                    lblIdProduct.Text = dataId
                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerProductAccess WHERE Id='" & lblIdProduct.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindDesignProduct(lblCompanyId.Text)

                    Dim tagsArray() As String = myData("DesignId").ToString().Split(",")
                    Dim tagsList As List(Of String) = tagsArray.ToList()
                    For Each i In tagsArray
                        If Not (i.Equals(String.Empty)) Then
                            lbProductTags.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
                Catch ex As Exception
                    MessageErrorProcess_Product(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageErrorProcess_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSubmitResetProduct_Click(sender As Object, e As EventArgs)
        MessageError_Product(False, String.Empty)
        Session("selectedTabCustomer") = "list-product"
        Try
            Using thisConn As New SqlConnection(myConn)
                Dim desingId As String = settingClass.GetProductAccess(lblCompanyId.Text)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerProductAccess SET DesignId=@DesignId WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Parameters.AddWithValue("@DesignId", desingId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"CustomerProductAccess", lblId.Text, Session("LoginId").ToString(), "Reset Customer Product Access"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnProcessProduct_Click(sender As Object, e As EventArgs)
        MessageError_Product(False, String.Empty)
        Session("selectedTabCustomer") = "list-product"
        Dim thisScript As String = "window.onload = function() { showProcessProduct(); };"
        Try
            Dim designId As String = String.Empty
            Dim tags As String = String.Empty
            For Each item As ListItem In lbProductTags.Items
                If item.Selected Then
                    tags += item.Value.ToString() & ","
                End If
            Next
            If Not tags = "" Then
                designId = tags.Remove(tags.Length - 1).ToString()
            End If

            If msgErrorProcessProduct.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerProductAccess SET DesignId=@DesignId WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@DesignId", designId)

                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                dataLog = {"CustomerProductAccess", lblIdProduct.Text, Session("LoginId").ToString(), "Customer Product Access Updated"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageErrorProcess_Product(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageErrorProcess_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
        End Try
    End Sub

    Protected Sub BindDataProduct(customerId As String)
        MessageError_Product(False, String.Empty)
        Try
            gvListProduct.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerProductAccess WHERE Id='" + customerId + "'")
            gvListProduct.DataBind()
        Catch ex As Exception
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesignProduct(companyId As String)
        lbProductTags.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                lbProductTags.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray WHERE companyArray.VALUE='" & companyId & "' ORDER BY Name ASC")
                lbProductTags.DataTextField = "Name"
                lbProductTags.DataValueField = "Id"
                lbProductTags.DataBind()
                If lbProductTags.Items.Count > 1 Then
                    lbProductTags.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            lbProductTags.Items.Clear()
        End Try
    End Sub

    Protected Function BindDetailProduct(customerId As String) As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty
            Dim myData As DataTable = settingClass.GetDataTable("SELECT Designs.Name AS DesignName FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray LEFT JOIN Designs ON designArray.VALUE=Designs.Id WHERE CustomerProductAccess.Id='" & customerId & "' ORDER BY Designs.Name ASC ")
            If Not myData.Rows.Count = 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("DesignName").ToString()
                    hasil += designName & ", "
                Next
            End If

            result = hasil.Remove(hasil.Length - 2).ToString()
        Catch ex As Exception
            result = "Error"
        End Try
        Return result
    End Function

    Protected Sub MessageErrorProcess_Product(visible As Boolean, message As String)
        divErrorProcessProduct.Visible = visible : msgErrorProcessProduct.InnerText = message
    End Sub

    Protected Sub MessageError_Product(visible As Boolean, message As String)
        divErrorProduct.Visible = visible : msgErrorProduct.InnerText = message
    End Sub

    ' END CUSTOMER PRODUCT ACCESS

    ' START CUSTOMER QUOTE
    Protected Sub BindDataQuote(customerId As String)
        MessageError_Quote(False, String.Empty)
        Try
            gvListQuote.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            gvListQuote.DataBind()
        Catch ex As Exception
            MessageError_Quote(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Quote(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function BindQuoteddress(customerId As String) As String
        Dim result As String = String.Empty

        If Not String.IsNullOrEmpty(customerId) Then
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")

            If thisData IsNot Nothing Then
                Dim parts As New List(Of String)

                Dim address As String = thisData("Address").ToString().Trim()
                Dim suburb As String = thisData("Suburb").ToString().Trim()
                Dim state As String = thisData("State").ToString().Trim()
                Dim postCode As String = thisData("PostCode").ToString().Trim()

                If Not String.IsNullOrEmpty(address) Then parts.Add(address)
                If Not String.IsNullOrEmpty(suburb) Then parts.Add(suburb)

                Dim statePostCode As String = (state & " " & postCode).Trim()
                If Not String.IsNullOrEmpty(statePostCode) Then parts.Add(statePostCode)

                result = String.Join(", ", parts)
            End If
        End If

        Return result
    End Function

    Protected Sub MessageError_Quote(visible As Boolean, message As String)
        divErrorQuote.Visible = visible : msgErrorQuote.InnerText = message
    End Sub

    ' END CUSTOMER QUOTE

    Protected Sub AllMessageError(visible As Boolean, message As String)
        MessageError(visible, message)
        MessageError_CreateOrder(visible, message)

        MessageError_Contact(visible, message)
        MessageError_Address(visible, message)
        MessageError_Business(visible, message)
        MessageError_Login(visible, message)
        MessageError_InstallerAccess(visible, message)

        MessageError_Discount(visible, message)
        MessageError_ProcessDiscount(visible, message)

        MessageError_Promo(visible, message)
        MessageError_DetailPromo(visible, message)

        MessageError_Product(visible, message)
        MessageErrorProcess_Product(visible, message)

        MessageError_Quote(visible, message)
    End Sub
End Class