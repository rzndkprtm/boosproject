Imports System.Data
Imports System.Data.SqlClient

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
                dt.Columns.Add("Discount")
                dt.Columns.Add("Description")

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

        If String.IsNullOrEmpty(Request.QueryString("custid")) Then
            Response.Redirect("~/setting/customer/discount", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblCustomerId.Text = Request.QueryString("custid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer(lblCustomerId.Text)

            DiscountTable.Rows.Clear()
            DiscountTable.Rows.Add("", "", "", "")

            ddlType.SelectedValue = ""

            BindGrid()
        End If
    End Sub

    Protected Sub rptDiscount_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)

                Dim ddlProduct As DropDownList = CType(e.Item.FindControl("ddlProduct"), DropDownList)
                Dim ddlMethod As DropDownList = CType(e.Item.FindControl("ddlMethod"), DropDownList)
                Dim txtDiscount As TextBox = CType(e.Item.FindControl("txtDiscount"), TextBox)
                Dim txtDescription As TextBox = CType(e.Item.FindControl("txtDescription"), TextBox)

                If ddlProduct Is Nothing Then Exit Sub

                If ddlMethod IsNot Nothing Then
                    ddlMethod.SelectedValue = drv("Method").ToString()
                End If
                If txtDiscount IsNot Nothing Then
                    txtDiscount.Text = drv("Discount").ToString()
                End If
                If txtDescription IsNot Nothing Then
                    txtDescription.Text = drv("Description").ToString()
                End If

                If ddlType.SelectedValue <> "" Then
                    BindProduct(lblCustomerId.Text, ddlType.SelectedValue, ddlProduct)

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
            If Session("RoleName").ToString() <> "Developer" Then
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
                DiscountTable.Rows.Add("", "", "", "")
            End If
            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim customerId As String = lblCustomerId.Text.Trim()
            Dim discType As String = ddlType.SelectedValue.Trim()

            DiscountTable.Rows.Clear()

            If String.IsNullOrEmpty(discType) Then
                DiscountTable.Rows.Add("", "", "", "")
                BindGrid()

                Exit Sub
            End If

            If discType = "Designs" OrElse discType = "PriceProductGroups" Then
                Dim checkData As DataRow = settingClass.GetDataRow("SELECT COUNT(*) AS Total FROM CustomerDiscounts WHERE CustomerId='" & customerId & "'")
                Dim totalDiscount As Integer = 0
                If checkData IsNot Nothing Then
                    Integer.TryParse(checkData("Total").ToString(), totalDiscount)
                End If

                If totalDiscount = 0 Then
                    Dim dtProduct As DataTable = GetProductData(customerId, discType)

                    If dtProduct IsNot Nothing AndAlso dtProduct.Rows.Count > 0 Then
                        For Each productRow As DataRow In dtProduct.Rows
                            Dim newRow As DataRow = DiscountTable.NewRow()
                            newRow("Product") = productRow("Id").ToString()
                            newRow("Method") = ""
                            newRow("Discount") = ""
                            newRow("Description") = ""

                            DiscountTable.Rows.Add(newRow)
                        Next
                    Else
                        DiscountTable.Rows.Add("", "", "", "")
                    End If

                    BindGrid()

                    Exit Sub
                End If
            End If

            DiscountTable.Rows.Clear()
            DiscountTable.Rows.Add("", "", "", "")

            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Try
            SaveGrid()
            DiscountTable.Rows.Add("", "", "", "")
            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
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
                Dim discount As String = dt.Rows(i)("Discount").ToString().Trim()

                Dim rowNumber As Integer = i + 1

                If product = "" AndAlso discount = "" AndAlso method = "" Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT, METHOD AND DISCOUNT ARE REQUIRED !", rowNumber))
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
                If discount = "" Then
                    MessageError(True, String.Format("ROW {0}: DISCOUNT IS REQUIRED !", rowNumber))
                    Exit Sub
                End If
            Next

            For Each dr As DataRow In dt.Rows
                If dr("Product").ToString = "" Then Continue For
                If dr("Method").ToString = "" Then Continue For
                If dr("Discount").ToString = "" Then Continue For

                Dim checkData As DataRow = settingClass.GetDataRow(String.Format("SELECT * FROM CustomerDiscounts WHERE CustomerId='{0}' AND Type='{1}' AND DataId='{2}'", lblCustomerId.Text, ddlType.SelectedValue, dr("Product").ToString))
                If checkData IsNot Nothing Then
                    Dim thisId As String = checkData("Id").ToString()
                    Dim thisDiscount As Decimal = CDec(checkData("Discount"))
                    Dim newDisc As Decimal = settingClass.GetTotalDiscount(thisDiscount, dr("Discount"))

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("UPDATE CustomerDiscounts SET Discount=@Discount, Description=@Description WHERE Id=@Id", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisCmd.Parameters.AddWithValue("@Discount", newDisc)
                            thisCmd.Parameters.AddWithValue("@Description", dr("Description").ToString())
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                    settingClass.Logs({"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Added"})
                Else
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerDiscounts ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("INSERT INTO CustomerDiscounts VALUES (@Id, @CustomerId, @Type, @Method, @DataId, @Discount, @Description)", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisCmd.Parameters.AddWithValue("@CustomerId", lblCustomerId.Text)
                            thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                            thisCmd.Parameters.AddWithValue("@Method", dr("Method").ToString())
                            thisCmd.Parameters.AddWithValue("@DataId", dr("Product").ToString())
                            thisCmd.Parameters.AddWithValue("@Discount", dr("Discount"))
                            thisCmd.Parameters.AddWithValue("@Description", dr("Description").ToString())
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    settingClass.Logs({"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Created"})
                End If
            Next

            url = "~/setting/customer/discount"
            If lblReturnPage.Text = "detail" Then
                url = String.Format("~/setting/customer/detail?customerid={0}", lblCustomerId.Text)
            End If
            If action = "Again" Then
                url = String.Format("~/setting/customer/discount/add?custid={0}", lblCustomerId.Text)
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/customer/discount/add?custid={0}&returnpage=detail", lblCustomerId.Text)
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

            ddlCustomer.DataSource = settingClass.GetDataTableSP("sp_Customers_List_Dropdown", params)
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

    Protected Sub BindProduct(customerId As String, discType As String, ddl As DropDownList)
        Try
            If Not String.IsNullOrEmpty(discType) Then
                Dim dt As DataTable

                Dim thisData As DataRow = settingClass.GetDataRow("SELECT CompanyId, CompanyDetailId, PriceGroupId FROM Customers WHERE Id='" & customerId & "'")
                If thisData IsNot Nothing Then
                    Dim companyId As String = thisData("CompanyId").ToString().Trim()
                    Dim companyDetailId As String = thisData("CompanyDetailId").ToString().Trim()
                    Dim priceGroupId As String = thisData("PriceGroupId").ToString().Trim()

                    Dim thisString As String = String.Empty
                    If discType = "Designs" Then
                        thisString = "SELECT Id, Name FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE companyArray.VALUE='" & companyId & "' AND applyArray.VALUE='Discounts' ORDER BY Name ASC"
                    End If
                    If discType = "PriceProductGroups" Then
                        thisString = "SELECT PriceProductGroups.Id, PriceProductGroups.Name FROM PriceProductGroups CROSS APPLY STRING_SPLIT(PriceGroupId, ',') AS thisArray WHERE thisArray.VALUE='" & priceGroupId & "'"
                    End If
                    If discType = "RollerFabrics" Then
                        thisString = "SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='12' AND (Status='In Stock' OR Status='Limited Stock')"
                    End If
                    If discType = "RomanFabrics" Then
                        thisString = "SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='8' AND (Status='In Stock' OR Status='Limited Stock')"
                    End If
                    If discType = "PanelGlideFabrics" Then
                        thisString = "SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='6' AND (Status='In Stock' OR Status='Limited Stock')"
                    End If
                    If discType = "RollerFabricColours" Then
                        thisString = "SELECT FabricColours.Id, FabricColours.Name FROM FabricColours LEFT JOIN Fabrics ON FabricColours.FabricId=Fabrics.Id CROSS APPLY STRING_SPLIT(Fabrics.CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='12' AND (Fabrics.Status='In Stock' OR Fabrics.Status='Limited Stock') AND (FabricColours.Status='In Stock' OR FabricColours.Status='Limited Stock')"
                    End If
                    If discType = "RollerChains" Then
                        thisString = "SELECT Chains.Id, Chains.Name FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray WHERE designArray.VALUE='12' AND companyDetailArray.VALUE='" & companyDetailId & "' AND ControlTypeId='1' AND (Status='In Stock' OR Status='Limited Stock')"
                    End If

                    dt = settingClass.GetDataTable(thisString)

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
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Function GetProductData(customerId As String, discType As String) As DataTable
        Try
            Dim dt As New DataTable

            If String.IsNullOrEmpty(discType) Then
                Return dt
            End If

            Dim thisData As DataRow = settingClass.GetDataRow("SELECT CompanyId, CompanyDetailId, PriceGroupId FROM Customers WHERE Id='" & customerId & "'")

            If thisData Is Nothing Then
                Return dt
            End If

            Dim companyId As String = thisData("CompanyId").ToString().Trim()
            Dim companyDetailId As String = thisData("CompanyDetailId").ToString().Trim()
            Dim priceGroupId As String = thisData("PriceGroupId").ToString().Trim()

            Dim thisString As String = String.Empty

            If discType = "Designs" Then
                thisString = "SELECT Id, Name FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE companyArray.VALUE='" & companyId & "' AND applyArray.VALUE='Discounts' ORDER BY Name ASC"
            ElseIf discType = "PriceProductGroups" Then
                thisString = "SELECT PriceProductGroups.Id, PriceProductGroups.Name FROM PriceProductGroups CROSS APPLY STRING_SPLIT(PriceGroupId, ',') AS thisArray WHERE thisArray.VALUE='" & priceGroupId & "' ORDER BY Name ASC"
            ElseIf discType = "RollerFabrics" Then
                thisString = "SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='12' AND (Status='In Stock' OR Status='Limited Stock') ORDER BY Name ASC"
            ElseIf discType = "RomanFabrics" Then
                thisString = "SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='8' AND (Status='In Stock' OR Status='Limited Stock') ORDER BY Name ASC"
            ElseIf discType = "PanelGlideFabrics" Then
                thisString = "SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='6' AND (Status='In Stock' OR Status='Limited Stock') ORDER BY Name ASC"
            ElseIf discType = "RollerFabricColours" Then
                thisString = "SELECT FabricColours.Id, FabricColours.Name FROM FabricColours LEFT JOIN Fabrics ON FabricColours.FabricId=Fabrics.Id CROSS APPLY STRING_SPLIT(Fabrics.CompanyDetailId, ',') AS companyDetailArray CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray WHERE companyDetailArray.VALUE='" & companyDetailId & "' AND designArray.VALUE='12' AND (Fabrics.Status='In Stock' OR Fabrics.Status='Limited Stock') AND (FabricColours.Status='In Stock' OR FabricColours.Status='Limited Stock') ORDER BY FabricColours.Name ASC"
            ElseIf discType = "RollerChains" Then
                thisString = "SELECT Chains.Id, Chains.Name FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyDetailArray WHERE designArray.VALUE='12' AND companyDetailArray.VALUE='" & companyDetailId & "' AND ControlTypeId='1' AND (Status='In Stock' OR Status='Limited Stock')"
            End If

            If Not String.IsNullOrEmpty(thisString) Then
                dt = settingClass.GetDataTable(thisString)
            End If
            Return dt
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
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
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub SaveGrid()
        Try
            Dim dt As DataTable = DiscountTable

            While dt.Rows.Count < rptDiscount.Items.Count
                dt.Rows.Add("", "", "", "")
            End While

            For i As Integer = 0 To rptDiscount.Items.Count - 1
                Dim item As RepeaterItem = rptDiscount.Items(i)

                Dim ddlProduct As DropDownList = CType(item.FindControl("ddlProduct"), DropDownList)
                Dim ddlMethod As DropDownList = CType(item.FindControl("ddlMethod"), DropDownList)
                Dim txtDiscount As TextBox = CType(item.FindControl("txtDiscount"), TextBox)
                Dim txtDescription As TextBox = CType(item.FindControl("txtDescription"), TextBox)

                If ddlProduct Is Nothing OrElse ddlMethod Is Nothing OrElse txtDiscount Is Nothing OrElse txtDescription Is Nothing Then
                    Continue For
                End If

                dt.Rows(i)("Discount") = txtDiscount.Text.Trim()
                dt.Rows(i)("Description") = txtDescription.Text.Trim()
                If ddlMethod.SelectedItem Is Nothing Then
                    dt.Rows(i)("Method") = ""
                Else
                    dt.Rows(i)("Method") =
                    ddlMethod.SelectedValue
                End If
                If ddlProduct.SelectedItem Is Nothing Then
                    dt.Rows(i)("Product") = ""
                Else
                    dt.Rows(i)("Product") =
                    ddlProduct.SelectedValue
                End If
            Next
            DiscountTable = dt
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
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
