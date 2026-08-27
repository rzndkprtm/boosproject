Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Markup_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Private Property PromoTable As DataTable
        Get
            If Session("PromoTable") Is Nothing Then
                Dim dt As New DataTable
                dt.Columns.Add("Product")
                dt.Columns.Add("Markup")
                dt.Columns.Add("Description")

                Session("PromoTable") = dt
            End If

            Return DirectCast(Session("PromoTable"), DataTable)
        End Get
        Set(value As DataTable)
            Session("PromoTable") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/markup/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("custid")) Then
            Response.Redirect("~/setting/customer/markup/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblCustomerId.Text = Request.QueryString("custid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer(lblCustomerId.Text)

            PromoTable.Rows.Clear()
            PromoTable.Rows.Add("", "", "")

            ddlType.SelectedValue = ""

            BindGrid()
        End If
    End Sub

    Protected Sub rptMarkup_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)

                Dim ddlProduct As DropDownList = CType(e.Item.FindControl("ddlProduct"), DropDownList)
                Dim txtMarkup As TextBox = CType(e.Item.FindControl("txtMarkup"), TextBox)
                If ddlProduct Is Nothing Then Exit Sub

                txtMarkup.Text = drv("Markup").ToString()

                If ddlType.SelectedValue <> "" Then
                    BindProduct(lblCustomerId.Text, ddlType.SelectedValue, ddlProduct)
                    If drv("Product") IsNot DBNull.Value Then
                        Dim item As ListItem = ddlProduct.Items.FindByValue(drv("Product").ToString())
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

    Protected Sub rptMarkup_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName <> "DeleteRow" Then Exit Sub
            SaveGrid()

            Dim index As Integer
            If Not Integer.TryParse(e.CommandArgument.ToString(), index) Then
                Exit Sub
            End If
            If index >= 0 AndAlso index < PromoTable.Rows.Count Then
                PromoTable.Rows.RemoveAt(index)
            End If
            If PromoTable.Rows.Count = 0 Then
                PromoTable.Rows.Add("", "", "")
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
            For Each item As RepeaterItem In rptMarkup.Items
                Dim ddlProduct As DropDownList = CType(item.FindControl("ddlProduct"), DropDownList)

                If ddlProduct IsNot Nothing Then
                    BindProduct(lblCustomerId.Text, ddlType.SelectedValue, ddlProduct)
                End If
            Next
            For Each row As DataRow In PromoTable.Rows
                row("Product") = ""
            Next
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
            PromoTable.Rows.Add("", "", "")
            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
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

            Dim dt As DataTable = PromoTable
            If dt.Rows.Count = 0 Then
                MessageError(True, "AT LEAST ONE MARKUP ROW IS REQUIRED !")
                Exit Sub
            End If

            For i As Integer = 0 To dt.Rows.Count - 1
                Dim product As String = dt.Rows(i)("Product").ToString().Trim()
                Dim markup As String = dt.Rows(i)("Markup").ToString().Trim()

                Dim rowNumber As Integer = i + 1

                If product = "" AndAlso markup = "" Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT AND MARKUP ARE REQUIRED !", rowNumber))
                    Exit Sub
                End If
                If product = "" Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT IS REQUIRED !", rowNumber))
                    Exit Sub
                End If
                If markup = "" Then
                    MessageError(True, String.Format("ROW {0}: MARKUP IS REQUIRED !", rowNumber))
                    Exit Sub
                End If
            Next

            For Each dr As DataRow In dt.Rows
                If dr("Product").ToString = "" Then Continue For
                If dr("Markup").ToString = "" Then Continue For

                Dim checkData As DataRow = settingClass.GetDataRow(String.Format("SELECT * FROM CustomerMarkups WHERE CustomerId='{0}' AND Type='{1}' AND DataId='{2}'", lblCustomerId.Text, ddlType.SelectedValue, dr("Product").ToString))
                If checkData IsNot Nothing Then
                    Dim thisId As String = checkData("Id").ToString()
                    Dim thisMarkup As Decimal = CDec(checkData("Markup"))
                    Dim newMarkup As Decimal = settingClass.GetTotalMarkup(thisMarkup, dr("Markup"))

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("UPDATE CustomerMarkups SET Markup=@Markup, Description=@Description WHERE Id=@Id", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisCmd.Parameters.AddWithValue("@Markup", newMarkup)
                            thisCmd.Parameters.AddWithValue("@Description", dr("Description").ToString())
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                    settingClass.Logs({"CustomerMarkups", thisId, Session("LoginId").ToString(), "Customer Markup Added"})
                Else
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerMarkups ORDER BY Id DESC")
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As New SqlCommand("INSERT INTO CustomerMarkups VALUES (@Id, @CustomerId, @Type, @DataId, @Markup, @Description)", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisCmd.Parameters.AddWithValue("@CustomerId", lblCustomerId.Text)
                            thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                            thisCmd.Parameters.AddWithValue("@DataId", dr("Product").ToString())
                            thisCmd.Parameters.AddWithValue("@Markup", dr("Markup"))
                            thisCmd.Parameters.AddWithValue("@Description", dr("Description").ToString())
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    settingClass.Logs({"CustomerMarkups", thisId, Session("LoginId").ToString(), "Customer Markup Created"})
                End If
            Next

            url = "~/setting/customer/markup"
            If lblReturnPage.Text = "detail" Then
                url = String.Format("~/setting/customer/detail?customerid={0}", lblCustomerId.Text)
            End If
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/customer/markup"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindCustomer(customerId As String)
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Customers WHERE Status='Active' AND Id='" & customerId & "' ORDER BY Name ASC")
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()
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

    Protected Sub BindGrid()
        Try
            rptMarkup.DataSource = PromoTable
            rptMarkup.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub SaveGrid()
        Try
            Dim dt As DataTable = PromoTable

            While dt.Rows.Count < rptMarkup.Items.Count
                dt.Rows.Add("", "", "")
            End While

            For i As Integer = 0 To rptMarkup.Items.Count - 1
                Dim item As RepeaterItem = rptMarkup.Items(i)

                Dim ddlProduct As DropDownList = CType(item.FindControl("ddlProduct"), DropDownList)
                Dim txtMarkup As TextBox = CType(item.FindControl("txtMarkup"), TextBox)
                Dim txtDescription As TextBox = CType(item.FindControl("txtDescription"), TextBox)

                If ddlProduct Is Nothing OrElse txtMarkup Is Nothing OrElse txtDescription Is Nothing Then
                    Continue For
                End If

                dt.Rows(i)("Markup") = txtMarkup.Text.Trim()
                dt.Rows(i)("Description") = txtDescription.Text.Trim()
                If ddlProduct.SelectedItem Is Nothing Then
                    dt.Rows(i)("Product") = ""
                Else
                    dt.Rows(i)("Product") =
                    ddlProduct.SelectedValue
                End If
            Next
            PromoTable = dt
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
