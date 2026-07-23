Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Promo_Detail_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Private Property PromoTable As DataTable
        Get
            If Session("PromoTable") Is Nothing Then

                Dim dt As New DataTable
                dt.Columns.Add("Type")
                dt.Columns.Add("Data")
                dt.Columns.Add("Discount")

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
            Response.Redirect("~/setting/price/promo/detail", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("promoid")) Then
            Response.Redirect("~/setting/price/promo/", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("promoid").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindPromo()
            ddlPromo.SelectedValue = lblId.Text

            PromoTable.Rows.Clear()
            PromoTable.Rows.Add("", "", "")

            BindGrid()
        End If
    End Sub

    Protected Sub rptPromo_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)

                Dim ddlType As DropDownList = CType(e.Item.FindControl("ddlType"), DropDownList)
                Dim ddlData As DropDownList = CType(e.Item.FindControl("ddlData"), DropDownList)

                If ddlType Is Nothing OrElse ddlData Is Nothing Then Exit Sub

                If drv("Type") IsNot DBNull.Value Then
                    ddlType.SelectedValue = drv("Type").ToString()
                End If

                If ddlType.SelectedValue <> "" Then
                    BindData(ddlType.SelectedValue, ddlData)

                    If drv("Data") IsNot DBNull.Value Then
                        Dim item As ListItem = ddlData.Items.FindByValue(drv("Data").ToString())

                        If item IsNot Nothing Then
                            ddlData.SelectedValue = item.Value
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
            If Not Integer.TryParse(e.CommandArgument.ToString(), index) Then Exit Sub

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
        Try
            Dim ddlType As DropDownList = CType(sender, DropDownList)
            Dim item As RepeaterItem = CType(ddlType.NamingContainer, RepeaterItem)
            Dim ddlData As DropDownList = CType(item.FindControl("ddlData"), DropDownList)

            BindData(ddlType.SelectedValue, ddlData)

            PromoTable.Rows(item.ItemIndex)("Type") = ddlType.SelectedValue
            PromoTable.Rows(item.ItemIndex)("Data") = ""
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName").ToString() <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    'Protected Sub ddlData_SelectedIndexChanged(sender As Object, e As EventArgs)
    '    Try
    '        Dim ddl As DropDownList = CType(sender, DropDownList)
    '        Dim item As RepeaterItem = CType(ddl.NamingContainer, RepeaterItem)
    '        PromoTable.Rows(item.ItemIndex)("Data") = ddl.SelectedValue
    '    Catch ex As Exception
    '        MessageError(True, ex.ToString())
    '        If Session("RoleName").ToString() <> "Developer" Then
    '            MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
    '        End If
    '    End Try
    'End Sub

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

            Dim dt As DataTable = PromoTable
            If dt.Rows.Count = 0 Then Exit Sub

            For Each dr As DataRow In dt.Rows

                If dr("Type").ToString = "" Then Continue For
                If dr("Data").ToString = "" Then Continue For

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PromoDetails ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)

                    Using thisCmd As New SqlCommand("INSERT INTO PromoDetails VALUES (@Id, @PromoId, @Type, @DataId, @Discount)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@PromoId", ddlPromo.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Type", dr("Type"))
                        thisCmd.Parameters.AddWithValue("@DataId", dr("Data"))
                        thisCmd.Parameters.AddWithValue("@Discount", dr("Discount"))

                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PromoDetails", thisId, Session("LoginId").ToString(), "Promo Detail Created"}
                settingClass.Logs(dataLog)
            Next

            Session.Remove("PromoTable")
            Response.Redirect(String.Format("~/setting/price/promo/detail?promoid={0}", ddlPromo.SelectedValue), False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Session("RoleName") <> "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Session.Remove("PromoTable")
        Dim url As String = String.Format("~/setting/price/promo/detail/?promoid={0}", ddlPromo.SelectedValue)
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindPromo()
        ddlPromo.Items.Clear()
        Try
            ddlPromo.DataSource = settingClass.GetDataTable("SELECT * FROM Promos ORDER BY Name ASC")
            ddlPromo.DataTextField = "Name"
            ddlPromo.DataValueField = "Id"
            ddlPromo.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(type As String, ddl As DropDownList)
        Try
            Dim dt As DataTable
            Select Case type
                Case "Designs"
                    dt = settingClass.GetDataTable("SELECT * FROM Designs")
                Case "Blinds"
                    dt = settingClass.GetDataTable("SELECT Blinds.Id, '[' + Designs.Name + '] ' + Blinds.Name AS Name FROM Blinds INNER JOIN Designs ON Blinds.DesignId=Designs.Id ORDER BY Designs.Name, Blinds.Name ASC")
                Case "Products"
                    dt = settingClass.GetDataTable("SELECT * FROM Products")
                Case "Fabrics"
                    dt = settingClass.GetDataTable("SELECT * FROM Fabrics")
                Case "FabricColours"
                    dt = settingClass.GetDataTable("SELECT * FROM FabricColours")
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
                dt.Rows.Add("", "", "")
            End While

            For i As Integer = 0 To rptPromo.Items.Count - 1
                Dim item As RepeaterItem = rptPromo.Items(i)

                Dim ddlType As DropDownList = CType(item.FindControl("ddlType"), DropDownList)
                Dim ddlData As DropDownList = CType(item.FindControl("ddlData"), DropDownList)
                Dim txtDiscount As TextBox = CType(item.FindControl("txtDiscount"), TextBox)

                If ddlType Is Nothing OrElse ddlData Is Nothing OrElse txtDiscount Is Nothing Then
                    Continue For
                End If

                dt.Rows(i)("Type") = ddlType.SelectedValue

                If ddlData.SelectedItem Is Nothing Then
                    dt.Rows(i)("Data") = ""
                Else
                    dt.Rows(i)("Data") = ddlData.SelectedValue
                End If

                dt.Rows(i)("Discount") = txtDiscount.Text.Trim()
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
