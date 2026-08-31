Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Customer_Service_Edit
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

        If String.IsNullOrEmpty(Request.QueryString("serviceid")) Then
            Response.Redirect("~/setting/customer/service", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("serviceid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
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

    Protected Sub BindData(serviceId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerServices WHERE Id='" & serviceId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/customer/service", False)
                Exit Sub
            End If

            Dim customerId As String = myData("CustomerId").ToString()
            Dim itemServiceId As String = myData("ServiceId").ToString()

            BindCustomer(customerId)
            BindService(itemServiceId)

            ddlCustomer.SelectedValue = myData("CustomerId").ToString()
            ddlService.SelectedValue = myData("ServiceId").ToString()
            ddlType.SelectedValue = myData("Type").ToString()

            txtSellPrice.Text = If(IsDBNull(myData("SellPrice")) OrElse myData("SellPrice") Is Nothing, "", Convert.ToDecimal(myData("SellPrice")).ToString("#,##0.##", enUS))
            txtBuyPrice.Text = If(IsDBNull(myData("BuyPrice")) OrElse myData("BuyPrice") Is Nothing, "", Convert.ToDecimal(myData("BuyPrice")).ToString("#,##0.##", enUS))
            txtFactoryPrice.Text = If(IsDBNull(myData("FactoryPrice")) OrElse myData("FactoryPrice") Is Nothing, "", Convert.ToDecimal(myData("FactoryPrice")).ToString("#,##0.##", enUS))

            ddlParameter.SelectedValue = myData("Parameter").ToString()
            ddlOperator.SelectedValue = myData("Operator").ToString()

            txtSellValue.Text = If(IsDBNull(myData("SellValue")) OrElse myData("SellValue") Is Nothing, "", Convert.ToDecimal(myData("SellValue")).ToString("#,##0.##", enUS))
            txtBuyValue.Text = If(IsDBNull(myData("BuyValue")) OrElse myData("BuyValue") Is Nothing, "", Convert.ToDecimal(myData("BuyValue")).ToString("#,##0.##", enUS))
            txtFactoryValue.Text = If(IsDBNull(myData("FactoryValue")) OrElse myData("FactoryValue") Is Nothing, "", Convert.ToDecimal(myData("FactoryValue")).ToString("#,##0.##", enUS))

            txtMinimumValue.Text = If(IsDBNull(myData("MinValue")) OrElse myData("MinValue") Is Nothing, "", Convert.ToDecimal(myData("MinValue")).ToString("#,##0.##", enUS))
            txtMaximumValue.Text = If(IsDBNull(myData("MaxValue")) OrElse myData("MaxValue") Is Nothing, "", Convert.ToDecimal(myData("MaxValue")).ToString("#,##0.##", enUS))

            ddlRegion.SelectedValue = myData("Region").ToString()

            BindForm(myData("Type").ToString())

            Dim allowCustom As Boolean = CBool(myData("UseCustom"))

            ddlType.Enabled = False
            txtSellPrice.Enabled = False
            txtBuyPrice.Enabled = False
            txtFactoryPrice.Enabled = False
            ddlParameter.Enabled = False
            ddlOperator.Enabled = False
            txtSellValue.Enabled = False
            txtBuyValue.Enabled = False
            txtFactoryValue.Enabled = False
            txtMinimumValue.Enabled = False
            txtMaximumValue.Enabled = False
            ddlRegion.Enabled = False

            If allowCustom = True Then
                ddlType.Enabled = True
                txtSellPrice.Enabled = True
                txtBuyPrice.Enabled = True
                txtFactoryPrice.Enabled = True
                ddlParameter.Enabled = True
                ddlOperator.Enabled = True
                txtSellValue.Enabled = True
                txtBuyValue.Enabled = True
                txtFactoryValue.Enabled = True
                txtMinimumValue.Enabled = True
                txtMaximumValue.Enabled = True
                ddlRegion.Enabled = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCustomer(customerId As String)
        ddlCustomer.Items.Clear()
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@CustomerId", If(customerId Is Nothing, CType(DBNull.Value, Object), customerId)),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            ddlCustomer.DataSource = settingClass.GetDataTableSP("sp_Customers_List_Dropdown_Setting", params)
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

    Protected Sub BindService(serviceId As String)
        ddlService.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(serviceId) Then

                ddlService.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceServices WHERE Id='" & serviceId & "' AND Status='Active'")
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

    Protected Sub BindForm(type As String)
        Try
            divDefaultPrice.Visible = False
            divOperator.Visible = False
            divFormulaValue.Visible = False

            If type = "Price" Then
                divDefaultPrice.Visible = True
            End If

            If type = "Formula" Then
                divOperator.Visible = True
                divFormulaValue.Visible = True
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
