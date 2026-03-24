Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Base_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindProductGroup()
            BindPriceGroup()
        End If
    End Sub

    Protected Sub btnSort_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCategory.SelectedValue = "" Then
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "" Then
                Exit Sub
            End If
            If ddlProductGroup.SelectedValue = "" Then
                Exit Sub
            End If
            If ddlPriceGroup.SelectedValue = "" Then
                Exit Sub
            End If

            If String.IsNullOrEmpty(txtDiscount.Text) Then
                txtDiscount.Text = "0"
            End If

            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Category", If(String.IsNullOrEmpty(ddlCategory.SelectedValue), CType(DBNull.Value, Object), ddlCategory.SelectedValue)),
                New SqlParameter("@Method", If(String.IsNullOrEmpty(ddlMethod.SelectedValue), CType(DBNull.Value, Object), ddlMethod.SelectedValue)),
                New SqlParameter("@ProductGroupId", If(String.IsNullOrEmpty(ddlProductGroup.SelectedValue), CType(DBNull.Value, Object), ddlProductGroup.SelectedValue)),
                New SqlParameter("@PriceGroupId", If(String.IsNullOrEmpty(ddlPriceGroup.SelectedValue), CType(DBNull.Value, Object), ddlPriceGroup.SelectedValue)),
                New SqlParameter("@Discount", If(String.IsNullOrEmpty(txtDiscount.Text), CType(DBNull.Value, Object), txtDiscount.Text))
            }

            Dim thisData As DataTable = settingClass.GetDataTableSP("sp_GetPriceListPivot", params)

            gvList.DataSource = thisData
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindProductGroup()
        ddlProductGroup.Items.Clear()
        Try
            ddlProductGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceProductGroups ORDER BY Id ASC")
            ddlProductGroup.DataTextField = "Name"
            ddlProductGroup.DataValueField = "Id"
            ddlProductGroup.DataBind()

            If ddlProductGroup.Items.Count > 0 Then
                ddlProductGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlProductGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups ORDER BY Id ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
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
