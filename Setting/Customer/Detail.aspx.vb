Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Web.Services

Partial Class Setting_Customer_Detail
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim dataMailing As Object() = Nothing

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Dim url As String = String.Empty

    <WebMethod(EnableSession:=True)>
    Public Shared Sub UpdateSession(value As String)
        HttpContext.Current.Session("selectedTabCustomer") = value
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("customerid")) Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("customerid").ToString()

        If lblId.Text = "1" AndAlso Not Session("RoleName") = "Developer" Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

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
            If Session("RoleName") = "Sales" AndAlso Session("CustomerId") = lblId.Text Then
                secDetail.Visible = False
            End If
        End If
    End Sub

    Protected Sub btnEditCustomer_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/customer/edit?customeredit={0}", lblId.Text)
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
                Dim orderId As String = ""

                Do While Not success
                    retry += 1
                    If retry > maxRetry Then
                        Throw New Exception("FAILED TO GENERATE UNIQUE ORDER ID")
                    End If

                    Dim randomCode As String = orderClass.GenerateRandomCode()
                    orderId = companyAlias & randomCode
                    Try
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As New SqlCommand("INSERT INTO OrderHeaders (Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, OrderType, Status, CreatedBy, CreatedDate, DownloadBOE, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, @OrderType, 'Unsubmitted', @CreatedBy, GETDATE(), 0, 1); INSERT INTO OrderQuotes VALUES (@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
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

                Dim dataLog As Object() = {"OrderHeaders", thisId, Session("LoginId").ToString(), "Order Created"}
                settingClass.Logs(dataLog)

                url = String.Format("~/order/detail?orderid={0}", thisId)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError_CreateOrder(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_CreateOrder(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnCreateOrder_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
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

            Response.Redirect("~/setting/customer/", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDelete_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnLog_Click(sender As Object, e As EventArgs)
        MessageError_Log(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showLog(); };"
        Try
            gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE DataId='" & lblId.Text & "' AND Type='Customers' ORDER BY ActionDate DESC")
            gvListLogs.DataBind()

            ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
        Catch ex As Exception
            MessageError_Log(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnLog_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
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

            If customerId = "2" AndAlso (Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office") Then
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

            btnEditCustomer.Visible = PageAction("Edit")
            If Session("RoleName") = "Sales" AndAlso Session("CustomerId") = customerId Then
                btnEditCustomer.Visible = False
            End If
            aDelete.Visible = PageAction("Delete")
            aCreateOrder.Visible = PageAction("Create Order")
            If lblOnStop.Text = "Yes" Then aCreateOrder.Visible = False

            divLevelSponsor.Visible = PageAction("Visible Level Sponsor")
            divOrderType.Visible = False
            If lblCompanyDetailName.Text = "JPMD BP" Then divOrderType.Visible = True
        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_CreateOrder(visible As Boolean, message As String)
        divErrorCreateOrder.Visible = visible : msgErrorCreateOrder.InnerText = message
    End Sub

    Protected Sub MessageError_Log(visible As Boolean, message As String)
        divErrorLog.Visible = visible : msgErrorLog.InnerText = message
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

    Protected Function BindTextLog(logId As String) As String
        Return settingClass.getTextLog(logId)
    End Function


    ' START CUSTOMER CONTACT
    Protected Sub btnAddContact_Click(sender As Object, e As EventArgs)
        MessageError_ProcessContact(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessContact(); };"
        Session("selectedTabCustomer") = "list-contact"
        Try
            lblActionContact.Text = "Add"
            titleContact.InnerText = "Add Customer Contact"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessContact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessContact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddContact_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
        End Try
    End Sub

    Protected Sub gvListContact_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-contact"

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_ProcessContact(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessContact(); };"
                Try
                    lblIdContact.Text = dataId
                    lblActionContact.Text = "Edit"
                    titleContact.InnerText = "Edit Customer Contact"

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerContacts WHERE Id='" & dataId & "'")

                    If thisData Is Nothing Then Exit Sub

                    txtContactName.Text = thisData("Name").ToString()
                    ddlContactSalutation.SelectedValue = thisData("Salutation").ToString()
                    txtContactRole.Text = thisData("Role").ToString()
                    txtContactEmail.Text = thisData("Email").ToString()
                    txtContactPhone.Text = thisData("Phone").ToString()
                    txtContactMobile.Text = thisData("Mobile").ToString()
                    txtContactFax.Text = thisData("Fax").ToString()
                    txtContactNote.Text = thisData("Note").ToString()

                    Dim tagsArray() As String = thisData("Tags").ToString().Split(",")
                    Dim tagsList As List(Of String) = tagsArray.ToList()

                    For Each i In tagsArray
                        If Not (i.Equals(String.Empty)) Then
                            lbContactTags.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcessContact(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_ProcessContact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetailContact_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE Type='CustomerContacts' AND DataId='" & dataId & "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLogContact_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcessContact_Click(sender As Object, e As EventArgs)
        MessageError_ProcessContact(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessContact(); };"
        Session("selectedTabCustomer") = "list-contact"
        Try
            If txtContactName.Text = "" Then
                MessageError_ProcessContact(True, "CONTACT NAME IS REQURIED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessContact.InnerText = "" Then
                Dim thisTags As String = String.Empty
                Dim selected As String = String.Empty
                For Each item As ListItem In lbContactTags.Items
                    If item.Selected Then
                        selected += item.Text & ","
                    End If
                Next

                If Not selected = "" Then
                    thisTags = selected.Remove(selected.Length - 1).ToString()
                End If

                If lblActionContact.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerContacts ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerContacts VALUES (@Id, @CustomerId, @Name, @Salutation, @Role, @Email, @Phone, @Mobile, @Fax, @Tags, @Note, 0)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtContactName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Salutation", ddlContactSalutation.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Role", txtContactRole.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtContactEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Phone", txtContactPhone.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Mobile", txtContactMobile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Fax", txtContactFax.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtContactNote.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerContacts", thisId, Session("LoginId").ToString(), "Customer Contact Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblActionContact.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET CustomerId=@CustomerId, Name=@Name, Salutation=@Salutation, Role=@Role, Email=@Email, Phone=@Phone, Mobile=@Mobile, Fax=@Fax, Tags=@Tags, Note=@Note WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdContact.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtContactName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Salutation", ddlContactSalutation.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Role", txtContactRole.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtContactEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Phone", txtContactPhone.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Mobile", txtContactMobile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Fax", txtContactFax.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtContactNote.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerContacts", lblIdContact.Text, Session("LoginId"), "Customer Contact Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessContact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessContact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProcessContact_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "list-contact"
        Try
            Dim thisId As String = txtIdContactDelete.Text

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

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteContact_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnPrimaryContact_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataContact(customerId As String)
        MessageError_Contact(False, String.Empty)
        Try
            gvListContact.DataSource = settingClass.GetDataTable("SELECT *, CONVERT(VARCHAR, Salutation) + ' ' + CONVERT(VARCHAR, Name) AS ContactName FROM CustomerContacts WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
            gvListContact.DataBind()
            gvListContact.Columns(1).Visible = PageAction("Visible ID Contact")

            btnAddContact.Visible = PageAction("Add Contact")
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataContact", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub MessageError_Contact(visible As Boolean, message As String)
        divErrorContact.Visible = visible : msgErrorContact.InnerText = message
    End Sub

    Protected Sub MessageError_ProcessContact(visible As Boolean, message As String)
        divErrorProcessContact.Visible = visible : msgErrorProcessContact.InnerText = message
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

    Protected Sub btnAddAddress_Click(sender As Object, e As EventArgs)
        MessageError_ProcessAddress(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessAddress(); };"
        Session("selectedTabCustomer") = "list-address"
        Try
            lblActionAddress.Text = "Add"
            titleAddress.InnerText = "Add Customer Address"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessAddress(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessAddress(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddAddress_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
        End Try
    End Sub

    Protected Sub gvListAddress_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-address"

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_ProcessAddress(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessAddress(); };"
                Try
                    lblActionAddress.Text = "Edit"
                    titleAddress.InnerText = "Edit Customer Address"
                    lblIdAddress.Text = dataId

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerAddress WHERE Id='" & lblIdAddress.Text & "'")

                    If thisData Is Nothing Then Exit Sub

                    txtAddressDescription.Text = thisData("Description").ToString()
                    txtAddressName.Text = thisData("Address").ToString()
                    txtAddressSuburb.Text = thisData("Suburb").ToString()
                    txtAddressState.Text = thisData("State").ToString()
                    txtAddressPostCode.Text = thisData("PostCode").ToString()
                    ddlAddressCountry.SelectedValue = thisData("Country").ToString()
                    txtAddressNote.Text = thisData("Note").ToString()

                    Dim tagsArray() As String = thisData("Tags").ToString().Split(",")
                    Dim tagsList As List(Of String) = tagsArray.ToList()

                    For Each i In tagsArray
                        If Not (i.Equals(String.Empty)) Then
                            lbAddressTags.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcessAddress(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_ProcessAddress(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetailAddress_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE Type='CustomerAddress' AND DataId='" & dataId & "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLogAddress_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcessAddress_Click(sender As Object, e As EventArgs)
        MessageError_ProcessAddress(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessAddress(); };"
        Session("selectedTabCustomer") = "list-address"
        Try
            If txtAddressName.Text = "" Then
                MessageError_ProcessAddress(True, "ADDRESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If txtAddressSuburb.Text = "" Then
                MessageError_ProcessAddress(True, "SUBURB IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If txtAddressState.Text = "" Then
                MessageError_ProcessAddress(True, "STATE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If txtAddressPostCode.Text = "" Then
                MessageError_ProcessAddress(True, "POST CODE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If ddlAddressCountry.SelectedValue = "" Then
                MessageError_ProcessAddress(True, "COUNTRY IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessAddress.InnerText = "" Then
                Dim thisTags As String = String.Empty
                Dim selected As String = String.Empty
                For Each item As ListItem In lbAddressTags.Items
                    If item.Selected Then
                        selected += item.Text & ","
                    End If
                Next

                If Not selected = "" Then
                    thisTags = selected.Remove(selected.Length - 1).ToString()
                End If

                If lblActionAddress.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerAddress ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerAddress VALUES (@Id, @CustomerId, @Description, @Address, @Suburb, @State, @PostCode, @Country, @Tags, @Note, 0)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Description", txtAddressDescription.Text)
                            myCmd.Parameters.AddWithValue("@Address", txtAddressName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Suburb", txtAddressSuburb.Text.Trim())
                            myCmd.Parameters.AddWithValue("@State", txtAddressState.Text.Trim())
                            myCmd.Parameters.AddWithValue("@PostCode", txtAddressPostCode.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Country", ddlAddressCountry.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtAddressNote.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerAddress", thisId, Session("LoginId"), "Address Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblActionAddress.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET CustomerId=@CustomerId, Description=@Description, Address=@Address, Suburb=@Suburb, State=@State, PostCode=@PostCode, Country=@Country, Tags=@Tags, Note=@Note WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdAddress.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Description", txtAddressDescription.Text)
                            myCmd.Parameters.AddWithValue("@Address", txtAddressName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Suburb", txtAddressSuburb.Text.Trim())
                            myCmd.Parameters.AddWithValue("@State", txtAddressState.Text.Trim())
                            myCmd.Parameters.AddWithValue("@PostCode", txtAddressPostCode.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Country", ddlAddressCountry.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtAddressNote.Text.Trim())

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerAddress", lblIdAddress.Text, Session("LoginId"), "Address Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessAddress(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessAddress(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProcessAddress_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Session("selectedTabCustomer") = "list-address"
        Try
            Dim thisId As String = txtIdAddressDelete.Text

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

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Address(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteAddress_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnPrimaryAddress_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataAddress(customerId As String)
        MessageError_Address(False, String.Empty)
        lblIdAddress.Text = String.Empty
        lblActionAddress.Text = String.Empty
        Try
            gvListAddress.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerAddress WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
            gvListAddress.DataBind()
            gvListAddress.Columns(1).Visible = PageAction("Visible ID Address")

            btnAddAddress.Visible = PageAction("Add Address")
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Address(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataAddress", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Function BindDetailAddress(addressId As String) As String
        Dim result As String = String.Empty
        If Not addressId = "" Then
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerAddress WHERE Id='" & addressId & "'")
            If Not thisData IsNot Nothing Then
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

    Protected Sub MessageError_ProcessAddress(visible As Boolean, message As String)
        divErrorProcessAddress.Visible = visible : msgErrorProcessAddress.InnerText = message
    End Sub

    ' END CUSTOMER ADDRESS

    ' START CUSTOMER BUSINESS
    Protected Sub gvListBusiness_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-business"

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_ProcessBusiness(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessBusiness(); };"
                Try
                    lblIdBusiness.Text = dataId
                    lblActionBusiness.Text = "Edit"
                    titleBusiness.InnerText = "Edit Customer Business"

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerBusiness WHERE Id='" & dataId & "'")
                    If thisData Is Nothing Then Exit Sub

                    txtBusinessNumber.Text = thisData("ABNNumber").ToString()
                    txtBusinessName.Text = thisData("RegisteredName").ToString()

                    If Not String.IsNullOrEmpty(thisData("RegisteredDate").ToString()) Then
                        txtBusinessRegistered.Text = Convert.ToDateTime(thisData("RegisteredDate")).ToString("yyyy-MM-dd")
                    End If

                    If Not String.IsNullOrEmpty(thisData("ExpiryDate").ToString()) Then
                        txtBusinessExpiry.Text = Convert.ToDateTime(thisData("ExpiryDate")).ToString("yyyy-MM-dd")
                    End If

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessBusiness", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcessBusiness(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_ProcessBusiness(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetailBusiness_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessBusiness", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE Type='CustomerBusiness' AND DataId='" & dataId & "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLogBusiness_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddBusiness_Click(sender As Object, e As EventArgs)
        MessageError_ProcessBusiness(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessBusiness(); };"
        Session("selectedTabCustomer") = "list-business"
        Try
            lblActionBusiness.Text = "Add"
            titleBusiness.InnerText = "Add Customer Business"

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessBusiness", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessBusiness(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessBusiness(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddBusiness_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessBusiness", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessBusiness_Click(sender As Object, e As EventArgs)
        MessageError_ProcessBusiness(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessBusiness(); };"
        Session("selectedTabCustomer") = "list-business"
        Try
            If txtBusinessNumber.Text = "" Then
                MessageError_ProcessBusiness(True, "ABN NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessBusiness", thisScript, True)
                Exit Sub
            End If

            If txtBusinessName.Text = "" Then
                MessageError_ProcessBusiness(True, "REGISTERED NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessBusiness", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessBusiness.InnerText = "" Then
                If lblActionBusiness.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerBusiness ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerBusiness VALUES (@Id, @CustomerId, @ABNNumber, @RegisteredName, @RegisteredDate, @ExpiryDate, 0)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@ABNNumber", txtBusinessNumber.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredName", txtBusinessName.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredDate", If(String.IsNullOrEmpty(txtBusinessRegistered.Text), CType(DBNull.Value, Object), txtBusinessRegistered.Text))
                            myCmd.Parameters.AddWithValue("@ExpiryDate", If(String.IsNullOrEmpty(txtBusinessExpiry.Text), CType(DBNull.Value, Object), txtBusinessExpiry.Text))

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerBusiness", thisId, Session("LoginId"), "Business Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblActionBusiness.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerBusiness SET  CustomerId=@CustomerId, ABNNumber=@ABNNumber, RegisteredName=@RegisteredName, RegisteredDate=@RegisteredDate, ExpiryDate=@ExpiryDate WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdBusiness.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@ABNNumber", txtBusinessNumber.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredName", txtBusinessName.Text)
                            myCmd.Parameters.AddWithValue("@RegisteredDate", If(String.IsNullOrEmpty(txtBusinessRegistered.Text), CType(DBNull.Value, Object), txtBusinessRegistered.Text))
                            myCmd.Parameters.AddWithValue("@ExpiryDate", If(String.IsNullOrEmpty(txtBusinessExpiry.Text), CType(DBNull.Value, Object), txtBusinessExpiry.Text))

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerBusiness", lblIdBusiness.Text, Session("LoginId"), "Business Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessBusiness(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessBusiness(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProcessBusiness_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessBusiness", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteBusiness_Click(sender As Object, e As EventArgs)
        MessageError_Business(False, String.Empty)
        Session("selectedTabCustomer") = "list-business"
        Try
            Dim thisId As String = txtIdBusinessDelete.Text

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

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Business(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Business(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteBusiness_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Contact(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnPrimaryBusiness_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataBusiness(customerId As String)
        MessageError_Business(False, String.Empty)
        lblIdBusiness.Text = String.Empty
        lblActionBusiness.Text = String.Empty
        Try
            gvListBusiness.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerBusiness WHERE CustomerId='" & customerId & "' ORDER BY Id ASC")
            gvListBusiness.DataBind()
            gvListBusiness.Columns(1).Visible = PageAction("Visible ID Business")

            btnAddBusiness.Visible = PageAction("Add Business")
        Catch ex As Exception
            MessageError_Business(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Business(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataBusiness", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub MessageError_Business(visible As Boolean, message As String)
        divErrorBusiness.Visible = visible : msgErrorBusiness.InnerText = message
    End Sub

    Protected Sub MessageError_ProcessBusiness(visible As Boolean, message As String)
        divErrorProcessBusiness.Visible = visible : msgErrorProcessBusiness.InnerText = message
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
            Session("selectedTabCustomer") = "list-login"

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_ProcesLogin(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessLogin(); };"
                Try
                    lblIdLogin.Text = dataId
                    lblActionLogin.Text = "Edit"
                    titleLogin.InnerText = "Edit Customer Login"
                    divAccess.Visible = False
                    divLoginEmail.Visible = False
                    divPassword.Visible = False
                    If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                        divAccess.Visible = True
                        divLoginEmail.Visible = True
                        divPassword.Visible = True
                    End If

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerLogins WHERE Id='" & lblIdLogin.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindDataLoginRole()
                    BindDataLoginLevel()

                    ddlLoginRole.SelectedValue = myData("RoleId").ToString()
                    ddlLoginLevel.SelectedValue = myData("LevelId").ToString()
                    txtLoginUserName.Text = myData("UserName").ToString()
                    lblLoginUserNameOld.Text = myData("UserName").ToString()
                    txtLoginFullName.Text = myData("FullName").ToString()
                    txtLoginEmail.Text = myData("Email").ToString()
                    ddlPricing.SelectedValue = Convert.ToInt32(myData("Pricing"))
                    Dim password As String = myData("Password").ToString()
                    txtLoginPassword.Text = settingClass.Decrypt(password)

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcesLogin(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_ProcesLogin(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetailLogin_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                End Try
            ElseIf e.CommandName = "InstallerAccess" Then
                MessageError_InstallerAccess(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showInstallerAccess(); };"
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
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkInstallerAccess_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showInstallerAccess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE Type='CustomerLogins' AND DataId='" & dataId & "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLogLogin_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddLogin_Click(sender As Object, e As EventArgs)
        MessageError_ProcesLogin(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessLogin(); };"
        Session("selectedTabCustomer") = "list-login"
        Try
            lblActionLogin.Text = "Add"
            titleLogin.InnerText = "Add Customer Login"
            divAccess.Visible = False
            divLoginEmail.Visible = False
            divPassword.Visible = True
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                divAccess.Visible = True
                divLoginEmail.Visible = True
            End If
            txtLoginPassword.Text = settingClass.GenerateNewPassword(15)

            BindDataLoginRole()
            BindDataLoginLevel()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
        Catch ex As Exception
            MessageError_ProcesLogin(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcesLogin(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddLogin_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessLogin_Click(sender As Object, e As EventArgs)
        MessageError_ProcesLogin(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessLogin(); };"

        Session("selectedTabCustomer") = "list-login"
        Try
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                If ddlLoginRole.SelectedValue = "" Then
                    MessageError_ProcesLogin(True, "ROLE MEMBER IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If

                If ddlLoginLevel.SelectedValue = "" Then
                    MessageError_ProcesLogin(True, "LEVEL MEMBER IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If
            End If

            If Session("RoleName") = "IT" AndAlso Session("LevelName") = "Leader" Then
                If ddlLoginRole.SelectedValue = "3" AndAlso ddlLoginLevel.SelectedValue = "1" Then
                    MessageError_ProcesLogin(True, "YOU DO NOT HAVE PERMISSION TO ADD DATA WITH THIS ROLE AND LEVEL !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If
            End If

            If txtLoginUserName.Text = "" Then
                MessageError_ProcesLogin(True, "USERNAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                Exit Sub
            End If

            If Not Regex.IsMatch(txtLoginUserName.Text, "^[a-zA-Z0-9._-]+$") Then
                MessageError_ProcesLogin(True, "INVALID USERNAME. ONLY LETTERS, NUMBERS, DOT (.), UNDERSCRORE (_) & HYPHEN (-) ARE ALLOWED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                Exit Sub
            End If

            Dim checkUsername As String = settingClass.GetItemData("SELECT UserName FROM CustomerLogins WHERE UserName='" + txtLoginUserName.Text + "'")

            If lblActionLogin.Text = "Add" Then
                If txtLoginUserName.Text = checkUsername Then
                    MessageError_ProcesLogin(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If
            End If

            If lblActionLogin.Text = "Edit" And txtLoginUserName.Text <> lblLoginUserNameOld.Text Then
                If txtLoginUserName.Text = checkUsername Then
                    MessageError_ProcesLogin(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorProcessLogin.InnerText = "" Then
                If txtLoginPassword.Text = "" Then txtLoginPassword.Text = txtLoginUserName.Text
                If txtLoginFullName.Text = "" Then txtLoginFullName.Text = txtLoginUserName.Text
                Dim password As String = settingClass.Encrypt(txtLoginPassword.Text)

                If lblActionLogin.Text = "Add" Then
                    If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" Then
                        ddlLoginRole.SelectedValue = "8" : ddlLoginLevel.SelectedValue = "2"
                    End If

                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerLogins ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerLogins VALUES (@Id, @CustomerId, @RoleId, @LevelId, @UserName, @Password, @FullName, @Email, 0, NULL, 1, @Pricing, 1)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@RoleId", ddlLoginRole.SelectedValue)
                            myCmd.Parameters.AddWithValue("@LevelId", ddlLoginLevel.SelectedValue)
                            myCmd.Parameters.AddWithValue("@UserName", txtLoginUserName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Password", password)
                            myCmd.Parameters.AddWithValue("@FullName", txtLoginFullName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtLoginEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Pricing", ddlPricing.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerLogins", thisId, Session("LoginId").ToString(), "Customer Login Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblActionLogin.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET CustomerId=@CustomerId, RoleId=@RoleId, LevelId=@LevelId, UserName=@UserName, Password=@Password, FullName=@FullName, Email=@Email, Pricing=@Pricing WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblIdLogin.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@RoleId", ddlLoginRole.SelectedValue)
                            myCmd.Parameters.AddWithValue("@LevelId", ddlLoginLevel.SelectedValue)
                            myCmd.Parameters.AddWithValue("@UserName", txtLoginUserName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Password", password)
                            myCmd.Parameters.AddWithValue("@FullName", txtLoginFullName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtLoginEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Pricing", ddlPricing.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerLogins", lblIdLogin.Text, Session("LoginId").ToString(), "Customer Login Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcesLogin(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcesLogin(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProccessLogin_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
        End Try
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

            Dim dataLog As Object() = {"CustomerBusiness", lblIdBusiness.Text, Session("LoginId"), "Business Updated"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_InstallerAccess(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_InstallerAccess(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnInstallerAccess_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Active=@Active, FailedCount=0 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Active", active)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Customer Login Has Been Activated"
            If active = 0 Then activeDesc = "Customer Login Has Been Deactivated"

            Dim dataLog As Object() = {"CustomerLogins", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnActiveLogin_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnDeleteLogin_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "list-login"
        Try
            Dim thisId As String = txtIdLoginDelete.Text

            Dim userName As String = settingClass.GetItemData("SELECT UserName FROM CustomerLogins WHERE Id='" & thisId & "'")
            Dim logDescription As String = String.Format("Delete Customer Login : {0}", userName)
            Dim dataLog As Object() = {"CustomerLogins", "", Session("LoginId"), logDescription}
            settingClass.Logs(dataLog)

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerLogins WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerLogins' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteLogin_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Password=@Password, FailedCount=0, ResetLogin=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Password", newPassword)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"CustomerLogins", thisId, Session("LoginId").ToString(), "Customer Login Reset Password"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnResetPass_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataLogin(customerId As String)
        MessageError_Login(False, String.Empty)
        Try
            Dim thisQuery As String = "SELECT CustomerLogins.*, CustomerLoginRoles.Name AS RoleName, CustomerLoginLevels.Name AS LevelName FROM CustomerLogins LEFT JOIN CustomerLoginRoles ON CustomerLogins.RoleId=CustomerLoginRoles.Id LEFT JOIN CustomerLoginLevels ON CustomerLogins.LevelId=CustomerLoginLevels.Id WHERE CustomerLogins.CustomerId='" & customerId & "' ORDER BY CustomerLogins.RoleId, CustomerLogins.Id ASC"

            gvListLogin.DataSource = settingClass.GetDataTable(thisQuery)
            gvListLogin.DataBind()
            gvListLogin.Columns(1).Visible = PageAction("Visible ID Login")
            gvListLogin.Columns(5).Visible = PageAction("Visible Email Login")

            btnAddLogin.Visible = PageAction("Add Login")
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataLogin", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataLoginRole()
        ddlLoginRole.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM CustomerLoginRoles ORDER BY Name ASC"
            If Session("RoleName") = "IT" Then
                thisQuery = "SELECT * FROM CustomerLoginRoles WHERE Id<>'1' ORDER BY Name ASC"
                If Session("LevelName") = "Member" Then
                    thisQuery = "SELECT * FROM CustomerLoginRoles WHERE Id<>'1' AND Id<>'2' ORDER BY Name ASC"
                End If
            End If
            If Session("RoleName") = "Factory Office" Then
                thisQuery = "SELECT * FROM CustomerLoginRoles WHERE Id<>'1' AND Id<>'2' ORDER BY Name ASC"
                If Session("LevelName") = "Member" Then
                    thisQuery = "SELECT * FROM CustomerLoginRoles WHERE Id<>'1' AND Id<>'2' AND Id<>'3' ORDER BY Name ASC"
                End If
            End If

            ddlLoginRole.DataSource = settingClass.GetDataTable(thisQuery)
            ddlLoginRole.DataTextField = "Name"
            ddlLoginRole.DataValueField = "Id"
            ddlLoginRole.DataBind()

            ddlLoginRole.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlLoginRole.Items.Clear()
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataLoginRole", ex.ToString()}
            mailingClass.WebError(dataMailing)
        End Try
    End Sub

    Protected Sub BindDataLoginLevel()
        ddlLoginLevel.Items.Clear()
        Try
            ddlLoginLevel.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerLoginLevels ORDER BY Name ASC")
            ddlLoginLevel.DataTextField = "Name"
            ddlLoginLevel.DataValueField = "Id"
            ddlLoginLevel.DataBind()

            ddlLoginLevel.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlLoginLevel.Items.Clear()
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataLoginLevel", ex.ToString()}
            mailingClass.WebError(dataMailing)
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

    Protected Sub MessageError_ProcesLogin(visible As Boolean, message As String)
        divErrorProcessLogin.Visible = visible : msgErrorProcessLogin.InnerText = message
    End Sub

    Protected Function VisibleInstallerAccess(roleId As String) As Boolean
        If Not String.IsNullOrEmpty(roleId) Then
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                Dim roleName As String = settingClass.GetItemData("SELECT Name FROM CustomerLoginRoles WHERE Id='" & roleId & "'")
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
            Session("selectedTabCustomer") = "list-discount"

            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_ProcessDiscount(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessDiscount(); visibleDiscountType();};"
                Try
                    lblIdDiscount.Text = dataId
                    lblActionDiscount.Text = "Edit"
                    titleDiscount.InnerText = "Edit Discount"

                    ddlDiscountType.Enabled = False
                    ddlDiscountDataId.Enabled = False

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerDiscounts WHERE Id='" & lblIdDiscount.Text & "'")
                    If myData Is Nothing Then Exit Sub

                    BindDiscountData()
                    BindDiscountDataB()

                    Dim discountType As String = myData("Type").ToString()
                    ddlDiscountType.SelectedValue = myData("Type").ToString()
                    If discountType = "Designs" Then
                        ddlDiscountDataId.SelectedValue = myData("DataId").ToString()
                    End If
                    If discountType = "PriceProductGroups" Then
                        ddlDiscountDataIdB.SelectedValue = myData("DataId").ToString()
                    End If

                    txtDiscountValue.Text = Convert.ToDecimal(myData("Discount")).ToString("G29", enUS)

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcessDiscount(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_ProcessDiscount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetailDiscount_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE Type='CustomerDiscounts' AND DataId='" & dataId & "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLogDiscount_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddDiscount_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDiscount(False, String.Empty)
        Dim thisScript As String = "window.onload = function() {showProcessDiscount(); hideAllDiscountInputs();};"
        Session("selectedTabCustomer") = "list-discount"
        Try
            lblActionDiscount.Text = "Add"
            titleDiscount.InnerText = "Add Discount (All Products)"

            ddlDiscountType.SelectedValue = "Designs"

            BindDiscountData()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessDiscount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDiscount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddDiscount_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddDiscountCustom_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDiscount(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessDiscount(); visibleDiscountType(); };"
        Session("selectedTabCustomer") = "list-discount"
        Try
            lblActionDiscount.Text = "Add Custom"
            titleDiscount.InnerText = "Add Discount (Custom Product)"

            ddlDiscountType.SelectedValue = "Designs"

            BindDiscountData()
            BindDiscountDataB()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessDiscount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDiscount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddDiscountCustom_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessDiscount_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDiscount(False, String.Empty)
        Session("selectedTabCustomer") = "list-discount"
        Dim thisScript As String = "window.onload = function() { showProcessDiscount(); };"
        Try
            If msgErrorProcessDiscount.InnerText = "" Then
                If lblActionDiscount.Text = "Add" Then
                    Dim designData As DataTable = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray WHERE Type='Blinds' AND companyArray.VALUE='" & lblCompanyId.Text & "' AND Active=1 ORDER BY Id ASC")

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

                    Dim dataLog As Object() = {"CustomerDiscounts", lblIdDiscount.Text, Session("LoginId").ToString(), "Customer Discount Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessDiscount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDiscount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProcessDiscount_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeleteDiscount_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Discount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnResetDiscount_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataDiscount(customerId As String)
        MessageError_Discount(False, String.Empty)
        Try
            Dim thisQuery As String = "SELECT * FROM CustomerDiscounts WHERE CustomerId='" & customerId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END, DataId ASC"

            gvListDiscount.DataSource = settingClass.GetDataTable(thisQuery)
            gvListDiscount.DataBind()

            gvListDiscount.Columns(1).Visible = PageAction("Visible ID Discount")
            gvListDiscount.Columns(2).Visible = PageAction("Visible Type Discount")

            btnAddDiscount.Visible = PageAction("Add Discount")
            btnAddDiscountCustom.Visible = PageAction("Add Discount Custom")
            aResetDiscount.Visible = PageAction("Reset Discount")
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Discount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataDiscount", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDiscountData()
        ddlDiscountDataId.Items.Clear()
        Try
            ddlDiscountDataId.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray WHERE companyArray.VALUE='" & lblCompanyId.Text & "' AND Active=1 ORDER BY Name ASC")
            ddlDiscountDataId.DataTextField = "Name"
            ddlDiscountDataId.DataValueField = "Id"
            ddlDiscountDataId.DataBind()

            If ddlDiscountDataId.Items.Count > 1 Then
                ddlDiscountDataId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDiscountDataId.Items.Clear()
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDiscountData", ex.ToString()}
            mailingClass.WebError(dataMailing)
        End Try
    End Sub

    Protected Sub BindDiscountDataB()
        ddlDiscountDataIdB.Items.Clear()
        Try
            ddlDiscountDataIdB.DataSource = settingClass.GetDataTable("SELECT * FROM PriceProductGroups WHERE Active=1 ORDER BY Name ASC")
            ddlDiscountDataIdB.DataTextField = "Name"
            ddlDiscountDataIdB.DataValueField = "Id"
            ddlDiscountDataIdB.DataBind()

            If ddlDiscountDataIdB.Items.Count > 1 Then
                ddlDiscountDataIdB.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDiscountDataIdB.Items.Clear()
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDiscountDataB", ex.ToString()}
            mailingClass.WebError(dataMailing)
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
                    gvListDetailPromo.DataSource = settingClass.GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & dataId & "'  ORDER BY Id DESC")
                    gvListDetailPromo.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailPromo", thisScript, True)
                Catch ex As Exception
                    MessageError_DetailPromo(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_DetailPromo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetailPromo_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailPromo", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE Type='CustomerPromos' AND DataId='" & dataId & "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkLogPromo_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddPromo_Click(sender As Object, e As EventArgs)
        MessageError_ProcessPromo(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessPromo(); };"
        Session("selectedTabCustomer") = "list-promo"
        Try
            lblActionPromo.Text = "Add"
            titlePromo.InnerText = "Add Promo"

            BindListPromo()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessPromo", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessPromo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessPromo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnAddPromo_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessPromo", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessPromo_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDiscount(False, String.Empty)
        Session("selectedTabCustomer") = "list-promo"
        Dim thisScript As String = "window.onload = function() { showProcessPromo(); };"
        Try
            If ddlListPromo.SelectedValue = "" Then
                MessageError_ProcessPromo(True, "PROMO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessPromo", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessPromo.InnerText = "" Then
                If lblActionPromo.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerPromos ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerPromos VALUES (@Id, @CustomerId, @PromoId)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@PromoId", ddlListPromo.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerPromos", thisId, Session("LoginId").ToString(), "Customer Promo Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessPromo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessPromo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProcessPromo_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessPromo", thisScript, True)
        End Try
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnDeletePromo_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnResetPromo_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDataPromo(customerId As String)
        MessageError_Promo(False, String.Empty)
        Try
            Dim thisQuery As String = "SELECT CustomerPromos.*, Promos.Name AS PromoName FROM CustomerPromos LEFT JOIN Promos ON CustomerPromos.PromoId=Promos.Id WHERE CustomerPromos.CustomerId='" & customerId & "'"

            gvListPromo.DataSource = settingClass.GetDataTable(thisQuery)
            gvListPromo.DataBind()
            gvListPromo.Columns(1).Visible = PageAction("Visible ID Promo")

            btnAddPromo.Visible = PageAction("Add Promo")
            aResetPromo.Visible = PageAction("Reset Promo")
        Catch ex As Exception
            MessageError_Promo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Promo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataPromo", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindListPromo()
        ddlListPromo.Items.Clear()
        Try
            ddlListPromo.DataSource = settingClass.GetDataTable("SELECT * FROM Promos WHERE Active=1 ORDER BY Name ASC")
            ddlListPromo.DataTextField = "Name"
            ddlListPromo.DataValueField = "Id"
            ddlListPromo.DataBind()

            If ddlListPromo.Items.Count > 1 Then
                ddlListPromo.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlListPromo.Items.Clear()
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindListPromo", ex.ToString()}
            mailingClass.WebError(dataMailing)
        End Try
    End Sub

    Protected Function PromoValue(data As Decimal) As String
        If data > 0 Then Return data.ToString("G29", enUS) & "%"
        Return "ERROR"
    End Function

    Protected Function PromoTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Sub MessageError_Promo(visible As Boolean, message As String)
        divErrorPromo.Visible = visible : msgErrorPromo.InnerText = message
    End Sub

    Protected Sub MessageError_DetailPromo(visible As Boolean, message As String)
        divErrorDetailPromo.Visible = visible : msgErrorDetailPromo.InnerText = message
    End Sub

    Protected Sub MessageError_ProcessPromo(visible As Boolean, message As String)
        divErrorProcessPromo.Visible = visible : msgErrorProcessPromo.InnerText = message
    End Sub


    ' CUSTOMER PRODUCT ACCESS
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

                    BindDesignProduct()

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
                        dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "linkDetailProduct_Click", ex.ToString()}
                        mailingClass.WebError(dataMailing)
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
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerProductAccess WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='CustomerProductAccess' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.ExecuteNonQuery()
                End Using

                Dim desingId As String = settingClass.GetProductAccess(lblCompanyId.Text)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerProductAccess VALUES (@Id, @DesignId)", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Parameters.AddWithValue("@DesignId", desingId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Dim dataLog As Object() = {"CustomerProductAccess", lblId.Text, Session("LoginId").ToString(), "Reset Customer Product Access"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmitResetProduct_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
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

                Dim dataLog As Object() = {"CustomerProductAccess", lblIdProduct.Text, Session("LoginId").ToString(), "Customer Product Access Updated"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageErrorProcess_Product(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageErrorProcess_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnProccessProduct_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
        End Try
    End Sub

    Protected Sub BindDataProduct(customerId As String)
        MessageError_Product(False, String.Empty)
        Try
            gvListProduct.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerProductAccess WHERE Id='" + customerId + "'")
            gvListProduct.DataBind()

            aResetProduct.Visible = PageAction("Reset Product Access")
        Catch ex As Exception
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataProduct", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindDesignProduct()
        lbProductTags.Items.Clear()
        Try
            lbProductTags.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            lbProductTags.DataTextField = "Name"
            lbProductTags.DataValueField = "Id"
            lbProductTags.DataBind()
            If lbProductTags.Items.Count > 1 Then
                lbProductTags.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbProductTags.Items.Clear()
            dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDesignProduct", ex.ToString()}
            mailingClass.WebError(dataMailing)
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
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Product(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDetailProduct", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
        Return result
    End Function

    Protected Sub MessageErrorProcess_Product(visible As Boolean, message As String)
        divErrorProcessProduct.Visible = visible : msgErrorProcessProduct.InnerText = message
    End Sub

    Protected Sub MessageError_Product(visible As Boolean, message As String)
        divErrorProduct.Visible = visible : msgErrorProduct.InnerText = message
    End Sub


    ' CUSTOMER QUOTE
    Protected Sub BindDataQuote(customerId As String)
        MessageError_Quote(False, String.Empty)
        Try
            gvListQuote.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            gvListQuote.DataBind()
        Catch ex As Exception
            MessageError_Quote(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Quote(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindDataQuote", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Function BindQuoteddress(customerId As String) As String
        Dim result As String = String.Empty
        If Not customerId = "" Then
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")
            If thisData IsNot Nothing Then
                Dim address As String = thisData("Address").ToString()
                Dim suburb As String = thisData("Suburb").ToString()
                Dim state As String = thisData("State").ToString()
                Dim postCode As String = thisData("PostCode").ToString()
                Dim country As String = thisData("Country").ToString()

                result = String.Format("{0}, {1}, {2} {3} {4}", address, suburb, state, country, postCode)
            End If
        End If
        Return result
    End Function

    Protected Sub MessageError_Quote(visible As Boolean, message As String)
        divErrorQuote.Visible = visible : msgErrorQuote.InnerText = message
    End Sub

    Protected Sub AllMessageError(visible As Boolean, message As String)
        MessageError(visible, message)
        MessageError_CreateOrder(visible, message)
        MessageError_Log(visible, message)

        MessageError_Contact(visible, message)
        MessageError_ProcessContact(visible, message)

        MessageError_Address(visible, message)
        MessageError_ProcessAddress(visible, message)

        MessageError_Business(visible, message)
        MessageError_ProcessBusiness(visible, message)

        MessageError_Login(visible, message)
        MessageError_ProcesLogin(visible, message)
        MessageError_InstallerAccess(visible, message)

        MessageError_Discount(visible, message)
        MessageError_ProcessDiscount(visible, message)

        MessageError_Promo(visible, message)
        MessageError_ProcessPromo(visible, message)
        MessageError_DetailPromo(visible, message)

        MessageError_Product(visible, message)
        MessageErrorProcess_Product(visible, message)

        MessageError_Quote(visible, message)
    End Sub
End Class