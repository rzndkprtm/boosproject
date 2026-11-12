Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Customer_Discount_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Private Property DiscountTable As DataTable
        Get
            If Session("DiscountTable") Is Nothing Then
                Dim dt As New DataTable
                dt.Columns.Add("Product")
                dt.Columns.Add("Method")
                dt.Columns.Add("Discount1")
                dt.Columns.Add("Discount2")
                dt.Columns.Add("Discount3")
                dt.Columns.Add("Discount4")
                dt.Columns.Add("Discount5")

                Session("DiscountTable") = dt
            End If

            Return DirectCast(Session("DiscountTable"), DataTable)
        End Get
        Set(value As DataTable)
            Session("DiscountTable") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/discount", False)
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

            DiscountTable.Rows.Clear()
            DiscountTable.Rows.Add("", "", "", "", "", "", "")

            ddlType.SelectedValue = ""

            BindGrid()
        End If
    End Sub

    Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim customerId As String = ddlCustomer.SelectedValue
            Dim discType As String = ddlType.SelectedValue

            DiscountTable.Rows.Clear()

            If String.IsNullOrEmpty(discType) Then
                DiscountTable.Rows.Add("", "", "", "", "", "", "")
                BindGrid()
                Exit Sub
            End If

            If discType = "PriceProductGroups" Then
                Dim dtProduct As DataTable = GetProductData(customerId, discType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = DiscountTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = ""
                        newRow("Discount1") = ""
                        newRow("Discount2") = ""
                        newRow("Discount3") = ""
                        newRow("Discount4") = ""
                        newRow("Discount5") = ""

                        DiscountTable.Rows.Add(newRow)
                    Next
                Else
                    DiscountTable.Rows.Add("", "", "", "", "", "", "")
                End If

                BindGrid()
                Exit Sub
            End If

            Dim totalDiscount As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) AS Total FROM CustomerDiscounts WHERE CustomerId='" & customerId & "'")

            If totalDiscount = 0 Then
                Dim dtProduct As DataTable = GetProductData(customerId, discType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = DiscountTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = ""
                        newRow("Discount1") = ""
                        newRow("Discount2") = ""
                        newRow("Discount3") = ""
                        newRow("Discount4") = ""
                        newRow("Discount5") = ""

                        DiscountTable.Rows.Add(newRow)
                    Next
                Else
                    DiscountTable.Rows.Add("", "", "", "", "", "", "")
                End If
            Else
                DiscountTable.Rows.Add("", "", "", "", "", "", "")
            End If

            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim customerId As String = ddlCustomer.SelectedValue
            Dim discType As String = ddlType.SelectedValue

            DiscountTable.Rows.Clear()

            If String.IsNullOrEmpty(discType) Then
                DiscountTable.Rows.Add("", "", "", "", "", "", "")
                BindGrid()
                Exit Sub
            End If

            If discType = "PriceProductGroups" Then
                Dim dtProduct As DataTable = GetProductData(customerId, discType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = DiscountTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = "Percent"
                        newRow("Discount1") = ""
                        newRow("Discount2") = ""
                        newRow("Discount3") = ""
                        newRow("Discount4") = ""
                        newRow("Discount5") = ""

                        DiscountTable.Rows.Add(newRow)
                    Next
                Else
                    DiscountTable.Rows.Add("", "", "", "", "", "", "")
                End If

                BindGrid()
                Exit Sub
            End If

            Dim totalDiscount As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) AS Total FROM CustomerDiscounts WHERE CustomerId='" & customerId & "'")

            If totalDiscount = 0 Then
                Dim dtProduct As DataTable = GetProductData(customerId, discType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = DiscountTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = "Percent"
                        newRow("Discount1") = ""
                        newRow("Discount2") = ""
                        newRow("Discount3") = ""
                        newRow("Discount4") = ""
                        newRow("Discount5") = ""

                        DiscountTable.Rows.Add(newRow)
                    Next
                Else
                    DiscountTable.Rows.Add("", "", "", "", "", "", "")
                End If
            Else
                DiscountTable.Rows.Add("", "", "", "", "", "", "")
            End If

            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnApplyDiscount_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim discount As String = txtApplyDiscount.Text.Trim()

            If String.IsNullOrEmpty(discount) Then
                MessageError(True, "PLEASE ENTER DISCOUNT VALUE !")
                Exit Sub
            End If

            SaveGrid()

            Dim dt As DataTable = DiscountTable

            For Each dr As DataRow In dt.Rows
                dr("Discount1") = discount
            Next

            DiscountTable = dt

            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub rptDiscount_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)

                Dim ddlProduct As DropDownList = CType(e.Item.FindControl("ddlProduct"), DropDownList)
                Dim ddlMethod As DropDownList = CType(e.Item.FindControl("ddlMethod"), DropDownList)
                Dim txtDiscount1 As TextBox = CType(e.Item.FindControl("txtDiscount1"), TextBox)
                Dim txtDiscount2 As TextBox = CType(e.Item.FindControl("txtDiscount2"), TextBox)
                Dim txtDiscount3 As TextBox = CType(e.Item.FindControl("txtDiscount3"), TextBox)
                Dim txtDiscount4 As TextBox = CType(e.Item.FindControl("txtDiscount4"), TextBox)
                Dim txtDiscount5 As TextBox = CType(e.Item.FindControl("txtDiscount5"), TextBox)

                If ddlProduct Is Nothing Then Exit Sub

                If ddlMethod IsNot Nothing Then
                    ddlMethod.SelectedValue = drv("Method").ToString()
                End If

                If txtDiscount1 IsNot Nothing Then
                    txtDiscount1.Text = drv("Discount1").ToString()
                End If
                If txtDiscount2 IsNot Nothing Then
                    txtDiscount2.Text = drv("Discount2").ToString()
                End If
                If txtDiscount3 IsNot Nothing Then
                    txtDiscount3.Text = drv("Discount3").ToString()
                End If
                If txtDiscount4 IsNot Nothing Then
                    txtDiscount4.Text = drv("Discount4").ToString()
                End If
                If txtDiscount5 IsNot Nothing Then
                    txtDiscount5.Text = drv("Discount5").ToString()
                End If

                If ddlType.SelectedValue <> "" Then
                    BindProduct(ddlCustomer.SelectedValue, ddlType.SelectedValue, ddlProduct)

                    Dim productId As String = drv("Product").ToString()
                    If Not String.IsNullOrEmpty(productId) Then
                        Dim item As ListItem = ddlProduct.Items.FindByValue(productId)
                        If item IsNot Nothing Then
                            ddlProduct.SelectedValue = item.Value
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub rptDiscount_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName <> "DeleteRow" Then Exit Sub
            SaveGrid()

            Dim index As Integer
            If Not Integer.TryParse(e.CommandArgument.ToString(), index) Then
                Exit Sub
            End If
            If index >= 0 AndAlso index < DiscountTable.Rows.Count Then
                DiscountTable.Rows.RemoveAt(index)
            End If
            If DiscountTable.Rows.Count = 0 Then
                DiscountTable.Rows.Add("", "", "", "", "", "", "")
            End If
            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Try
            SaveGrid()
            DiscountTable.Rows.Add("", "", "", "", "", "", "")
            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnSubmitAgain_Click(sender As Object, e As EventArgs)
        Process("Again")
    End Sub

    Protected Sub btnSubmitFinish_Click(sender As Object, e As EventArgs)
        Process()
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/customer/discount"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub Process(Optional action As String = "")
        MessageError(False, String.Empty)
        Try
            SaveGrid()
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "DISCOUNT TYPE IS REQUIRED !")
                Exit Sub
            End If

            Dim dt As DataTable = DiscountTable
            If dt.Rows.Count = 0 Then
                MessageError(True, "AT LEAST ONE DISCOUNT ROW IS REQUIRED !")
                Exit Sub
            End If

            For i As Integer = 0 To dt.Rows.Count - 1
                Dim product As String = dt.Rows(i)("Product").ToString().Trim()
                Dim method As String = dt.Rows(i)("Method").ToString().Trim()
                Dim discount1 As String = dt.Rows(i)("Discount1").ToString().Trim()
                Dim discount2 As String = dt.Rows(i)("Discount2").ToString().Trim()
                Dim discount3 As String = dt.Rows(i)("Discount3").ToString().Trim()
                Dim discount4 As String = dt.Rows(i)("Discount4").ToString().Trim()
                Dim discount5 As String = dt.Rows(i)("Discount5").ToString().Trim()

                Dim rowNumber As Integer = i + 1

                If product = "" AndAlso discount1 = "" AndAlso discount2 = "" AndAlso discount3 = "" AndAlso discount4 = "" AndAlso discount5 = "" AndAlso method = "" Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT, METHOD AND DISCOUNT 1 ARE REQUIRED !", rowNumber))
                    Exit Sub
                End If

                If product = "" Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT IS REQUIRED !", rowNumber))
                    Exit Sub
                End If

                If method = "" Then
                    MessageError(True, String.Format("ROW {0}: METHOD IS REQUIRED !", rowNumber))
                    Exit Sub
                End If

                If discount1 = "" Then
                    MessageError(True, String.Format("ROW {0}: DISCOUNT 1 IS REQUIRED !", rowNumber))
                    Exit Sub
                End If
            Next

            Dim seenProducts As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim product As String = dt.Rows(i)("Product").ToString().Trim()
                Dim rowNumber As Integer = i + 1

                If Not seenProducts.Add(product) Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT ALREADY EXISTS IN ANOTHER ROW !", rowNumber))
                    Exit Sub
                End If
            Next

            For Each dr As DataRow In dt.Rows
                If dr("Product").ToString() = "" Then Continue For
                If dr("Method").ToString() = "" Then Continue For
                If dr("Discount1").ToString() = "" Then Continue For

                Dim discount1 As Decimal = Convert.ToDecimal(dr("Discount1"), CultureInfo.GetCultureInfo("en-US"))
                Dim discount2 As Decimal = 0
                Dim discount3 As Decimal = 0
                Dim discount4 As Decimal = 0
                Dim discount5 As Decimal = 0

                If Not String.IsNullOrWhiteSpace(dr("Discount2").ToString()) Then
                    discount2 = Convert.ToDecimal(dr("Discount2"), CultureInfo.GetCultureInfo("en-US"))
                End If

                If Not String.IsNullOrWhiteSpace(dr("Discount3").ToString()) Then
                    discount3 = Convert.ToDecimal(dr("Discount3"), CultureInfo.GetCultureInfo("en-US"))
                End If

                If Not String.IsNullOrWhiteSpace(dr("Discount4").ToString()) Then
                    discount4 = Convert.ToDecimal(dr("Discount4"), CultureInfo.GetCultureInfo("en-US"))
                End If

                If Not String.IsNullOrWhiteSpace(dr("Discount5").ToString()) Then
                    discount5 = Convert.ToDecimal(dr("Discount5"), CultureInfo.GetCultureInfo("en-US"))
                End If

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerDiscounts ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("INSERT INTO CustomerDiscounts VALUES (@Id, @CustomerId, @Type, @Method, @DataId, @Discount1, @Discount2, @Discount3, @Discount4, @Discount5, NULL)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Method", dr("Method").ToString())
                        thisCmd.Parameters.AddWithValue("@DataId", dr("Product").ToString())
                        thisCmd.Parameters.AddWithValue("@Discount1", discount1)
                        thisCmd.Parameters.AddWithValue("@Discount2", discount2)
                        thisCmd.Parameters.AddWithValue("@Discount3", discount3)
                        thisCmd.Parameters.AddWithValue("@Discount4", discount4)
                        thisCmd.Parameters.AddWithValue("@Discount5", discount5)

                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                settingClass.Logs({"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Created"})
            Next

            url = "~/setting/customer/discount"
            If lblReturnPage.Text = "detail" Then
                url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
            End If
            If action = "Again" Then
                url = String.Format("~/setting/customer/discount/add?custid={0}", ddlCustomer.SelectedValue)
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/customer/discount/add?custid={0}&returnpage=detail", ddlCustomer.SelectedValue)
                End If
            End If
            Response.Redirect(url, False)
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
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindProduct(customerId As String, discType As String, ddl As DropDownList)
        Try
            If Not String.IsNullOrEmpty(discType) Then

                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@CustomerId", customerId),
                    New SqlParameter("@DiscType", discType)
                }
                Dim dt As DataTable = settingClass.GetDataTableSP("sp_BindDiscountData", params)

                ddl.SelectedIndex = -1
                ddl.ClearSelection()
                ddl.Items.Clear()

                ddl.DataSource = Nothing
                ddl.DataBind()

                ddl.DataSource = dt
                ddl.DataTextField = "Name"
                ddl.DataValueField = "Id"
                ddl.DataBind()

                ddl.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function GetProductData(customerId As String, discType As String) As DataTable
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@CustomerId", customerId),
                New SqlParameter("@DiscType", discType)
            }
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_BindDiscountData", params)
            Return dt
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            Return New DataTable()
        End Try
    End Function

    Protected Sub BindGrid()
        Try
            rptDiscount.DataSource = DiscountTable
            rptDiscount.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub SaveGrid()
        Try
            Dim dt As DataTable = DiscountTable

            While dt.Rows.Count < rptDiscount.Items.Count
                dt.Rows.Add("", "", "", "", "", "", "")
            End While

            For i As Integer = 0 To rptDiscount.Items.Count - 1
                Dim item As RepeaterItem = rptDiscount.Items(i)

                Dim ddlProduct As DropDownList = CType(item.FindControl("ddlProduct"), DropDownList)
                Dim ddlMethod As DropDownList = CType(item.FindControl("ddlMethod"), DropDownList)
                Dim txtDiscount1 As TextBox = CType(item.FindControl("txtDiscount1"), TextBox)
                Dim txtDiscount2 As TextBox = CType(item.FindControl("txtDiscount2"), TextBox)
                Dim txtDiscount3 As TextBox = CType(item.FindControl("txtDiscount3"), TextBox)
                Dim txtDiscount4 As TextBox = CType(item.FindControl("txtDiscount4"), TextBox)
                Dim txtDiscount5 As TextBox = CType(item.FindControl("txtDiscount5"), TextBox)

                If ddlProduct Is Nothing OrElse ddlMethod Is Nothing OrElse txtDiscount1 Is Nothing OrElse txtDiscount2 Is Nothing OrElse txtDiscount3 Is Nothing OrElse txtDiscount4 Is Nothing OrElse txtDiscount5 Is Nothing Then
                    Continue For
                End If

                dt.Rows(i)("Method") = ddlMethod.SelectedValue
                dt.Rows(i)("Product") = ddlProduct.SelectedValue
                dt.Rows(i)("Discount1") = txtDiscount1.Text.Trim()
                dt.Rows(i)("Discount2") = txtDiscount2.Text.Trim()
                dt.Rows(i)("Discount3") = txtDiscount3.Text.Trim()
                dt.Rows(i)("Discount4") = txtDiscount4.Text.Trim()
                dt.Rows(i)("Discount5") = txtDiscount5.Text.Trim()
            Next

            DiscountTable = dt

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
