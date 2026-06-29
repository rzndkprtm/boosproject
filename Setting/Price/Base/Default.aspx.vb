Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Price_Base_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim idIDR As New CultureInfo("id-ID")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindProductGroup()
            BindPriceGroup()
            BindData(ddlCategory.SelectedValue, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base/add", False)
    End Sub

    Protected Sub btnImport_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base/import", False)
    End Sub

    Protected Sub ddlCategory_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0
        MessageError(False, String.Empty)
        BindData(ddlCategory.SelectedValue, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub ddlProductGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0
        MessageError(False, String.Empty)
        BindData(ddlCategory.SelectedValue, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub ddlMethod_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0
        MessageError(False, String.Empty)
        BindData(ddlCategory.SelectedValue, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0
        MessageError(False, String.Empty)
        BindData(ddlCategory.SelectedValue, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
        BindData(ddlCategory.SelectedValue, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        gvList.PageIndex = e.NewPageIndex
        BindData(ddlCategory.SelectedValue, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM PriceBases WHERE Id=@Id; DELETE FROM Logs WHERE Type='PriceBases' AND DataId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Response.Redirect("~/setting/price/base", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(category As String, method As String, productgroup As String, pricegroup As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@Category", If(String.IsNullOrEmpty(category), CType(DBNull.Value, Object), category)),
                New SqlParameter("@Method", If(String.IsNullOrEmpty(method), CType(DBNull.Value, Object), method)),
                New SqlParameter("@ProductGroup", If(String.IsNullOrEmpty(productgroup), CType(DBNull.Value, Object), productgroup)),
                New SqlParameter("@PriceGroup", If(String.IsNullOrEmpty(pricegroup), CType(DBNull.Value, Object), pricegroup))
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_PriceBaseList", params)
            gvList.DataBind()

            btnAdd.Visible = LoginAccess("Add")
            btnImport.Visible = LoginAccess("Import")

            aMatrix.Visible = False

            If pricegroup = "1" AndAlso Not String.IsNullOrWhiteSpace(category) AndAlso Not String.IsNullOrWhiteSpace(productgroup) AndAlso Not String.IsNullOrWhiteSpace(method) Then
                aMatrix.Visible = True

                Dim paramMatrixs As New List(Of SqlParameter) From {
                    New SqlParameter("@Category", ddlCategory.SelectedValue),
                    New SqlParameter("@Method", ddlMethod.SelectedValue),
                    New SqlParameter("@ProductGroupId", ddlProductGroup.SelectedValue),
                    New SqlParameter("@PriceGroupId", ddlPriceGroup.SelectedValue)
                }
                gvListMatrix.DataSource = settingClass.GetDataTableSP("sp_PriceBaseMatrix", paramMatrixs)
                gvListMatrix.DataBind()
            End If
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
            Dim thisString As String = "SELECT * FROM PriceProductGroups ORDER BY Id ASC"
            If Session("RoleName") = "Account" OrElse Session("RoleName") = "Sales" Then
                thisString = "SELECT * FROM PriceProductGroups CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE companyArray.VALUE='" & Session("CompanyDetailId").ToString() & "' ORDER BY Id ASC"
            End If

            ddlProductGroup.DataSource = settingClass.GetDataTable(thisString)
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
            Dim thisString As String = "SELECT * FROM PriceGroups ORDER BY Id ASC"
            If Session("RoleName") = "Account" OrElse Session("RoleName") = "Sales" Then
                thisString = "SELECT * FROM PriceGroups WHERE CompanyId='" & Session("CompanyId").ToString() & "' ORDER BY Id ASC"
            End If
            ddlPriceGroup.DataSource = settingClass.GetDataTable(thisString)
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

    Protected Function BindCost(cost As Decimal, priceGroupId As String) As String
        Try
            If cost >= 0 Then
                If priceGroupId = "2" OrElse priceGroupId = "3" OrElse priceGroupId = "4" OrElse priceGroupId = "5" OrElse priceGroupId = "10" OrElse priceGroupId = "17" OrElse priceGroupId = "19" Then
                    Return cost.ToString("N2", idIDR)
                End If
                Return cost.ToString("N2", enUS)
            End If
        Catch ex As Exception
            Return String.Empty
        End Try
        Return String.Empty
    End Function

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
