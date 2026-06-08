Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Database_Table_View
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/database/table", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("tablename")) Then
            Response.Redirect("~/setting/database/table", False)
            Exit Sub
        End If

        lblName.Text = Request.QueryString("tablename").ToString()
        hTitle.InnerText = "Table : " & Request.QueryString("tablename").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindFieldList(lblName.Text)
            BindData(lblName.Text, lblFieldName.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(lblName.Text, lblFieldName.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub lblFieldName_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(lblName.Text, lblFieldName.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Page" Then
                gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
                BindData(lblName.Text, lblFieldName.SelectedValue, txtSearch.Text)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(lblName.Text, lblFieldName.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        Try
            BuildPager()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub BindData(tableName As String, filterText As String, searchText As String)
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                Dim pkColumn As String = GetPrimaryKeyColumn(tableName)
                If Not String.IsNullOrEmpty(pkColumn) Then
                    search = String.Format("WHERE [{0}] = '{1}'", pkColumn, searchText.Replace("'", "''"))
                End If
                search = "WHERE Id='" & searchText & "'"
            End If

            Dim fieldList As String = GetSelectedFields()

            Dim thisString As String = String.Format("SELECT {0} FROM [{1}] {2}", fieldList, tableName, search)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindFieldList(tableName As String)
        Dim sql As String = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & tableName & "' ORDER BY ORDINAL_POSITION"

        Dim dt As DataTable = settingClass.GetDataTable(sql)

        lblFieldName.DataSource = dt
        lblFieldName.DataTextField = "COLUMN_NAME"
        lblFieldName.DataValueField = "COLUMN_NAME"
        lblFieldName.DataBind()
    End Sub

    Protected Function GetPrimaryKeyColumn(tableName As String) As String
        Try
            Dim sql As String = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey')=1 AND TABLE_NAME=@TableName"

            Using thisConn As New SqlConnection(ConfigurationManager.ConnectionStrings("ConnString").ConnectionString)
                Using thisCmd As New SqlCommand(sql, thisConn)
                    thisCmd.Parameters.AddWithValue("@TableName", tableName)
                    thisConn.Open()

                    Dim result As Object = thisCmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        Return result.ToString()
                    End If
                    Return String.Empty
                End Using
            End Using
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Private Function GetSelectedFields() As String
        Dim fields As New List(Of String)
        For Each item As ListItem In lblFieldName.Items
            If item.Selected Then
                fields.Add("[" & item.Value & "]")
            End If
        Next
        If fields.Count = 0 Then
            Return "*"
        End If
        Return String.Join(",", fields)
    End Function

    Protected Sub BuildPager()
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
