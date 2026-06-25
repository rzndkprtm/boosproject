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
            BindDesign()
            ddlDesign.SelectedValue = Session("DesignSurcharge")
            BindBlind(ddlDesign.SelectedValue)
            ddlBlind.SelectedValue = Session("BlindSurcharge")
            BindPriceGroup()
            ddlPriceGroup.SelectedValue = Session("PriceGroupSurcharge")
            ddlActive.SelectedValue = Session("ActiveSurcharge")
            txtSearch.Text = Session("SearchSurcharge")
            BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("DesignSurcharge") = ddlDesign.SelectedValue
        Session("BlindSurcharge") = ddlBlind.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
        Session("ActiveSurcharge") = ddlActive.SelectedValue
        Session("SearchSurcharge") = txtSearch.Text

        Response.Redirect("~/setting/price/surcharge/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)

        Session("DesignSurcharge") = ddlDesign.SelectedValue
        Session("BlindSurcharge") = ddlBlind.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
        Session("ActiveSurcharge") = ddlActive.SelectedValue
        Session("SearchSurcharge") = txtSearch.Text

        MessageError(True, ddlDesign.SelectedValue)
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindBlind(ddlDesign.SelectedValue)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)

        MessageError(True, ddlDesign.SelectedValue)
    End Sub

    Protected Sub ddlBlind_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)

        Session("DesignSurcharge") = ddlDesign.SelectedValue
        Session("BlindSurcharge") = ddlBlind.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
        Session("ActiveSurcharge") = ddlActive.SelectedValue
        Session("SearchSurcharge") = txtSearch.Text
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)

        Session("DesignSurcharge") = ddlDesign.SelectedValue
        Session("BlindSurcharge") = ddlBlind.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
        Session("ActiveSurcharge") = ddlActive.SelectedValue
        Session("SearchSurcharge") = txtSearch.Text
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)

        Session("DesignSurcharge") = ddlDesign.SelectedValue
        Session("BlindSurcharge") = ddlBlind.SelectedValue
        Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
        Session("ActiveSurcharge") = ddlActive.SelectedValue
        Session("SearchSurcharge") = txtSearch.Text
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM PriceSurcharges WHERE Id=@Id; DELETE FROM Logs WHERE Type='PriceSurcharges' AND DataId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("DesignSurcharge") = ddlDesign.SelectedValue
            Session("BlindSurcharge") = ddlBlind.SelectedValue
            Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
            Session("ActiveSurcharge") = ddlActive.SelectedValue
            Session("SearchSurcharge") = txtSearch.Text

            Response.Redirect("~/setting/price/surcharge", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCopy_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdCopy.Text
            Dim newId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceSurcharges ORDER BY Id DESC")

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO PriceSurcharges SELECT @NewId, DesignId, BlindId, BlindNumber, PriceGroupId, 'Copy - ' + Name, FieldName, Formula, BuyCharge, SellCharge, Description, Active FROM PriceSurcharges WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@NewId", newId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"PriceSurcharges", newId, Session("LoginId").ToString(), "Surcharge Createad | Duplicate from ID : " & thisId}
            settingClass.Logs(dataLog)

            Session("DesignSurcharge") = ddlDesign.SelectedValue
            Session("BlindSurcharge") = ddlBlind.SelectedValue
            Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
            Session("ActiveSurcharge") = ddlActive.SelectedValue
            Session("SearchSurcharge") = txtSearch.Text

            Response.Redirect("~/setting/price/surcharge", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(designText As String, blindText As String, priceGroupText As String, activeText As String, searchText As String)
        Session("DesignSurcharge") = String.Empty
        Session("BlindSurcharge") = String.Empty
        Session("PriceGroupSurcharge") = String.Empty
        Session("ActiveSurcharge") = String.Empty
        Session("SearchSurcharge") = String.Empty
        Try
            Dim designString As String = ""
            Dim blindString As String = ""
            Dim priceGroupString As String = String.Empty
            Dim activeString As String = "WHERE PriceSurcharges.Active=1"
            Dim searchString As String = ""

            If activeText = 0 Then
                activeString = "WHERE PriceSurcharges.Active=0"
            End If

            If Not designText = "" Then
                designString = "AND PriceSurcharges.DesignId='" & designText & "'"
            End If

            If Not blindText = "" Then
                blindString = "AND PriceSurcharges.BlindId='" & blindText & "'"
            End If

            If Not priceGroupText = "" Then
                priceGroupString = "AND PriceGroupId='" & priceGroupText & "'"
            End If

            If Not searchText = "" Then
                searchString = " AND (PriceSurcharges.Name LIKE '%" & searchText.Trim() & "%' OR PriceSurcharges.FieldName LIKE '%" & searchText.Trim() & "%' OR PriceSurcharges.Formula LIKE '%" & searchText.Trim() & "%' OR PriceSurcharges.Description LIKE '%" & searchText.Trim() & "%' OR PriceGroups.Name LIKE '%" & searchText.Trim() & "%')"
            End If

            Dim thisQuery As String = String.Format("SELECT PriceSurcharges.*, PriceGroups.Name AS PriceGroupName, Designs.Name + ' | ' + Blinds.Name AS Product FROM PriceSurcharges INNER JOIN Designs ON PriceSurcharges.DesignId=Designs.Id INNER JOIN Blinds ON PriceSurcharges.BlindId=Blinds.Id LEFT JOIN PriceGroups ON PriceSurcharges.PriceGroupId=PriceGroups.Id {0} {1} {2} {3} {4} ORDER BY PriceSurcharges.FieldName, PriceSurcharges.Name, PriceSurcharges.PriceGroupId, PriceSurcharges.BlindNumber ASC", activeString, designString, blindString, priceGroupString, searchString)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

            btnAdd.Visible = LoginAccess("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            ddlDesign.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindBlind(designId As String)
        ddlBlind.Items.Clear()
        Try
            If Not designId = "" Then
                ddlBlind.DataSource = settingClass.GetDataTable("SELECT * FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlind.DataTextField = "Name"
                ddlBlind.DataValueField = "Id"
                ddlBlind.DataBind()

                If ddlBlind.Items.Count > 1 Then
                    ddlBlind.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup(Optional isEdit As Boolean = False)
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups ORDER BY Name ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 1 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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
