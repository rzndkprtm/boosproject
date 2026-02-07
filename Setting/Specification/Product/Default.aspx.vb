Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDesignSort()
            ddlDesignSort.SelectedValue = Session("DesignProduct")
            BindBlindSort(ddlDesignSort.SelectedValue)
            ddlBlindSort.SelectedValue = Session("BlindProduct")
            BindCompanyDetailSort()
            ddlCompanyDetailSort.SelectedValue = Session("CompanyDetailProduct")
            ddlActive.SelectedValue = Session("ActiveProduct")
            txtSearch.Text = Session("SearchProduct")

            BindData(ddlDesignSort.SelectedValue, ddlBlindSort.SelectedValue, ddlCompanyDetailSort.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub ddlDesignSort_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlindSort(ddlDesignSort.SelectedValue)

        BindData(ddlDesignSort.SelectedValue, ddlBlindSort.SelectedValue, ddlCompanyDetailSort.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlBlindSort_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignSort.SelectedValue, ddlBlindSort.SelectedValue, ddlCompanyDetailSort.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlCompanyDetailSort_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindData(ddlDesignSort.SelectedValue, ddlBlindSort.SelectedValue, ddlCompanyDetailSort.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlActive_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignSort.SelectedValue, ddlBlindSort.SelectedValue, ddlCompanyDetailSort.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("DesignProduct") = ddlDesignSort.SelectedValue
        Session("BlindProduct") = ddlBlindSort.SelectedValue
        Session("CompanyDetailProduct") = ddlCompanyDetailSort.SelectedValue
        Session("ActiveProduct") = ddlActive.SelectedValue
        Session("SearchProduct") = txtSearch.Text

        Response.Redirect("~/setting/specification/product/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(ddlDesignSort.SelectedValue, ddlBlindSort.SelectedValue, ddlCompanyDetailSort.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(ddlDesignSort.SelectedValue, ddlBlindSort.SelectedValue, ddlCompanyDetailSort.SelectedValue, ddlActive.SelectedValue, txtSearch.Text)
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
                    Session("DesignProduct") = ddlDesignSort.SelectedValue
                    Session("BlindProduct") = ddlBlindSort.SelectedValue
                    Session("CompanyDetailProduct") = ddlCompanyDetailSort.SelectedValue
                    Session("ActiveProduct") = ddlActive.SelectedValue
                    Session("SearchProduct") = txtSearch.Text

                    url = String.Format("~/setting/specification/product/detail?productid={0}", dataId)
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

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Products WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using
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
            Dim newId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Products ORDER BY Id DESC")

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Products SELECT @NewId, DesignId, BlindId, CompanyDetailId, Name + ' - Copy', InvoiceName, TubeType, ControlType, ColourType, Description, Active FROM Products WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@NewId", newId)
                    myCmd.Parameters.AddWithValue("@Id", thisId)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Products", thisId, Session("LoginId").ToString(), "Product Created | Duplicated of " & thisId}
            settingClass.Logs(dataLog)

            Session("DesignProduct") = ddlDesignSort.SelectedValue
            Session("BlindProduct") = ddlBlindSort.SelectedValue
            Session("CompanyDetailProduct") = ddlCompanyDetailSort.SelectedValue
            Session("ActiveProduct") = ddlActive.SelectedValue
            Session("SearchProduct") = txtSearch.Text

            Response.Redirect("~/setting/specification/product", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnActive_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdActive.Text

            Dim oldActive As String = txtActive.Text
            Dim newActive As Boolean = False
            If oldActive = "0" Then newActive = True

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Products SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Active", newActive)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim descAction As String = "Product Activated"
            If newActive = False Then descAction = "Product Deactivated"

            dataLog = {"Products", thisId, Session("LoginId").ToString(), descAction}
            settingClass.Logs(dataLog)

            Session("DesignProduct") = ddlDesignSort.SelectedValue
            Session("BlindProduct") = ddlBlindSort.SelectedValue
            Session("CompanyDetailProduct") = ddlCompanyDetailSort.SelectedValue
            Session("ActiveProduct") = ddlActive.SelectedValue
            Session("SearchProduct") = txtSearch.Text

            Response.Redirect("~/setting/specification/product", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Private Sub BindData(designText As String, blindText As String, companyText As String, active As String, searchText As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Active", active),
                New SqlParameter("@DesignId", If(String.IsNullOrEmpty(designText), CType(DBNull.Value, Object), designText)),
                New SqlParameter("@BlindId", If(String.IsNullOrEmpty(blindText), CType(DBNull.Value, Object), blindText)),
                New SqlParameter("@CompanyDetailId", If(String.IsNullOrEmpty(companyText), CType(DBNull.Value, Object), companyText)),
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText))
            }

            Dim thisData As DataTable = settingClass.GetDataTableSP("sp_ProductList", params)

            gvList.DataSource = thisData
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")

            btnAdd.Visible = PageAction("Add")
            ddlDesignSort.Visible = PageAction("Design Sort")
            ddlBlindSort.Visible = PageAction("Blind Sort")
            ddlCompanyDetailSort.Visible = PageAction("Company Detail Sort")
            ddlActive.Visible = PageAction("Active")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesignSort()
        ddlDesignSort.Items.Clear()
        Try
            ddlDesignSort.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesignSort.DataTextField = "Name"
            ddlDesignSort.DataValueField = "Id"
            ddlDesignSort.DataBind()

            If ddlDesignSort.Items.Count > 1 Then
                ddlDesignSort.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindBlindSort(designId As String)
        ddlBlindSort.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                ddlBlindSort.DataSource = settingClass.GetDataTable("SELECT * FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlindSort.DataTextField = "Name"
                ddlBlindSort.DataValueField = "Id"
                ddlBlindSort.DataBind()

                If ddlBlindSort.Items.Count > 1 Then
                    ddlBlindSort.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub


    Protected Sub BindCompanyDetailSort()
        ddlCompanyDetailSort.Items.Clear()
        Try
            ddlCompanyDetailSort.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails ORDER BY Id ASC")
            ddlCompanyDetailSort.DataTextField = "Name"
            ddlCompanyDetailSort.DataValueField = "Id"
            ddlCompanyDetailSort.DataBind()

            If ddlCompanyDetailSort.Items.Count > 1 Then
                ddlCompanyDetailSort.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function BindCompanyDetail(productId As String) As String
        If Not String.IsNullOrEmpty(productId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Products.Id='" & productId & "' ORDER BY CompanyDetails.Id ASC")
            Dim hasil As String = String.Empty
            If Not myData.Rows.Count = 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("CompanyName").ToString()
                    hasil += designName & ", "
                Next
            End If
            Return hasil.Remove(hasil.Length - 2).ToString()
        End If
        Return "Error"
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function TextActive(active As Integer) As String
        If active = 1 Then : Return "Deactivate" : End If
        Return "Activate"
    End Function

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