Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Price_Service_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/service", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("serviceid")) Then
            Response.Redirect("~/setting/price/service", False)
            Exit Sub
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
            If lbCompanyDetail.SelectedValue = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If txtName.Text = "" Then
                MessageError(True, "SERVICE NAME IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                If ddlType.SelectedValue = "Price" Then
                    ddlOperator.SelectedValue = ""
                    txtBuyValue.Text = String.Empty
                    txtSellValue.Text = String.Empty
                End If
                If ddlType.SelectedValue = "Formula" Then
                    txtBuyPrice.Text = String.Empty
                    txtSellPrice.Text = String.Empty
                End If

                Dim companyDetailId As String = String.Empty
                If Not String.IsNullOrEmpty(lbCompanyDetail.SelectedValue) Then
                    companyDetailId = String.Join(",", lbCompanyDetail.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE PriceServices SET  CompanyDetailId=@CompanyDetailId, Name=@Name, Type=@Type, DefaultBuyPrice=@DefaultBuyPrice, DefaultSellPrice=@DefaultSellPrice, Parameter=@Parameter, Operator=@Operator, BuyValue=@BuyValue, SellValue=@SellValue, MinValue=@MinValue, MaxValue=@MaxValue, Region=@Region, AutoCreate=@AutoCreate, AllowCustom=@AllowCustom, Description=@Description, Status=@Status WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetailId)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text)
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@DefaultBuyPrice", If(String.IsNullOrEmpty(txtBuyPrice.Text), CType(DBNull.Value, Object), txtBuyPrice.Text))
                        thisCmd.Parameters.AddWithValue("@DefaultSellPrice", If(String.IsNullOrEmpty(txtSellPrice.Text), CType(DBNull.Value, Object), txtSellPrice.Text))
                        thisCmd.Parameters.AddWithValue("@Parameter", ddlParameter.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Operator", ddlOperator.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@BuyValue", If(String.IsNullOrEmpty(txtBuyValue.Text), CType(DBNull.Value, Object), txtBuyValue.Text))
                        thisCmd.Parameters.AddWithValue("@SellValue", If(String.IsNullOrEmpty(txtSellValue.Text), CType(DBNull.Value, Object), txtSellValue.Text))
                        thisCmd.Parameters.AddWithValue("@MinValue", If(String.IsNullOrEmpty(txtMinimumValue.Text), CType(DBNull.Value, Object), txtMinimumValue.Text))
                        thisCmd.Parameters.AddWithValue("@MaxValue", If(String.IsNullOrEmpty(txtMinimumValue.Text), CType(DBNull.Value, Object), txtMinimumValue.Text))
                        thisCmd.Parameters.AddWithValue("@Region", ddlRegion.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@AutoCreate", ddlAutoCreate.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@AllowCustom", ddlAllowCustom.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"OrderDetails", lblId.Text, Session("LoginId"), "Price Service Updated"}
                settingClass.Logs(dataLog)

                If ddlAutoCreate.SelectedValue = "1" AndAlso lblAutoCreate.Text = "0" Then
                    Dim thisData As DataTable = settingClass.GetDataTable("SELECT Id FROM Customers WHERE CompanyDetailId IN (SELECT TRY_CAST(value AS INT) FROM STRING_SPLIT('" & companyDetailId & "', ','));")
                    For Each row As DataRow In thisData.Rows
                        Dim customerId As Integer = Convert.ToInt32(row("Id"))
                        Dim customerServiceId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerServices ORDER BY Id DESC")

                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerServices SELECT @Id, @CustomerId, Id, AllowCustom, Type, DefaultBuyPrice, DefaultSellPrice, Parameter, Operator, BuyValue, SellValue, MinValue, MaxValue, Region FROM PriceServices WHERE Id=@ServiceId", thisConn)
                                thisCmd.Parameters.AddWithValue("@Id", customerServiceId)
                                thisCmd.Parameters.AddWithValue("@CustomerId", customerId)
                                thisCmd.Parameters.AddWithValue("@ServiceId", lblId.Text)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using
                    Next
                End If

                Response.Redirect("~/setting/price/service", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/service", False)
    End Sub

    Protected Sub BindData(serviceId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceServices WHERE Id='" & serviceId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/price/service", False)
                Exit Sub
            End If

            BindCompanyDetail()

            If Not myData("CompanyDetailId").ToString() = "" Then
                Dim companyDetailArray() As String = myData("CompanyDetailId").ToString().Split(",")
                For Each i In companyDetailArray
                    If Not String.IsNullOrEmpty(i) Then
                        Dim item = lbCompanyDetail.Items.FindByValue(i)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If

            txtName.Text = myData("Name").ToString()
            ddlType.SelectedValue = myData("Type").ToString()
            ddlParameter.SelectedValue = myData("Parameter").ToString()
            ddlOperator.SelectedValue = myData("Operator").ToString()
            ddlRegion.SelectedValue = myData("Region").ToString()
            ddlAutoCreate.SelectedValue = Convert.ToInt32(myData("AutoCreate"))
            lblAutoCreate.Text = Convert.ToInt32(myData("AutoCreate"))
            ddlAllowCustom.SelectedValue = Convert.ToInt32(myData("AllowCustom"))
            ddlStatus.SelectedValue = myData("Status").ToString()
            txtDescription.Text = myData("Description").ToString()

            txtBuyPrice.Text = If(IsDBNull(myData("DefaultBuyPrice")) OrElse myData("DefaultBuyPrice") Is Nothing, "", Convert.ToDecimal(myData("DefaultBuyPrice")).ToString("#,##0.##", enUS))
            txtSellPrice.Text = If(IsDBNull(myData("DefaultSellPrice")) OrElse myData("DefaultSellPrice") Is Nothing, "", Convert.ToDecimal(myData("DefaultSellPrice")).ToString("#,##0.##", enUS))
            txtBuyValue.Text = If(IsDBNull(myData("BuyValue")) OrElse myData("BuyValue") Is Nothing, "", Convert.ToDecimal(myData("BuyValue")).ToString("#,##0.##", enUS))
            txtSellValue.Text = If(IsDBNull(myData("SellValue")) OrElse myData("SellValue") Is Nothing, "", Convert.ToDecimal(myData("SellValue")).ToString("#,##0.##", enUS))
            txtMinimumValue.Text = If(IsDBNull(myData("MinValue")) OrElse myData("MinValue") Is Nothing, "", Convert.ToDecimal(myData("MinValue")).ToString("#,##0.##", enUS))
            txtMaximumValue.Text = If(IsDBNull(myData("MaxValue")) OrElse myData("MaxValue") Is Nothing, "", Convert.ToDecimal(myData("MaxValue")).ToString("#,##0.##", enUS))

            BindForm(myData("Type").ToString())
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

    Protected Sub BindCompanyDetail()
        lbCompanyDetail.Items.Clear()
        Try
            lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM CompanyDetails ORDER BY Name ASC")
            lbCompanyDetail.DataTextField = "Name"
            lbCompanyDetail.DataValueField = "Id"
            lbCompanyDetail.DataBind()

            If lbCompanyDetail.Items.Count > 1 Then
                lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbCompanyDetail.Items.Clear()
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
