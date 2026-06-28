Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Database_Table_Add
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Private Property ColumnTable As DataTable
        Get
            If ViewState("ColumnTable") Is Nothing Then
                Dim dt As New DataTable()
                dt.Columns.Add("FieldName")
                dt.Columns.Add("DataType")
                dt.Columns.Add("IsKey")
                dt.Columns.Add("IsNotNull")

                dt.Rows.Add(dt.NewRow())
                ViewState("ColumnTable") = dt
            End If

            Return CType(ViewState("ColumnTable"), DataTable)
        End Get

        Set(value As DataTable)
            ViewState("ColumnTable") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/database/table", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindGrid()
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        MessageError(False, String.Empty)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then

                Dim row As DataRowView = CType(e.Row.DataItem, DataRowView)

                Dim fieldName As TextBox = TryCast(e.Row.FindControl("txtFieldName"), TextBox)
                Dim type As DropDownList = TryCast(e.Row.FindControl("ddlType"), DropDownList)
                Dim primaryKey As CheckBox = TryCast(e.Row.FindControl("chkKey"), CheckBox)
                Dim notNull As CheckBox = TryCast(e.Row.FindControl("chkNotNull"), CheckBox)

                If fieldName IsNot Nothing AndAlso row("FieldName") IsNot DBNull.Value Then
                    fieldName.Text = row("FieldName").ToString()
                End If

                If type IsNot Nothing AndAlso row("DataType") IsNot DBNull.Value Then
                    Dim val As String = row("DataType").ToString()

                    If type.Items.FindByValue(val) IsNot Nothing Then
                        type.SelectedValue = val
                    End If
                End If

                If primaryKey IsNot Nothing Then
                    Boolean.TryParse(row("IsKey").ToString(), primaryKey.Checked)
                End If

                If notNull IsNot Nothing Then
                    Boolean.TryParse(row("IsNotNull").ToString(), notNull.Checked)
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnAddRow_Click(sender As Object, e As EventArgs)
        Try
            SaveGrid()

            Dim dt As DataTable = ColumnTable
            dt.Rows.Add(dt.NewRow())

            ColumnTable = dt

            BindGrid()
        Catch ex As Exception
            MessageError(True, ex.Message)
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            SaveGrid()

            Dim cols As New List(Of String)
            Dim pkCols As New List(Of String)

            For Each row As DataRow In ColumnTable.Rows
                Dim colName As String = row("FieldName").ToString()
                If colName = "" Then Continue For

                Dim colDef As String = "[" & colName & "] " & row("DataType").ToString()

                If row("IsNotNull").ToString() = "True" Then
                    colDef &= " NOT NULL"
                End If

                cols.Add(colDef)

                If row("IsKey").ToString() = "True" Then
                    pkCols.Add("[" & colName & "]")
                End If
            Next

            Dim sql As String = "CREATE TABLE [" & txtName.Text.Trim() & "] (" & vbCrLf
            sql &= String.Join("," & vbCrLf, cols)

            If pkCols.Count > 0 Then
                sql &= "," & vbCrLf &
                   "CONSTRAINT [PK_" & txtName.Text.Trim() & "] PRIMARY KEY (" &
                   String.Join(",", pkCols) &
                   ")"
            End If
            sql &= vbCrLf & ")"

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand(sql, thisConn)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/database/table", False)
    End Sub

    Protected Sub BindGrid()
        Try
            gvList.DataSource = ColumnTable
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.Message)
        End Try
    End Sub

    Protected Sub SaveGrid()
        Try
            Dim dt As DataTable = ColumnTable
            dt.Rows.Clear()

            For Each row As GridViewRow In gvList.Rows
                Dim col As TextBox = TryCast(row.FindControl("txtFieldName"), TextBox)
                Dim type As DropDownList = TryCast(row.FindControl("ddlType"), DropDownList)
                Dim primaryKey As CheckBox = TryCast(row.FindControl("chkKey"), CheckBox)
                Dim notNull As CheckBox = TryCast(row.FindControl("chkNotNull"), CheckBox)

                Dim dr As DataRow = dt.NewRow()

                dr("FieldName") = If(col IsNot Nothing, col.Text.Trim(), "")
                dr("DataType") = If(type IsNot Nothing, type.SelectedValue, "")
                dr("IsKey") = If(primaryKey IsNot Nothing, primaryKey.Checked, False)
                dr("IsNotNull") = If(notNull IsNot Nothing, notNull.Checked, False)

                dt.Rows.Add(dr)
            Next

            ColumnTable = dt
        Catch ex As Exception
            MessageError(True, ex.Message)
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
