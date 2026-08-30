Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_ChangeValue(False, String.Empty)

            BindDesignType()
            ddlDesignType.SelectedValue = Session("DesignSurcharge")

            BindPriceGroup(ddlDesignType.SelectedValue)
            ddlPriceGroup.SelectedValue = Session("PriceGroupSurcharge")

            txtSearch.Text = Session("SearchSurcharge")
            BindData(txtSearch.Text, ddlDesignType.SelectedValue, ddlPriceGroup.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchSurcharge") = txtSearch.Text
        Session("DesignSurcharge") = ddlDesignType.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue

        Response.Redirect("~/setting/price/surcharge/add", False)
    End Sub

    Protected Sub btnCopyAll_Click(sender As Object, e As EventArgs)
        Session("SearchSurcharge") = txtSearch.Text
        Session("DesignSurcharge") = ddlDesignType.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue

        Response.Redirect("~/setting/price/surcharge/copy", False)
    End Sub

    Protected Sub btnDeleteAll_Click(sender As Object, e As EventArgs)
        Session("SearchSurcharge") = txtSearch.Text
        Session("DesignSurcharge") = ddlDesignType.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue

        Response.Redirect("~/setting/price/surcharge/delete", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlDesignType.SelectedValue, ddlPriceGroup.SelectedValue)

        Session("SearchSurcharge") = txtSearch.Text
        Session("DesignSurcharge") = ddlDesignType.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
    End Sub

    Protected Sub ddlDesignType_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindPriceGroup(ddlDesignType.SelectedValue)

        BindData(txtSearch.Text, ddlDesignType.SelectedValue, ddlPriceGroup.SelectedValue)

        Session("SearchSurcharge") = txtSearch.Text
        Session("DesignSurcharge") = ddlDesignType.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlDesignType.SelectedValue, ddlPriceGroup.SelectedValue)

        Session("SearchSurcharge") = txtSearch.Text
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlDesignType.SelectedValue, ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text, ddlDesignType.SelectedValue, ddlPriceGroup.SelectedValue)
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnRePrice_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataOrder As DataTable = settingClass.GetDataTable("SELECT Id FROM OrderHeaders WHERE Active=1 AND (Status = 'Unsubmitted' OR Status='Waiting Proforma')")
            If Not dataOrder.Rows.Count = 0 Then
                Dim orderClass As New OrderClass
                For i As Integer = 0 To dataOrder.Rows.Count - 1
                    Dim orderId As String = dataOrder.Rows(i)("Id").ToString()
                    orderClass.CalculatePriceByOrder(orderId)

                    Dim dataLog As Object() = {"OrderHeaders", orderId, Session("LoginId").ToString(), "Re Price Order"}
                    settingClass.Logs(dataLog)
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnChangeValue_Click(sender As Object, e As EventArgs)
        MessageError_ChangeValue(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showChangeValue(); };"
        Try
            Dim thisId As String = txtChangeValueId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE PriceSurcharges SET BuyCharge=@BuyCharge, SellCharge=@SellCharge, FactoryCharge=@FactoryCharge WHERE Id=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@SellCharge", txtSell.Text)
                    thisCmd.Parameters.AddWithValue("@BuyCharge", txtBuy.Text)
                    thisCmd.Parameters.AddWithValue("@FactoryCharge", txtFactory.Text)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchSurcharge") = txtSearch.Text
            Session("DesignSurcharge") = ddlDesignType.SelectedValue
            Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue

            Response.Redirect("~/setting/price/surcharge", False)
        Catch ex As Exception
            MessageError_ChangeValue(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ChangeValue(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showChangeValue", thisScript, True)
        End Try
    End Sub

    Protected Sub btnCopy_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtCopyId.Text
            Dim newId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceSurcharges ORDER BY Id DESC")

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO PriceSurcharges SELECT @NewId, DesignId, PriceGroupId, Name + ' - Copy', Type, Formula, SellCharge, BuyCharge, FactoryCharge, Description, Status FROM PriceSurcharges WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@NewId", newId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"PriceSurcharges", newId, Session("LoginId").ToString(), "Surcharge Createad | Duplicate from ID : " & thisId}
            settingClass.Logs(dataLog)

            Session("SearchSurcharge") = txtSearch.Text
            Session("DesignSurcharge") = ddlDesignType.SelectedValue
            Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue

            url = String.Format("~/setting/price/surcharge/edit?surchargeid={0}", newId)

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
            Dim thisId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE PriceSurcharges SET Status='Deleted', Name=CASE WHEN Name LIKE '%(DELETED)%' THEN Name ELSE Name + ' (DELETED)' END WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.Add("@Id", SqlDbType.Int).Value = CInt(thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"PriceSurcharges", thisId, Session("LoginId").ToString(), "Price Surcharges Deleted"}
            settingClass.Logs(dataLog)

            Session("SearchSurcharge") = txtSearch.Text
            Session("DesignSurcharge") = ddlDesignType.SelectedValue
            Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue

            Response.Redirect("~/setting/price/surcharge", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String, designId As String, priceGroupId As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText)),
                New SqlParameter("@DesignId", If(String.IsNullOrEmpty(designId), CType(DBNull.Value, Object), designId)),
                New SqlParameter("@PriceGroupId", If(String.IsNullOrEmpty(priceGroupId), CType(DBNull.Value, Object), priceGroupId)),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(Session("CompanyId").ToString()), CType(DBNull.Value, Object), Session("CompanyId").ToString()))
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_PriceSurcharges_List", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

            btnAdd.Visible = LoginAccess("Add")
            aRePrice.Visible = LoginAccess("Re Price")
            btnMore.Visible = LoginAccess("More")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesignType()
        ddlDesignType.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT Id, Name FROM Designs ORDER BY Name ASC"
            If Session("RoleName") = "Sales" OrElse Session("LevelName") = "Account" Then
                thisQuery = "SELECT Id, Name FROM Designs D WHERE EXISTS (SELECT 1 FROM STRING_SPLIT(D.CompanyId, ',') S WHERE TRY_CAST(S.value AS INT) = '" & Session("CompanyId").ToString() & "' ) ORDER BY D.Name ASC;"
            End If
            ddlDesignType.DataSource = settingClass.GetDataTable(thisQuery)
            ddlDesignType.DataTextField = "Name"
            ddlDesignType.DataValueField = "Id"
            ddlDesignType.DataBind()

            If ddlDesignType.Items.Count > 1 Then
                ddlDesignType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindPriceGroup(designid As String)
        ddlPriceGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designid) Then
                Dim type As String = settingClass.GetItemData("SELECT Type FROM Designs WHERE Id='" & designid & "' AND Active=1")
                If Not String.IsNullOrEmpty(type) Then
                    Dim thisQuery As String = "SELECT Id, Name FROM PriceGroups WHERE Type='" & type & "' AND Status='Active' ORDER BY Name ASC"

                    If Session("RoleName") = "Sales" OrElse Session("LevelName") = "Account" Then
                        thisQuery = "SELECT Id, Name FROM PriceGroups WHERE Type='" & type & "' AND CompanyId='" & Session("CompanyId").ToString() & "' AND Status='Active' ORDER BY Name ASC"
                    End If

                    ddlPriceGroup.DataSource = settingClass.GetDataTable(thisQuery)
                    ddlPriceGroup.DataTextField = "Name"
                    ddlPriceGroup.DataValueField = "Id"
                    ddlPriceGroup.DataBind()

                    If ddlPriceGroup.Items.Count > 1 Then
                        ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Return
            End If

            navPager.Visible = True

            Dim currentPage As Integer = gvList.PageIndex
            Dim totalPages As Integer = gvList.PageCount

            Dim pages As New List(Of Object)

            If currentPage > 0 Then
                pages.Add(New With {.Text = "Previous", .PageIndex = currentPage - 1, .CssClass = ""})
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {.Text = (i + 1).ToString(), .PageIndex = i, .CssClass = If(i = currentPage, "active", "")})
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {.Text = "Next", .PageIndex = currentPage + 1, .CssClass = ""})
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_ChangeValue(visible As Boolean, message As String)
        divErrorChangeValue.Visible = visible : msgErrorChangeValue.InnerText = message
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
