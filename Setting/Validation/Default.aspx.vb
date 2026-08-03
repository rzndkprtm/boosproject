Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Setting_Validation_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    <WebMethod()>
    Public Shared Function GetValidationDetail(validationId As String) As List(Of ValidationDetailModel)
        Dim result As New List(Of ValidationDetailModel)

        Dim settingClass As New SettingClass()

        Dim dt As DataTable = settingClass.GetDataTable("SELECT * FROM ValidationDetails WHERE ValidationId='" & validationId & "' ORDER BY GroupNo")
        For Each dr As DataRow In dt.Rows
            Dim groupNo As String = dr("GroupNo").ToString()
            Dim fieldName As String = dr("FieldName").ToString()
            Dim operators As String = dr("Operator").ToString()
            Dim compareValue As String = dr("CompareValue").ToString()
            Dim dataType As String = dr("DataType").ToString()

            result.Add(New ValidationDetailModel With {.Id = dr("Id").ToString(), .ValidationId = validationId, .GroupNo = groupNo, .FieldName = fieldName, .Operators = operators, .CompareValue = compareValue, .DataType = dataType})
        Next
        Return result
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindDesignType()
            ddlDesign.SelectedValue = Session("SearchValidation")
            BindData(ddlDesign.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchValidation") = ddlDesign.SelectedValue
        Response.Redirect("~/setting/validation/add", False)
    End Sub

    Protected Sub btnDetail_Click(sender As Object, e As EventArgs)
        Dim thisId As String = txtValidationId.Text
        Dim url As String = String.Format("~/setting/validation/detail/add?validationid={0}", thisId)
        Response.Redirect(url, False)
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue)

        Session("SearchValidation") = ddlDesign.SelectedValue
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(ddlDesign.SelectedValue)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(ddlDesign.SelectedValue)
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub BindData(designId As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", If(String.IsNullOrEmpty(designId), CType(DBNull.Value, Object), designId))
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_Validations_List", params)
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

    Protected Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
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

Public Class ValidationDetailModel
    Public Property Id As String
    Public Property ValidationId As String
    Public Property GroupNo As String
    Public Property FieldName As String
    Public Property Operators As String
    Public Property CompareValue As String
    Public Property DataType As String
End Class