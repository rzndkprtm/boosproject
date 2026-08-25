Imports System.Data
Imports System.Globalization

Partial Class Setting_Customer_Service_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/service", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("custid")) Then
            lblCustomerId.Text = Request.QueryString("custid").ToString()
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer(lblCustomerId.Text)
            BindService(lblCustomerId.Text)
            BindServiceData(ddlService.SelectedValue)
            BindForm(ddlType.SelectedValue)
        End If
    End Sub

    Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindService(ddlCustomer.SelectedValue)
    End Sub

    Protected Sub ddlService_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindServiceData(ddlService.SelectedValue)
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindForm(ddlType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try

        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/customer/service"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindCustomer(customerId As String)
        ddlCustomer.Items.Clear()
        Try
            Dim role As String = String.Empty
            If Session("RoleName") = "Sales" Then
                role = "AND CompanyId='" & Session("CompanyId").ToString() & "'"
                If Session("LevelName") = "Member" Then
                    role = "AND (Id = '" & Session("CustomerId") & "' OR EXISTS (SELECT 1 FROM STRING_SPLIT(Operator, ',') WHERE value = '" & Session("LoginId") & "'))"
                End If
            End If

            ddlCustomer.DataSource = settingClass.GetDataTable(String.Format("SELECT Id, Name FROM Customers WHERE Status='Active' {0} ORDER BY Name ASC", role))
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
            ddlCustomer.SelectedValue = customerId

            ddlCustomer.Enabled = False
            If String.IsNullOrEmpty(customerId) Then ddlCustomer.Enabled = True
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindService(customerId As String)
        ddlService.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Dim companyDetailId As String = settingClass.GetItemData("SELECT CompanyDetailId FROM Customers WHERE Id='" & customerId & "'")

                ddlService.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceServices CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS thisArray WHERE thisArray.VALUE='" & companyDetailId & "' AND Status='Active'")
                ddlService.DataTextField = "Name"
                ddlService.DataValueField = "Id"
                ddlService.DataBind()

                If ddlService.Items.Count > 1 Then
                    ddlService.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlService.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindServiceData(serviceId As String)
        Try
            If Not String.IsNullOrEmpty(serviceId) Then
                Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceServices WHERE Id='" & serviceId & "'")

                ddlType.SelectedValue = myData("Type").ToString()
                ddlParameter.SelectedValue = myData("Parameter").ToString()
                ddlOperator.SelectedValue = myData("Operator").ToString()
                ddlRegion.SelectedValue = myData("Region").ToString()
                txtDescription.Text = myData("Description.").ToString()

                Dim allowCustom As Boolean = CBool(myData("AllowCustom"))

                txtBuyPrice.Text = If(IsDBNull(myData("DefaultBuyPrice")) OrElse myData("DefaultBuyPrice") Is Nothing, "", Convert.ToDecimal(myData("DefaultBuyPrice")).ToString("#,##0.##", enUS))
                txtSellPrice.Text = If(IsDBNull(myData("DefaultSellPrice")) OrElse myData("DefaultSellPrice") Is Nothing, "", Convert.ToDecimal(myData("DefaultSellPrice")).ToString("#,##0.##", enUS))
                txtBuyValue.Text = If(IsDBNull(myData("BuyValue")) OrElse myData("BuyValue") Is Nothing, "", Convert.ToDecimal(myData("BuyValue")).ToString("#,##0.##", enUS))
                txtSellValue.Text = If(IsDBNull(myData("SellValue")) OrElse myData("SellValue") Is Nothing, "", Convert.ToDecimal(myData("SellValue")).ToString("#,##0.##", enUS))
                txtMinimumValue.Text = If(IsDBNull(myData("MinValue")) OrElse myData("MinValue") Is Nothing, "", Convert.ToDecimal(myData("MinValue")).ToString("#,##0.##", enUS))
                txtMaximumValue.Text = If(IsDBNull(myData("MaxValue")) OrElse myData("MaxValue") Is Nothing, "", Convert.ToDecimal(myData("MaxValue")).ToString("#,##0.##", enUS))

                BindForm(myData("Type").ToString())

                ddlType.Enabled = False
                txtBuyPrice.Enabled = False
                txtSellPrice.Enabled = False
                ddlParameter.Enabled = False
                ddlOperator.Enabled = False
                txtBuyValue.Enabled = False
                txtSellValue.Enabled = False
                txtMinimumValue.Enabled = False
                txtMaximumValue.Enabled = False
                ddlRegion.Enabled = False

                If allowCustom = True Then
                    ddlType.Enabled = True
                    txtBuyPrice.Enabled = True
                    txtSellPrice.Enabled = True
                    ddlParameter.Enabled = True
                    ddlOperator.Enabled = True
                    txtBuyValue.Enabled = True
                    txtSellValue.Enabled = True
                    txtMinimumValue.Enabled = True
                    txtMaximumValue.Enabled = True
                    ddlRegion.Enabled = True
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindForm(type As String)
        Try
            divDefaultBuy.Visible = False
            divDefaultSell.Visible = False
            divFormula.Visible = False

            If type = "Price" Then
                divDefaultBuy.Visible = True
                divDefaultSell.Visible = True
            End If

            If type = "Formula" Then
                divFormula.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
End Class
