Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Promo_Detail_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Private Property PromoTable As DataTable
        Get
            If Session("PromoTable") Is Nothing Then
                Dim dt As New DataTable
                dt.Columns.Add("Product")
                dt.Columns.Add("Method")
                dt.Columns.Add("Discount")
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
            Response.Redirect("~/setting/price/promo", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("promoid")) Then
            Response.Redirect("~/setting/price/promo", False)
            Exit Sub
        End If

        lblPromoId.Text = Request.QueryString("promoid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindPromo(lblPromoId.Text)

            PromoTable.Rows.Clear()
            PromoTable.Rows.Add("", "", "", "")

            ddlType.SelectedValue = ""

            BindGrid()
        End If
    End Sub

    Protected Sub rptPromo_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
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
                    BindProduct(ddlType.SelectedValue, ddlProduct)

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

    Protected Sub rptPromo_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
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
                PromoTable.Rows.Add("", "", "", "")
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
            Dim discType As String = ddlType.SelectedValue.Trim()

            PromoTable.Rows.Clear()

            If String.IsNullOrEmpty(discType) Then
                PromoTable.Rows.Add("", "", "", "")
                BindGrid()

                Exit Sub
            End If

            PromoTable.Rows.Clear()
            PromoTable.Rows.Add("", "", "", "")

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
            PromoTable.Rows.Add("", "", "", "")
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
        url = String.Format("~/setting/price/promo/detail?promoid={0}", lblPromoId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub Process(Optional action As String = "")
        MessageError(False, String.Empty)
        Try
            SaveGrid()

            If ddlPromo.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "PROMO TYPE IS REQUIRED !")
                Exit Sub
            End If

            Dim dt As DataTable = PromoTable
            If dt.Rows.Count = 0 Then
                MessageError(True, "AT LEAST ONE PROMO ROW IS REQUIRED !")
                Exit Sub
            End If

            For i As Integer = 0 To dt.Rows.Count - 1
                Dim product As String = dt.Rows(i)("Product").ToString().Trim()
                Dim method As String = dt.Rows(i)("Method").ToString().Trim()
                Dim discount As String = dt.Rows(i)("Discount").ToString().Trim()

                Dim rowNumber As Integer = i + 1

                If product = "" AndAlso discount = "" AndAlso method = "" Then
                    MessageError(True, String.Format("ROW {0}: PRODUCT, METHOD AND PROMO ARE REQUIRED !", rowNumber))
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

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PromoDetails ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("INSERT INTO PromoDetails VALUES (@Id, @PromoId, @Type, @Method, @DataId, @Discount, @Description, 'Active')", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@PromoId", lblPromoId.Text)
                        thisCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Method", dr("Method"))
                        thisCmd.Parameters.AddWithValue("@DataId", dr("Product"))
                        thisCmd.Parameters.AddWithValue("@Discount", dr("Discount"))
                        thisCmd.Parameters.AddWithValue("@Description", dr("Description"))
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PromoDetails", thisId, Session("LoginId").ToString(), "Promo Detail Created"}
                settingClass.Logs(dataLog)
            Next

            url = String.Format("~/setting/price/promo/detail?promoid={0}", lblPromoId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPromo(promoId As String)
        ddlPromo.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(promoId) Then
                ddlPromo.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Promos WHERE Id='" & promoId & "' ORDER BY Name ASC")
                ddlPromo.DataTextField = "Name"
                ddlPromo.DataValueField = "Id"
                ddlPromo.DataBind()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindProduct(discType As String, ddl As DropDownList)
        Try
            If Not String.IsNullOrEmpty(discType) Then
                Dim dt As DataTable

                Select Case discType
                    Case "Designs"
                        dt = settingClass.GetDataTable("SELECT Id, Name FROM Designs")
                    Case "Blinds"
                        dt = settingClass.GetDataTable("SELECT Blinds.Id, '[' + Designs.Name + '] ' + Blinds.Name AS Name FROM Blinds INNER JOIN Designs ON Blinds.DesignId=Designs.Id ORDER BY Designs.Name, Blinds.Name ASC")
                    Case "Products"
                        dt = settingClass.GetDataTable("SELECT Id, Name FROM Products")
                    Case "RollerFabrics"
                        dt = settingClass.GetDataTable("SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE designArray.VALUE='12' AND (Status='In Stock' OR Status='Limited Stock')")
                    Case "CurtainFabrics"
                        dt = settingClass.GetDataTable("SELECT Id, Name FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE designArray.VALUE='3' AND (Status='In Stock' OR Status='Limited Stock')")
                    Case "RollerFabricColours"
                        dt = settingClass.GetDataTable("SELECT FabricColours.Id AS Id, FabricColours.Name AS Name FROM FabricColours LEFT JOIN Fabrics CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray ON FabricColours.FabricId=Fabrics.Id WHERE designArray.VALUE='12'")
                    Case "CurtainFabricColours"
                        dt = settingClass.GetDataTable("SELECT FabricColours.Id AS Id, FabricColours.Name AS Name FROM FabricColours LEFT JOIN Fabrics CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray ON FabricColours.FabricId=Fabrics.Id WHERE designArray.VALUE='3'")
                    Case "FrameColours"
                        dt = New DataTable()

                        dt.Columns.Add("Id")
                        dt.Columns.Add("Name")

                        dt.Rows.Add("Primrose (Express)", "Primrose (Express)")
                        dt.Rows.Add("Primrose (Regular)", "Primrose (Regular)")
                    Case Else
                        dt = New DataTable()
                End Select

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
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindGrid()
        Try
            rptPromo.DataSource = PromoTable
            rptPromo.DataBind()
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

            While dt.Rows.Count < rptPromo.Items.Count
                dt.Rows.Add("", "", "", "")
            End While

            For i As Integer = 0 To rptPromo.Items.Count - 1
                Dim item As RepeaterItem = rptPromo.Items(i)

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
