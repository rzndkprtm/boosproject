Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Customer_Markup_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Private Property MarkupTable As DataTable
        Get
            If Session("MarkupTable") Is Nothing Then
                Dim dt As New DataTable
                dt.Columns.Add("Product")
                dt.Columns.Add("Method")
                dt.Columns.Add("Markup1")
                dt.Columns.Add("Markup2")
                dt.Columns.Add("Markup3")
                dt.Columns.Add("Markup4")
                dt.Columns.Add("Markup5")

                Session("MarkupTable") = dt
            End If

            Return DirectCast(Session("MarkupTable"), DataTable)
        End Get
        Set(value As DataTable)
            Session("MarkupTable") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/markup", False)
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

            MarkupTable.Rows.Clear()
            MarkupTable.Rows.Add("", "", "", "", "", "", "")

            ddlType.SelectedValue = ""

            BindGrid()
        End If
    End Sub

    Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim customerId As String = ddlCustomer.SelectedValue
            Dim markupType As String = ddlType.SelectedValue

            MarkupTable.Rows.Clear()

            If String.IsNullOrEmpty(markupType) Then
                MarkupTable.Rows.Add("", "", "", "", "", "", "")
                BindGrid()
                Exit Sub
            End If

            If markupType = "PriceProductGroups" Then
                Dim dtProduct As DataTable = GetProductData(customerId, markupType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = MarkupTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = ""
                        newRow("Markup1") = ""
                        newRow("Markup2") = ""
                        newRow("Markup3") = ""
                        newRow("Markup4") = ""
                        newRow("Markup5") = ""

                        MarkupTable.Rows.Add(newRow)
                    Next
                Else
                    MarkupTable.Rows.Add("", "", "", "", "", "", "")
                End If

                BindGrid()

                Exit Sub
            End If

            Dim totalMarkup As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) AS Total FROM CustomerMarkups WHERE CustomerId='" & customerId & "'")
            If totalMarkup = 0 Then
                Dim dtProduct As DataTable = GetProductData(customerId, markupType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = MarkupTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = ""
                        newRow("Markup1") = ""
                        newRow("Markup2") = ""
                        newRow("Markup3") = ""
                        newRow("Markup4") = ""
                        newRow("Markup5") = ""

                        MarkupTable.Rows.Add(newRow)
                    Next
                Else
                    MarkupTable.Rows.Add("", "", "", "", "", "", "")
                End If
            Else
                MarkupTable.Rows.Add("", "", "", "", "", "", "")
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
            Dim markupType As String = ddlType.SelectedValue

            MarkupTable.Rows.Clear()

            If String.IsNullOrEmpty(markupType) Then
                MarkupTable.Rows.Add("", "", "", "", "", "", "")
                BindGrid()
                Exit Sub
            End If

            If markupType = "PriceProductGroups" Then
                Dim dtProduct As DataTable = GetProductData(customerId, markupType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = MarkupTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = "Percent"
                        newRow("Markup1") = ""
                        newRow("Markup2") = ""
                        newRow("Markup3") = ""
                        newRow("Markup4") = ""
                        newRow("Markup5") = ""

                        MarkupTable.Rows.Add(newRow)
                    Next
                Else
                    MarkupTable.Rows.Add("", "", "", "", "", "", "")
                End If

                BindGrid()
                Exit Sub
            End If

            Dim totalMarkup As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) AS Total FROM CustomerMarkups WHERE CustomerId='" & customerId & "'")
            If totalMarkup = 0 Then
                Dim dtProduct As DataTable = GetProductData(customerId, markupType)

                If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                    For Each productRow As DataRow In dtProduct.Rows
                        Dim newRow As DataRow = MarkupTable.NewRow()
                        newRow("Product") = productRow("Id").ToString()
                        newRow("Method") = "Percent"
                        newRow("Markup1") = ""
                        newRow("Markup2") = ""
                        newRow("Markup3") = ""
                        newRow("Markup4") = ""
                        newRow("Markup5") = ""

                        MarkupTable.Rows.Add(newRow)
                    Next
                Else
                    MarkupTable.Rows.Add("", "", "", "", "", "", "")
                End If
            Else
                MarkupTable.Rows.Add("", "", "", "", "", "", "")
            End If

            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnApplyMarkup_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim markup As String = txtApplyMarkup.Text.Trim()

            If String.IsNullOrEmpty(markup) Then
                MessageError(True, "PLEASE ENTER MARKUP VALUE !")
                Exit Sub
            End If

            SaveGrid()

            Dim dt As DataTable = MarkupTable

            For Each dr As DataRow In dt.Rows
                dr("Markup1") = markup
            Next

            MarkupTable = dt

            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub rptMarkup_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)
                Dim ddlProduct As DropDownList = CType(e.Item.FindControl("ddlProduct"), DropDownList)
                Dim ddlMethod As DropDownList = CType(e.Item.FindControl("ddlMethod"), DropDownList)
                Dim txtMarkup1 As TextBox = CType(e.Item.FindControl("txtMarkup1"), TextBox)
                Dim txtMarkup2 As TextBox = CType(e.Item.FindControl("txtMarkup2"), TextBox)
                Dim txtMarkup3 As TextBox = CType(e.Item.FindControl("txtMarkup3"), TextBox)
                Dim txtMarkup4 As TextBox = CType(e.Item.FindControl("txtMarkup4"), TextBox)
                Dim txtMarkup5 As TextBox = CType(e.Item.FindControl("txtMarkup5"), TextBox)

                If ddlProduct Is Nothing Then Exit Sub

                If ddlMethod IsNot Nothing Then
                    ddlMethod.SelectedValue = drv("Method").ToString()
                End If
                If txtMarkup1 IsNot Nothing Then
                    txtMarkup1.Text = drv("Markup1").ToString()
                End If
                If txtMarkup2 IsNot Nothing Then
                    txtMarkup2.Text = drv("Markup2").ToString()
                End If
                If txtMarkup3 IsNot Nothing Then
                    txtMarkup3.Text = drv("Markup3").ToString()
                End If
                If txtMarkup4 IsNot Nothing Then
                    txtMarkup4.Text = drv("Markup4").ToString()
                End If
                If txtMarkup5 IsNot Nothing Then
                    txtMarkup5.Text = drv("Markup5").ToString()
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

    Protected Sub rptMarkup_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName <> "DeleteRow" Then Exit Sub
            SaveGrid()

            Dim index As Integer
            If Not Integer.TryParse(e.CommandArgument.ToString(), index) Then
                Exit Sub
            End If
            If index >= 0 AndAlso index < MarkupTable.Rows.Count Then
                MarkupTable.Rows.RemoveAt(index)
            End If
            If MarkupTable.Rows.Count = 0 Then
                MarkupTable.Rows.Add("", "", "", "", "", "", "")
            End If
            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Try
            SaveGrid()
            MarkupTable.Rows.Add("", "", "", "", "", "", "")
            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
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
        url = "~/setting/customer/markup"
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
                MessageError(True, "MARKUP TYPE IS REQUIRED !")
                Exit Sub
            End If

            Dim dt As DataTable = MarkupTable
            If dt.Rows.Count = 0 Then
                MessageError(True, "AT LEAST ONE MARKUP ROW IS REQUIRED !")
                Exit Sub
            End If

            For i As Integer = 0 To dt.Rows.Count - 1
                Dim product As String = dt.Rows(i)("Product").ToString().Trim()
                Dim method As String = dt.Rows(i)("Method").ToString().Trim()
                Dim markup1 As String = dt.Rows(i)("Markup1").ToString().Trim()
                Dim markup2 As String = dt.Rows(i)("Markup2").ToString().Trim()
                Dim markup3 As String = dt.Rows(i)("Markup3").ToString().Trim()
                Dim markup4 As String = dt.Rows(i)("Markup4").ToString().Trim()
                Dim markup5 As String = dt.Rows(i)("Markup5").ToString().Trim()

                Dim rowNumber As Integer = i + 1

                If product = "" AndAlso markup1 = "" AndAlso markup2 = "" AndAlso markup3 = "" AndAlso markup4 = "" AndAlso markup5 = "" AndAlso method = "" Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT, METHOD AND MARKUP 1 ARE REQUIRED !", rowNumber))
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

                If markup1 = "" Then
                    MessageError(True, String.Format("ROW {0}: MARKUP 1 IS REQUIRED !", rowNumber))
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
                If dr("Markup1").ToString() = "" Then Continue For

                Dim markup1 As Decimal = Convert.ToDecimal(dr("Markup1"), CultureInfo.GetCultureInfo("en-US"))
                Dim markup2 As Decimal = 0
                Dim markup3 As Decimal = 0
                Dim markup4 As Decimal = 0
                Dim markup5 As Decimal = 0

                If Not String.IsNullOrWhiteSpace(dr("Markup2").ToString()) Then
                    markup2 = Convert.ToDecimal(dr("Markup2"), CultureInfo.GetCultureInfo("en-US"))
                End If

                If Not String.IsNullOrWhiteSpace(dr("Markup3").ToString()) Then
                    markup3 = Convert.ToDecimal(dr("Markup3"), CultureInfo.GetCultureInfo("en-US"))
                End If

                If Not String.IsNullOrWhiteSpace(dr("Markup4").ToString()) Then
                    markup4 = Convert.ToDecimal(dr("Markup4"), CultureInfo.GetCultureInfo("en-US"))
                End If

                If Not String.IsNullOrWhiteSpace(dr("Markup5").ToString()) Then
                    markup5 = Convert.ToDecimal(dr("Markup5"), CultureInfo.GetCultureInfo("en-US"))
                End If

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerMarkups ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("INSERT INTO CustomerMarkups VALUES (@Id, @CustomerId, @Type, @Method, @DataId, @Markup1, @Markup2, @Markup3, @Markup4, @Markup5, NULL)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Method", dr("Method").ToString())
                        thisCmd.Parameters.AddWithValue("@DataId", dr("Product").ToString())
                        thisCmd.Parameters.AddWithValue("@Markup1", markup1)
                        thisCmd.Parameters.AddWithValue("@Markup2", markup2)
                        thisCmd.Parameters.AddWithValue("@Markup3", markup3)
                        thisCmd.Parameters.AddWithValue("@Markup4", markup4)
                        thisCmd.Parameters.AddWithValue("@Markup5", markup5)

                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                settingClass.Logs({"CustomerMarkups", thisId, Session("LoginId").ToString(), "Customer Markup Created"})
            Next

            url = "~/setting/customer/markup"
            If lblReturnPage.Text = "detail" Then
                url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
            End If
            If action = "Again" Then
                url = String.Format("~/setting/customer/markup/add?custid={0}", ddlCustomer.SelectedValue)
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/customer/markup/add?custid={0}&returnpage=detail", ddlCustomer.SelectedValue)
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

    Protected Sub BindProduct(customerId As String, markupType As String, ddl As DropDownList)
        Try
            If Not String.IsNullOrEmpty(markupType) Then

                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@CustomerId", customerId),
                    New SqlParameter("@MarkupType", markupType)
                }
                Dim dt As DataTable = settingClass.GetDataTableSP("sp_BindMarkupData", params)

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
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function GetProductData(customerId As String, markupType As String) As DataTable
        Try
            Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@CustomerId", customerId),
                    New SqlParameter("@MarkupType", markupType)
                }
            Dim dt As DataTable = settingClass.GetDataTableSP("sp_BindMarkupData", params)
            Return dt
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            Return New DataTable()
        End Try
    End Function

    Protected Sub BindGrid()
        Try
            rptMarkup.DataSource = MarkupTable
            rptMarkup.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName").ToString() = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub SaveGrid()
        Try
            Dim dt As DataTable = MarkupTable

            While dt.Rows.Count < rptMarkup.Items.Count
                dt.Rows.Add("", "", "", "", "", "", "")
            End While

            For i As Integer = 0 To rptMarkup.Items.Count - 1
                Dim item As RepeaterItem = rptMarkup.Items(i)

                Dim ddlProduct As DropDownList = CType(item.FindControl("ddlProduct"), DropDownList)
                Dim ddlMethod As DropDownList = CType(item.FindControl("ddlMethod"), DropDownList)
                Dim txtMarkup1 As TextBox = CType(item.FindControl("txtMarkup1"), TextBox)
                Dim txtMarkup2 As TextBox = CType(item.FindControl("txtMarkup2"), TextBox)
                Dim txtMarkup3 As TextBox = CType(item.FindControl("txtMarkup3"), TextBox)
                Dim txtMarkup4 As TextBox = CType(item.FindControl("txtMarkup4"), TextBox)
                Dim txtMarkup5 As TextBox = CType(item.FindControl("txtMarkup5"), TextBox)

                If ddlProduct Is Nothing OrElse ddlMethod Is Nothing OrElse txtMarkup1 Is Nothing OrElse txtMarkup2 Is Nothing OrElse txtMarkup3 Is Nothing OrElse txtMarkup4 Is Nothing OrElse txtMarkup5 Is Nothing Then
                    Continue For
                End If

                dt.Rows(i)("Method") = ddlMethod.SelectedValue
                dt.Rows(i)("Product") = ddlProduct.SelectedValue

                dt.Rows(i)("Markup1") = txtMarkup1.Text.Trim()
                dt.Rows(i)("Markup2") = txtMarkup2.Text.Trim()
                dt.Rows(i)("Markup3") = txtMarkup3.Text.Trim()
                dt.Rows(i)("Markup4") = txtMarkup4.Text.Trim()
                dt.Rows(i)("Markup5") = txtMarkup5.Text.Trim()
            Next
            MarkupTable = dt
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
