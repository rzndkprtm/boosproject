Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
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

    <WebMethod()>
    Public Shared Function GetPromoDetail(id As String) As Object
        Dim settingClass As New SettingClass()

        Dim promoId As String = settingClass.GetItemData("SELECT PromoId FROM CustomerPromos WHERE Id='" & id & "'")

        Dim dt As DataTable = settingClass.GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'")

        Dim result As New List(Of Object)

        For Each dr As DataRow In dt.Rows
            Dim title As String = String.Empty
            If dr("Type").ToString() = "FrameColours" Then
                title = dr("DataId").ToString()
            Else
                title = settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", dr("Type").ToString(), dr("DataId").ToString()))
            End If
            result.Add(New With {.Type = title, .Discount = Convert.ToDecimal(dr("Discount")).ToString("G29") & "%"})
        Next
        Return result
    End Function

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
            aRecalculate.Visible = LoginAccess("Recalculate Order")
            aWelcome.Visible = LoginAccess("Welcome Customer")

            If customerId = "3" Then aWelcome.Visible = False

            divLevelSponsor.Visible = LoginAccess("Visible Level Sponsor")
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

    Protected Sub btnAddContact_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-contact"
        url = String.Format("~/setting/customer/contact/add?custid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnDeleteContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "list-contact"
        Try
            Dim thisId As String = txtIdContactDelete.Text

            Dim fullContact As String = settingClass.GetItemData("SELECT CONCAT('Contact Name: ', ISNULL(Name, ''), ', ', 'Email: ', ISNULL(Email, ''), ', ', 'Tags: ', ISNULL(Tags, '')) AS ThisContact FROM CustomerContacts WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerContacts WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerContacts' AND DataId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
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

            dataLog = {"CustomerContacts", thisId, Session("LoginId"), "Set As Primary Contact"}
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

    Protected Sub btnAddAddress_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-address"
        url = String.Format("~/setting/customer/address/add?custid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnDeleteAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Session("selectedTabCustomer") = "list-address"
        Try
            Dim thisId As String = txtIdAddressDelete.Text

            Dim fullDesc As String = settingClass.GetItemData("SELECT CONCAT('Description: ', ISNULL(Description, ''), ', ', 'Address: ', ISNULL(Address, ''), ', ', 'Suburb: ', ISNULL(Suburb, ''), ', ', 'State: ', ISNULL(State, ''), ', ', 'PostCode: ', ISNULL(PostCode, '')) AS FullDescription FROM CustomerAddress WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerAddress WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerAddress' AND DataId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
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

    Protected Sub btnAddBusiness_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-business"
        url = String.Format("~/setting/customer/business/add?custid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnDeleteBusiness_Click(sender As Object, e As EventArgs)
        MessageError_Business(False, String.Empty)
        Session("selectedTabCustomer") = "list-business"
        Try
            Dim thisId As String = txtIdBusinessDelete.Text

            Dim fullBusiness As String = settingClass.GetItemData("SELECT CONCAT('ABN Number: ', ISNULL(ABNNumber, ''), ', ', 'Registered Name: ', ISNULL(RegisteredName, '')) AS FullDescription FROM CustomerBusiness WHERE Id='" & thisId & "'")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerBusiness WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerBusiness' AND DataId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
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

    Protected Sub btnAddLogin_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "list-login"
        url = String.Format("~/setting/customer/login/add?custid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
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

            Dim activeDesc As String = "Login Has Been Activated"
            If active = 0 Then activeDesc = "Login Has Been Deactivated"

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

    Protected Sub btnResetPasswordLogin_Click(sender As Object, e As EventArgs)
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
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Login(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function TextActive_Login(active As Boolean) As String
        If active = True Then Return "Disable"
        Return "Enable"
    End Function

    Protected Sub MessageError_Login(visible As Boolean, message As String)
        divErrorLogin.Visible = visible : msgErrorLogin.InnerText = message
    End Sub

    ' END CUSTOMER LOGIN

    ' CUSTOMER DISCOUNT

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
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Discount(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
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

    ' END CUSTOMER DISCOUNT

    ' START CUSTOMER PROMO

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
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id;", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
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
        Catch ex As Exception
            MessageError_Promo(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Promo(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError_Promo(visible As Boolean, message As String)
        divErrorPromo.Visible = visible : msgErrorPromo.InnerText = message
    End Sub

    ' END CUSTOMER PROMO

    ' START CUSTOMER PRODUCT ACCESS
    Protected Sub gvListProduct_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "list-product"
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                url = String.Format("~/setting/customer/product/edit?productid={0}", dataId)
                Response.Redirect(url, False)
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
        MessageError_Contact(visible, message)
        MessageError_Address(visible, message)
        MessageError_Business(visible, message)
        MessageError_Login(visible, message)
        MessageError_Discount(visible, message)
        MessageError_Promo(visible, message)
        MessageError_Product(visible, message)
        MessageError_Quote(visible, message)
    End Sub
End Class