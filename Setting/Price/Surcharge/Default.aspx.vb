Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
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
        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Session("DesignSurcharge") = ddlDesign.SelectedValue
                    Session("BlindSurcharge") = ddlBlind.SelectedValue
                    Session("PriceGroupSurcharge") = ddlPriceGroup.SelectedValue
                    Session("ActiveSurcharge") = ddlActive.SelectedValue
                    Session("SearchSurcharge") = txtSearch.Text

                    url = String.Format("~/setting/price/surcharge/detail?surchargeid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlind(ddlDesign.SelectedValue)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlBlind_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue, ddlBlind.SelectedValue, ddlPriceGroup.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM PriceSurcharges WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='PriceSurcharges' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
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
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO PriceSurcharges SELECT @NewId, DesignId, BlindId, BlindNumber, PriceGroupId, 'Copy - ' + Name, FieldName, Formula, BuyCharge, SellCharge, Description, Active FROM PriceSurcharges WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@NewId", newId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
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
            gvList.Columns(1).Visible = PageAction("Visible ID")

            btnAdd.Visible = PageAction("Add")
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

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
